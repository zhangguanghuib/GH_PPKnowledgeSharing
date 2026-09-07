from work_data_common import create_client, delete_records, unique_name


def main() -> None:
    """Verify CreateMultiple and UpdateMultiple, then clean up synchronously."""
    account_ids: list[str] = []

    with create_client() as client:
        try:
            payloads = [
                {"name": unique_name("Bulk A")},
                {"name": unique_name("Bulk B")},
                {"name": unique_name("Bulk C")},
            ]
            created = client.records.create("account", payloads)
            assert isinstance(created, list)
            assert len(created) == len(payloads)
            assert all(isinstance(record_id, str) for record_id in created)
            account_ids = created
            print({"created_ids": account_ids})

            client.records.update(
                "account",
                account_ids,
                {"telephone1": "555-0200"},
            )

            for account_id in account_ids:
                account = client.records.retrieve(
                    "account", account_id, select=["telephone1"]
                )
                assert account is not None
                assert account["telephone1"] == "555-0200"
            print(f"Bulk update verified for {len(account_ids)} accounts")
        finally:
            delete_records(client, "account", account_ids)
            print(f"Cleaned up {len(account_ids)} accounts")


if __name__ == "__main__":
    main()
