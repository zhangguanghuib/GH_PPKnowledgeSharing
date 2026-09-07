import asyncio

from async_common import AsyncInteractiveBrowserCredential, create_async_client, unique_name


async def main() -> None:
    """Verify async batches and an atomic content-ID changeset."""
    credential = AsyncInteractiveBrowserCredential()
    batch_account_ids: list[str] = []
    linked_account_id: str | None = None
    contact_id: str | None = None

    try:
        async with create_async_client(credential) as client:
            try:
                batch = client.batch.new()
                batch.records.create("account", {"name": unique_name("Alpha")})
                batch.records.create("account", {"name": unique_name("Beta")})
                result = await batch.execute()
                assert not result.has_errors
                batch_account_ids = list(result.entity_ids)
                assert len(batch_account_ids) == 2
                print(f"Batch created {len(batch_account_ids)} accounts")

                linked_account_id = await client.records.create(
                    "account", {"name": unique_name("Changeset account")}
                )
                batch = client.batch.new()
                async with batch.changeset() as changeset:
                    contact_ref = changeset.records.create(
                        "contact", {"firstname": "Async", "lastname": "Changeset"}
                    )
                    changeset.records.update(
                        "account",
                        linked_account_id,
                        {"primarycontactid@odata.bind": contact_ref},
                    )
                result = await batch.execute()
                assert not result.has_errors
                assert len(result.responses) == 2
                contact_id = result.responses[0].entity_id
                assert contact_id is not None

                linked_account = await client.records.retrieve(
                    "account",
                    linked_account_id,
                    select=["name"],
                    expand=["primarycontactid"],
                )
                assert linked_account is not None
                primary_contact = linked_account.get("primarycontactid") or {}
                assert primary_contact.get("contactid") == contact_id
                print("Atomic changeset and content-ID binding verified")
            finally:
                if linked_account_id is not None:
                    await client.records.delete("account", linked_account_id)
                if contact_id is not None:
                    await client.records.delete("contact", contact_id)
                for account_id in batch_account_ids:
                    await client.records.delete("account", account_id)
                print(
                    f"Cleaned up {len(batch_account_ids) + int(linked_account_id is not None)} "
                    f"accounts and {int(contact_id is not None)} contact"
                )
    finally:
        await credential.close()


if __name__ == "__main__":
    asyncio.run(main())
