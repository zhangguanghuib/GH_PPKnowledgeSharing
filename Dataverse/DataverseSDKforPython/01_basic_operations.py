from work_data_common import create_client, unique_name


def main() -> None:
    """Verify create, retrieve, expand, update, and delete operations."""
    account_id: str | None = None

    with create_client() as client:
        try:
            account_name = unique_name("CRUD")
            account_id = client.records.create("account", {"name": account_name})

            account = client.records.retrieve(
                "account",
                account_id,
                select=["name", "telephone1"],
                expand=["primarycontactid"],
            )
            assert account is not None
            assert account["name"] == account_name

            primary_contact = account.get("primarycontactid") or {}
            print(f"Created: {account['name']} ({account_id})")
            print(f"Primary contact: {primary_contact.get('fullname', '<none>')}")

            client.records.update("account", account_id, {"telephone1": "555-0199"})
            updated = client.records.retrieve(
                "account", account_id, select=["name", "telephone1"]
            )
            assert updated is not None
            assert updated["telephone1"] == "555-0199"
            print("Update verified: telephone1=555-0199")
        finally:
            if account_id is not None:
                client.records.delete("account", account_id)
                assert client.records.retrieve("account", account_id) is None
                print("Delete verified")


if __name__ == "__main__":
    main()
