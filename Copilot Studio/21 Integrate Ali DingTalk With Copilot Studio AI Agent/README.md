# Integrated Ali DingTalk with Copilot Studio AI Agent 

## 1. Create a Copilot Studio AI Agent
<img width="1581" height="1383" alt="image" src="https://github.com/user-attachments/assets/2ea631a4-11bc-4f33-bfd5-462fab9999e6" /><br/>
Go to Settings->Security->Web channel security<br/>
<img width="2021" height="726" alt="image" src="https://github.com/user-attachments/assets/91aae21c-8fa9-4762-8382-5f32d4a5f1ee" /><br/>
Then get the Direct Line API Endpoint<br/>
<img width="2407" height="1186" alt="image" src="https://github.com/user-attachments/assets/1b88c736-52aa-482c-90a5-6188083dd974" /><br/>

2. Go to https://open-dev.dingtalk.com/ (钉钉开发者平台)
   创建应用：<br/>
   <img width="2456" height="947" alt="image" src="https://github.com/user-attachments/assets/13d025af-9386-45bc-86d5-720ca06cd3cc" /><br/>
   <img width="2452" height="837" alt="image" src="https://github.com/user-attachments/assets/67647007-00c8-4e22-9167-6efc679f5198" /><br/>
   Configure the mode of message receive<br/>
   <img width="1771" height="1467" alt="image" src="https://github.com/user-attachments/assets/6fdc356e-23fb-4463-9fab-e1ac4ed14a5f" /><br/>
   Copy "Client Id" and "Client Secret Id" for future use:<br/>
   <img width="1921" height="1073" alt="image" src="https://github.com/user-attachments/assets/c1d51268-2026-4b4c-9fc0-2bea9fc73d23" /><br/>

3. Write the below code:
   Parameters<br/>
   ```
   DINGTALK_CLIENT_ID= DingTalk Robot Client Id from https://open-dev.dingtalk.com
   DINGTALK_CLIENT_SECRET=  DingTalk Robot Client Secret from https://open-dev.dingtalk.com
   # Set DIRECTLINE_SECRET or COPILOT_TOKEN_ENDPOINT; the secret takes priority.
   DIRECTLINE_SECRET= Secret from Copilot Studio(See Step #1)
   COPILOT_TOKEN_ENDPOINT= Direct Line API Endpoint (See Step #1)
   ```
   Python Package Dependencies<br/>
   ```
   dingtalk-stream
   requests
    python-dotenv
   ```
4. Launch the Python Code
   ```ps
   .\venvLangGraph\Scripts\activate
   (venvLangGraph) PS  cd .\dingTalk1.0\          
   (venvLangGraph) PS  python.exe .\dingtalk_copilot_relay.py  
   ```
