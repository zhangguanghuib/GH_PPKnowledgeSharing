import logging
import os

import jwt
from dotenv import load_dotenv
from jwt import PyJWKClient
from pydantic import AnyHttpUrl
from starlette.requests import Request

from mcp.server.auth.middleware.auth_context import get_access_token
from mcp.server.auth.provider import AccessToken, TokenVerifier
from mcp.server.auth.settings import AuthSettings
from mcp.server.fastmcp import Context, FastMCP

load_dotenv()

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger("CopilotStudio-MCP-Server03")

# ---------------------------------------------------------------------------
# Microsoft Entra ID (Azure AD) OAuth 2.0 configuration
#
# This server acts as an OAuth 2.0 *resource server*: Microsoft Entra ID is
# the authorization server that authenticates the user (work/school account)
# and issues the access token. Microsoft Copilot Studio / Copilot agents
# discover this via the Protected Resource Metadata endpoint and drive the
# user through an Entra ID sign-in before ever calling a tool here.
# ---------------------------------------------------------------------------
def _required_env(name: str) -> str:
    value = os.environ.get(name)
    if not value:
        raise RuntimeError(
            f"Missing required environment variable '{name}'. "
            "Copy .env.example to .env (or set it as an App Service setting) and fill it in."
        )
    return value


ENTRA_TENANT_ID = _required_env("ENTRA_TENANT_ID")
# Application (client) ID of the App Registration that represents this MCP server.
ENTRA_CLIENT_ID = _required_env("ENTRA_CLIENT_ID")
# Public HTTPS URL this MCP server is reachable at (used as the OAuth resource identifier).
MCP_SERVER_URL = os.environ.get("MCP_SERVER_URL", "http://localhost:8000")
# Optional space-separated list of scopes Copilot must request/present, e.g. "api://<client-id>/mcp.invoke"
REQUIRED_SCOPES = [s for s in os.environ.get("ENTRA_REQUIRED_SCOPES", "").split() if s] or None

ISSUER_URL = f"https://login.microsoftonline.com/{ENTRA_TENANT_ID}/v2.0"
JWKS_URI = f"https://login.microsoftonline.com/{ENTRA_TENANT_ID}/discovery/v2.0/keys"
# Entra ID may issue v1 or v2 access tokens depending on the App Registration's
# `accessTokenAcceptedVersion` manifest value. v1 tokens use the sts.windows.net
# issuer; v2 tokens use the login.microsoftonline.com/<tenant>/v2.0 issuer.
VALID_ISSUERS = {
    f"https://login.microsoftonline.com/{ENTRA_TENANT_ID}/v2.0",
    f"https://sts.windows.net/{ENTRA_TENANT_ID}/",
}
# Entra ID access tokens issued for this API may carry either the raw client id
# or the "api://<client-id>" App ID URI as the audience, depending on how the
# App Registration's "Expose an API" blade is configured.
VALID_AUDIENCES = [ENTRA_CLIENT_ID, f"api://{ENTRA_CLIENT_ID}"]

_jwks_client = PyJWKClient(JWKS_URI, cache_keys=True)

logger.info(
    "Starting demo-mcp with issuer=%s resource=%s required_scopes=%s",
    ISSUER_URL,
    MCP_SERVER_URL,
    REQUIRED_SCOPES,
)


def _expand_token_scopes(raw_scopes: list[str], required_scopes: list[str] | None) -> list[str]:
    """Normalize scope names so Entra ID's short `scp` values can satisfy
    MCP middleware checks that are configured with full custom-scope URIs.

    Example:
    - token `scp`: `mcp.invoke`
    - configured required scope: `api://<app-id>/mcp.invoke`
    """
    expanded_scopes = set(raw_scopes)

    for scope in raw_scopes:
        if "/" in scope:
            expanded_scopes.add(scope.rsplit("/", 1)[-1])

    for required_scope in required_scopes or []:
        if "/" not in required_scope:
            continue

        short_scope = required_scope.rsplit("/", 1)[-1]
        if short_scope in expanded_scopes:
            expanded_scopes.add(required_scope)

    return sorted(expanded_scopes)


