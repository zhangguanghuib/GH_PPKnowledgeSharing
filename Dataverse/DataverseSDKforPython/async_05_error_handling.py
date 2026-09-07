import asyncio

import aiohttp
from PowerPlatform.Dataverse.core.errors import DataverseError

from async_common import AsyncInteractiveBrowserCredential, create_async_client


async def main() -> None:
    """Verify successful async I/O and the SDK's structured error branch."""
    credential = AsyncInteractiveBrowserCredential()

    try:
        async with create_async_client(credential) as client:
            try:
                accounts = await client.records.list(
                    "account", select=["accountid", "name"], top=1
                )
                print(f"Successful request returned {len(accounts)} account")
            except asyncio.TimeoutError:
                print("Request timed out.")
                raise
            except aiohttp.ClientError as error:
                print(f"Network error: {error}")
                raise
            except DataverseError as error:
                print(f"SDK error: {error.message}")
                raise

            try:
                await client.query.sql("")
            except DataverseError as error:
                assert error.code == "validation_error"
                print(f"Structured SDK error verified: {error.code}: {error.message}")
            else:
                raise AssertionError("Expected an empty SQL query to fail validation")
    finally:
        await credential.close()


if __name__ == "__main__":
    asyncio.run(main())