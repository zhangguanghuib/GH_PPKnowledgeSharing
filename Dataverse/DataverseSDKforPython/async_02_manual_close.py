import asyncio

from async_common import AsyncInteractiveBrowserCredential, create_async_client, unique_name


async def main() -> None:
    """Verify standalone use, explicit aclose, and closed-client behavior."""
    credential = AsyncInteractiveBrowserCredential()
    client = create_async_client(credential)
    account_id: str | None = None

    try:
        account_id = await client.records.create(
            "account", {"name": unique_name("Manual close")}
        )
        account = await client.records.retrieve(
            "account", account_id, select=["name"]
        )
        assert account is not None
        print(f"Standalone client created: {account['name']}")
    finally:
        if account_id is not None:
            await client.records.delete("account", account_id)
            print("Temporary account deleted")
        await client.aclose()
        await client.aclose()
        await credential.close()

    try:
        await client.records.retrieve(
            "account", "00000000-0000-0000-0000-000000000000"
        )
    except RuntimeError as error:
        print(f"Closed-client guard verified: {error}")
    else:
        raise AssertionError("A closed AsyncDataverseClient accepted an operation")


if __name__ == "__main__":
    asyncio.run(main())