class EntraIDTokenVerifier(TokenVerifier):
    """Validates OAuth 2.0 bearer access tokens issued by Microsoft Entra ID."""

    async def verify_token(self, token: str) -> AccessToken | None:
        logger.info("Validating Entra ID bearer token")
        try:
            signing_key = _jwks_client.get_signing_key_from_jwt(token)
            claims = jwt.decode(
                token,
                signing_key.key,
                algorithms=["RS256"],
                audience=VALID_AUDIENCES,
                issuer=ISSUER_URL,
                options={"require": ["exp", "iat", "aud", "iss"]},
            )
        except jwt.PyJWTError as exc:
            logger.warning("Rejected invalid Entra ID access token: %s", exc)
            return None

        scopes_claim = claims.get("scp") or claims.get("roles") or ""
        raw_scopes = scopes_claim.split() if isinstance(scopes_claim, str) else list(scopes_claim)
        scopes = _expand_token_scopes(raw_scopes, REQUIRED_SCOPES)
        logger.info(
            "Accepted Entra ID token subject=%s client=%s raw_scopes=%s normalized_scopes=%s",
            claims.get("oid") or claims.get("sub"),
            claims.get("appid") or claims.get("azp") or claims.get("sub", "unknown"),
            raw_scopes,
            scopes,
        )

        return AccessToken(
            token=token,
            client_id=claims.get("appid") or claims.get("azp") or claims.get("sub", "unknown"),
            scopes=scopes,
            expires_at=claims.get("exp"),
            resource=MCP_SERVER_URL,
            # "sub"/"oid" identify the signed-in Entra ID user for this token.
            subject=claims.get("oid") or claims.get("sub"),
            claims=claims,
        )


mcp = FastMCP(
    "demo-mcp",
    stateless_http=True,
    host="0.0.0.0",
    auth=AuthSettings(
        issuer_url=AnyHttpUrl(ISSUER_URL),
        resource_server_url=AnyHttpUrl(MCP_SERVER_URL),
        required_scopes=REQUIRED_SCOPES,
    ),
    token_verifier=EntraIDTokenVerifier(),
)


def _authorize_request(ctx: Context) -> AccessToken:
    """Pull the incoming HTTP request out of the tool call context, read its
    ``Authorization`` header, and return the validated Entra ID access token.

    Every tool calls this first so each method independently confirms who is
    calling it (the transport-level middleware already rejects requests with
    a missing/invalid token, but this gives per-tool visibility into the
    caller and a defense-in-depth check).
    """
    raw_request: Request | None = ctx.request_context.request
    logger.info(
        "Authorizing request for tool call request_id=%s path=%s",
        ctx.request_context.request_id,
        raw_request.url.path if raw_request else None,
    )
    auth_header = raw_request.headers.get("authorization") if raw_request else None

    if not auth_header or not auth_header.lower().startswith("bearer "):
        logger.warning("Authorization header missing or malformed for request_id=%s", ctx.request_context.request_id)
        raise PermissionError("Missing or malformed Authorization header (expected 'Bearer <token>').")

    bearer_token = auth_header[7:].strip()
    logger.info(
        "Authorization header bearer token request_id=%s token=%s",
        ctx.request_context.request_id,
        bearer_token,
    )

    access_token = get_access_token()
    if access_token is None:
        logger.warning("Validated access token missing from auth context for request_id=%s", ctx.request_context.request_id)
        raise PermissionError("Authorization bearer token could not be validated.")

    logger.info(
        "Authorized request_id=%s subject=%s client=%s scopes=%s",
        ctx.request_context.request_id,
        access_token.subject,
        access_token.client_id,
        access_token.scopes,
    )

    return access_token


@mcp.tool("add", description="Add two numbers")
def add(a: int, b: int, ctx: Context) -> int:
    logger.info("Tool add invoked with a=%s b=%s request_id=%s", a, b, ctx.request_context.request_id)
    caller = _authorize_request(ctx)
    logger.info("add called by Entra ID subject=%s client=%s", caller.subject, caller.client_id)
    return a + b

@mcp.tool("minus", description="Subtract two numbers")
def minus(a: int, b: int, ctx: Context) -> int:
    logger.info("Tool minus invoked with a=%s b=%s request_id=%s", a, b, ctx.request_context.request_id)
    caller = _authorize_request(ctx)
    logger.info("minus called by Entra ID subject=%s client=%s", caller.subject, caller.client_id)
    return a - b

if __name__ == "__main__":
    mcp.run(transport="streamable-http")