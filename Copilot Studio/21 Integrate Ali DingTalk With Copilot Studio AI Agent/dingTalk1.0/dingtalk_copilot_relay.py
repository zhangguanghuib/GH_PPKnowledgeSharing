"""
DingTalk (钉钉) <-> Microsoft Copilot Studio relay (sample, not production-hardened).

Flow:
    钉钉用户 -> 钉钉机器人 (Stream 模式, 无需公网回调地址)
             -> 本服务 -> Direct Line API -> Copilot Studio 智能体
             -> 本服务 -> 钉钉 sessionWebhook 回复

Setup
-----
1. Copilot Studio: 选择以下一种方式连接到“指定的智能体”, 然后发布智能体。
   智能体若启用了“要求用户登录”, 本示例不处理登录卡片。
   方式 A (推荐, 生产): Secret
     设置 -> 安全 -> Web 渠道安全 (保持“需要安全访问”开启), 复制 Secret 1。
     Secret 只属于这一个智能体, 由它隐式决定通讯对象。
   方式 B (测试 / 公开智能体): Token Endpoint
     需关闭 Web 渠道安全; 在 渠道 -> 移动应用 页面复制 Token Endpoint, 形如
     https://<环境域名>/powervirtualagents/botsbyschema/<智能体架构名>/directline/token?api-version=2022-03-01-preview
     URL 中显式包含智能体架构名 (或 /bots/<智能体 GUID>/)。
   两者都配置时优先使用 Secret。
2. 钉钉开放平台: 创建企业内部应用 -> 添加“机器人”能力 -> 消息接收模式选 Stream 模式,
   记下 Client ID (AppKey) 与 Client Secret (AppSecret), 发布应用。
3. pip install -r requirements.txt
4. 在本脚本所在目录创建 .env 文件 (可复制 .env.example), 填写:
     DINGTALK_CLIENT_ID=...
     DINGTALK_CLIENT_SECRET=...
     DIRECTLINE_SECRET=...            # 方式 A
     COPILOT_TOKEN_ENDPOINT=          # 方式 B (不设置 DIRECTLINE_SECRET 时生效)
   这四项仅从 .env 读取, 不读取或修改操作系统环境变量。
   .env 已被 Git 忽略, 不要提交真实密钥。
5. 运行:
     # 可选: 区域化 Direct Line 端点, 例如 https://europe.directline.botframework.com
     export DIRECTLINE_ENDPOINT=https://directline.botframework.com
     python dingtalk_copilot_relay.py

Production notes: store secrets in Key Vault; persist the conversation map
(e.g. Redis/Table Storage) instead of in-memory; add retries/logging; review
cross-border data transfer requirements (钉钉数据在中国境内, Copilot Studio 在 Azure 全球区域).
"""

import asyncio
import logging
import os
import threading
import time
from pathlib import Path

import requests
import dingtalk_stream
from dingtalk_stream import AckMessage
from dotenv import dotenv_values

logging.basicConfig(level=logging.INFO, format="%(asctime)s %(levelname)s %(message)s")
log = logging.getLogger("relay")

ENV_PATH = Path(__file__).resolve().with_name(".env")
if not ENV_PATH.is_file():
    raise SystemExit(f"Create {ENV_PATH} using .env.example and fill in the configuration.")
config = dotenv_values(ENV_PATH, encoding="utf-8-sig", interpolate=False)
DINGTALK_CLIENT_ID = (config.get("DINGTALK_CLIENT_ID") or "").strip()
DINGTALK_CLIENT_SECRET = (config.get("DINGTALK_CLIENT_SECRET") or "").strip()
if not DINGTALK_CLIENT_ID or not DINGTALK_CLIENT_SECRET:
    raise SystemExit(f"Set DINGTALK_CLIENT_ID and DINGTALK_CLIENT_SECRET in {ENV_PATH}.")
DL_ENDPOINT = os.environ.get("DIRECTLINE_ENDPOINT", "https://directline.botframework.com").rstrip("/")
DL_SECRET = (config.get("DIRECTLINE_SECRET") or "").strip()
TOKEN_ENDPOINT = (config.get("COPILOT_TOKEN_ENDPOINT") or "").strip()
if not DL_SECRET and not TOKEN_ENDPOINT:
    raise SystemExit(f"Set DIRECTLINE_SECRET (recommended) or COPILOT_TOKEN_ENDPOINT in {ENV_PATH}.")
AUTH_MODE = "secret" if DL_SECRET else "token-endpoint"
REPLY_TIMEOUT_S = 45      # max wait for the agent's reply
QUIET_PERIOD_S = 2.0      # stop collecting once no new agent message for this long
POLL_INTERVAL_S = 0.8


