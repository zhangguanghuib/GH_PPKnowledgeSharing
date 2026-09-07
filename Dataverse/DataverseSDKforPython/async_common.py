import asyncio
from concurrent.futures import ThreadPoolExecutor
from typing import Any
from uuid import uuid4

from azure.core.credentials import AccessToken
from azure.core.credentials_async import AsyncTokenCredential
from azure.identity import InteractiveBrowserCredential
from PowerPlatform.Dataverse.aio import AsyncDataverseClient


DATAVERSE_URL = "https://org5efadec2.crm.dynamics.com"


class AsyncInteractiveBrowserCredential(AsyncTokenCredential):
    """Adapt Azure Identity's browser credential to AsyncTokenCredential."""

    def __init__(self, **kwargs: Any) -> None:
        self._credential = InteractiveBrowserCredential(**kwargs)
        self._executor = ThreadPoolExecutor(max_workers=1)

    async def get_token(self, *scopes: str, **kwargs: Any) -> AccessToken:
        loop = asyncio.get_running_loop()
        return await loop.run_in_executor(
            self._executor,
            lambda: self._credential.get_token(*scopes, **kwargs),
        )

    async def close(self) -> None:
        self._credential.close()
        self._executor.shutdown(wait=False)


def create_async_client(
    credential: AsyncTokenCredential,
) -> AsyncDataverseClient:
    """Create an async client for the shared Dataverse test environment."""
    return AsyncDataverseClient(DATAVERSE_URL, credential)


def unique_name(prefix: str) -> str:
    """Create a recognizable, collision-resistant verification row name."""
    return f"Python async verification - {prefix} - {uuid4().hex[:8]}"
