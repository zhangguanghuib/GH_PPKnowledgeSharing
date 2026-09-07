import asyncio

from async_common import AsyncInteractiveBrowserCredential, create_async_client, unique_name


async def main() -> None:
    """Create, retrieve, and delete a contact with async context managers."""
    credential = AsyncInteractiveBrowserCredential()
    contact_id: str | None = None

    try:
        async with create_async_client(credential) as client:
            try:
                contact_id = await client.records.create(
                    "contact",
                    {"firstname": "Async", "lastname": unique_name("Quick start")},
                )
                contact = await client.records.retrieve(
                    "contact",
                    contact_id,
                    select=["firstname", "lastname"],
                )
                assert contact is not None
                assert contact["firstname"] == "Async"
                print(f"Created: {contact['firstname']} {contact['lastname']}")
            finally:
                if contact_id is not None:
                    await client.records.delete("contact", contact_id)
                    assert await client.records.retrieve("contact", contact_id) is None
                    print("Delete verified")
    finally:
        await credential.close()


if __name__ == "__main__":
    asyncio.run(main())