5. Send message from DingTalk to Copilot Agent and get answer from Copilot Agent:
   <img width="1997" height="707" alt="image" src="https://github.com/user-attachments/assets/6ad86597-313b-44aa-8f87-bfa10b1248d1" /><br/>

   From the Console log we can see the message can be sent from DingTalk to Copilot Agent and also DingTalk can get response from Copilot Agent<br/>
  <img width="1711" height="526" alt="image" src="https://github.com/user-attachments/assets/9f349b8d-cb46-45ba-8804-ee7ea4937676" /><br/>

  6. Attach the Source code for your reference
     ```py
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
      3. pip install dingtalk-stream requests python-dotenv
      4. 在本文件同一目录创建 .env (同名环境变量优先于 .env), 内容例如:
           DINGTALK_CLIENT_ID=...
           DINGTALK_CLIENT_SECRET=...
           DIRECTLINE_SECRET=...                   # 方式 A
           # COPILOT_TOKEN_ENDPOINT=...            # 方式 B (不设置 DIRECTLINE_SECRET 时生效)
           # 可选: 区域化 Direct Line 端点, 例如 https://europe.directline.botframework.com
           # DIRECTLINE_ENDPOINT=https://directline.botframework.com
           # 可选: LOG_LEVEL=DEBUG 打印智能体返回的每条活动
         然后运行: python dingtalk_copilot_relay.py
      
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
      from dotenv import load_dotenv
      
      # Read settings from the .env file next to this script (utf-8-sig tolerates a BOM from Notepad).
      # Real environment variables, if set, take precedence over .env values.
      load_dotenv(Path(__file__).resolve().with_name(".env"), encoding="utf-8-sig")
      
      logging.basicConfig(level=os.environ.get("LOG_LEVEL", "INFO").upper(),
                          format="%(asctime)s %(levelname)s %(message)s")
      log = logging.getLogger("relay")
      
      DL_ENDPOINT = os.environ.get("DIRECTLINE_ENDPOINT", "https://directline.botframework.com").rstrip("/")
      DL_SECRET = os.environ.get("DIRECTLINE_SECRET", "").strip()
      TOKEN_ENDPOINT = os.environ.get("COPILOT_TOKEN_ENDPOINT", "").strip()
      if not DL_SECRET and not TOKEN_ENDPOINT:
          raise SystemExit("Set DIRECTLINE_SECRET (recommended) or COPILOT_TOKEN_ENDPOINT.")
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
                  log.info("[DirectLine] POST message -> HTTP %s %s", r.status_code, r.text[:200])
                  r.raise_for_status()
                  started = time.monotonic()
                  replies = self._collect_replies()
                  log.info("[DirectLine] got %d reply message(s) in %.1fs (conversation %s)",
                           len(replies), time.monotonic() - started, self.conversation_id)
                  return replies
      
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
                      log.debug("[DirectLine] activity type=%s from=%s text=%r", act.get("type"),
                                act.get("from", {}).get("id"), (act.get("text") or "")[:80])
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
      _background_tasks: set = set()  # keep references so asyncio does not drop running tasks
      
      
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
      
              log.info("[DingTalk] received from %s (%s): %r", user_id, incoming.conversation_type, text[:100])
              # Ack quickly; do the slow Direct Line round-trip in the background.
              task = asyncio.get_running_loop().create_task(self._answer(incoming, key, user_id, text))
              _background_tasks.add(task)
              task.add_done_callback(_background_tasks.discard)
              return AckMessage.STATUS_OK, "OK"
      
          def _send(self, reply: str, incoming) -> None:
              """Reply via sessionWebhook; the SDK swallows errors, so check the result explicitly."""
              result = self.reply_markdown("Copilot", reply, incoming)
              if result is None:
                  log.error("[DingTalk] reply failed (HTTP error, see SDK log above)")
              elif isinstance(result, dict) and result.get("errcode", 0) != 0:
                  log.error("[DingTalk] reply rejected: %s", result)
              else:
                  log.info("[DingTalk] reply sent: %r", reply[:80])
      
          async def _answer(self, incoming, key, user_id, text):
              try:
                  session = await asyncio.to_thread(get_session, key, user_id)
                  replies = await asyncio.to_thread(session.ask, text, incoming.sender_nick or "")
                  if not replies:
                      replies = ["抱歉, 智能体暂时没有回复, 请稍后再试。"]
                  for reply in replies:
                      await asyncio.to_thread(self._send, reply, incoming)
              except Exception:
                  log.exception("Relay failed for %s", key)
                  await asyncio.to_thread(self.reply_text, "服务暂时不可用, 请稍后再试。", incoming)
      
      
      def main():
          credential = dingtalk_stream.Credential(
              os.environ["DINGTALK_CLIENT_ID"], os.environ["DINGTALK_CLIENT_SECRET"]
          )
          client = dingtalk_stream.DingTalkStreamClient(credential)
          client.register_callback_handler(dingtalk_stream.ChatbotMessage.TOPIC, CopilotStudioHandler())
          log.info("DingTalk <-> Copilot Studio relay started (Direct Line: %s, auth=%s)", DL_ENDPOINT, AUTH_MODE)
          client.start_forever()
      
      
      if __name__ == "__main__":
          main()
     ```



