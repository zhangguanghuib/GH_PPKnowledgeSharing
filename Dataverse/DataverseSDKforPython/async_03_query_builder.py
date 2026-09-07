import asyncio

from PowerPlatform.Dataverse.models.filters import col

from async_common import AsyncInteractiveBrowserCredential, create_async_client


async def main() -> None:
    """Verify eager and lazy async QueryBuilder execution."""
    credential = AsyncInteractiveBrowserCredential()

    try:
        async with create_async_client(credential) as client:
            result = await (
                client.query.builder("account")
                .select("name", "telephone1")
                .where(col("statecode") == 0)
                .top(10)
                .execute()
            )
            print(f"Eager query returned {len(result)} active accounts")
            for record in result:
                print(f"  {record.get('name')}")

            total = 0
            async for page in (
                client.query.builder("account")
                .select("name")
                .where(col("statecode") == 0)
                .top(5)
                .page_size(2)
                .execute_pages()
            ):
                total += len(page)
                print(f"Page returned {len(page)} accounts")
            assert total <= 5
            print(f"Lazy paging returned {total} accounts")
    finally:
        await credential.close()


if __name__ == "__main__":
    asyncio.run(main())