class DirectLineSession:
    """One Direct Line conversation per DingTalk (conversation, user) pair."""

    def __init__(self, user_id: str):
        self.user_id = user_id
        self.lock = threading.Lock()
        self._start()

    @staticmethod
    def _bootstrap_credential() -> str:
        """Credential used to start a conversation; it determines which agent we talk to."""
        if AUTH_MODE == "secret":
            return DL_SECRET  # the secret belongs to exactly one agent
        # Token endpoint URL contains the agent schema name / GUID explicitly.
        t = requests.get(TOKEN_ENDPOINT, timeout=15)
        t.raise_for_status()
        return t.json()["token"]

    def _start(self):
        credential = self._bootstrap_credential()
        r = requests.post(
            f"{DL_ENDPOINT}/v3/directline/conversations",
            headers={"Authorization": f"Bearer {credential}"},
            timeout=15,
        )
        r.raise_for_status()
        body = r.json()
        self.conversation_id = body["conversationId"]
        self.token = body.get("token") or credential  # conversation-scoped token
        self.watermark = None
        log.info("Started Direct Line conversation %s for %s (auth=%s)",
                 self.conversation_id, self.user_id, AUTH_MODE)

    def _headers(self):
        return {"Authorization": f"Bearer {self.token}", "Content-Type": "application/json"}

    def ask(self, text: str, user_name: str = "") -> list[str]:
        with self.lock:
            activity = {
                "type": "message",
                "from": {"id": self.user_id, "name": user_name},
                "locale": "zh-CN",
                "text": text,
            }
            url = f"{DL_ENDPOINT}/v3/directline/conversations/{self.conversation_id}/activities"
            r = requests.post(url, headers=self._headers(), json=activity, timeout=15)
            if r.status_code in (401, 403, 404):  # expired token / conversation -> restart once
                log.warning("Direct Line conversation expired (%s), restarting", r.status_code)
                self._start()
                url = f"{DL_ENDPOINT}/v3/directline/conversations/{self.conversation_id}/activities"
                r = requests.post(url, headers=self._headers(), json=activity, timeout=15)
            r.raise_for_status()
            return self._collect_replies()

    def _collect_replies(self) -> list[str]:
        replies: list[str] = []
        deadline = time.monotonic() + REPLY_TIMEOUT_S
        last_new = None
        while time.monotonic() < deadline:
            params = {"watermark": self.watermark} if self.watermark else {}
            r = requests.get(
                f"{DL_ENDPOINT}/v3/directline/conversations/{self.conversation_id}/activities",
                headers=self._headers(), params=params, timeout=15,
            )
            r.raise_for_status()
            body = r.json()
            self.watermark = body.get("watermark", self.watermark)
            for act in body.get("activities", []):
                if act.get("from", {}).get("id") == self.user_id or act.get("type") != "message":
                    continue
                msg = activity_to_markdown(act)
                if msg:
                    replies.append(msg)
                    last_new = time.monotonic()
            if last_new and time.monotonic() - last_new >= QUIET_PERIOD_S:
                break
            time.sleep(POLL_INTERVAL_S)
        return replies


def activity_to_markdown(act: dict) -> str:
    """DingTalk can't render Adaptive Cards; flatten text, card TextBlocks and suggested actions."""
    parts = []
    if act.get("text"):
        parts.append(act["text"])
    for att in act.get("attachments", []) or []:
        if att.get("contentType") == "application/vnd.microsoft.card.adaptive":
            parts.extend(_adaptive_texts(att.get("content", {})))
        elif isinstance(att.get("content"), dict):  # hero/thumbnail cards
            c = att["content"]
            parts.extend(x for x in (c.get("title"), c.get("subtitle"), c.get("text")) if x)
    actions = (act.get("suggestedActions") or {}).get("actions", [])
    if actions:
        parts.append("可选: " + " / ".join(a.get("title") or str(a.get("value", "")) for a in actions))
    return "\n\n".join(p for p in parts if p).strip()


def _adaptive_texts(node) -> list[str]:
    out = []
    if isinstance(node, dict):
        if node.get("type") == "TextBlock" and node.get("text"):
            out.append(node["text"])
        for v in node.values():
            out.extend(_adaptive_texts(v))
    elif isinstance(node, list):
        for v in node:
            out.extend(_adaptive_texts(v))
    return out


_sessions: dict[str, DirectLineSession] = {}
_sessions_lock = threading.Lock()


def get_session(key: str, user_id: str) -> DirectLineSession:
    with _sessions_lock:
        if key not in _sessions:
            _sessions[key] = DirectLineSession(user_id)
        return _sessions[key]


class CopilotStudioHandler(dingtalk_stream.ChatbotHandler):
    async def process(self, callback: dingtalk_stream.CallbackMessage):
        incoming = dingtalk_stream.ChatbotMessage.from_dict(callback.data)
        text = (incoming.text.content if incoming.text else "").strip()
        if not text:
            return AckMessage.STATUS_OK, "OK"

        sender = incoming.sender_staff_id or incoming.sender_id
        user_id = f"dingtalk-{sender}"
        key = f"{incoming.conversation_id}:{sender}"  # per user, also inside group chats

        # Ack quickly; do the slow Direct Line round-trip in the background.
        asyncio.get_running_loop().create_task(self._answer(incoming, key, user_id, text))
        return AckMessage.STATUS_OK, "OK"

    async def _answer(self, incoming, key, user_id, text):
        try:
            session = await asyncio.to_thread(get_session, key, user_id)
            replies = await asyncio.to_thread(session.ask, text, incoming.sender_nick or "")
            if not replies:
                replies = ["抱歉, 智能体暂时没有回复, 请稍后再试。"]
            for reply in replies:
                await asyncio.to_thread(self.reply_markdown, "Copilot", reply, incoming)
        except Exception:
            log.exception("Relay failed for %s", key)
            await asyncio.to_thread(self.reply_text, "服务暂时不可用, 请稍后再试。", incoming)


def main():
    credential = dingtalk_stream.Credential(
        DINGTALK_CLIENT_ID, DINGTALK_CLIENT_SECRET
    )
    client = dingtalk_stream.DingTalkStreamClient(credential)
    client.register_callback_handler(dingtalk_stream.ChatbotMessage.TOPIC, CopilotStudioHandler())
    log.info("DingTalk <-> Copilot Studio relay started (Direct Line: %s, auth=%s)", DL_ENDPOINT, AUTH_MODE)
    client.start_forever()


if __name__ == "__main__":
    main()