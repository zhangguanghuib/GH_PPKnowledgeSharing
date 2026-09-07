from work_data_common import create_client, delete_records, unique_name


def main() -> None:
    """Verify mixed record creation and SQL querying in one Web API batch."""
    account_ids: list[str] = []

    with create_client() as client:
        try:
            batch = client.batch.new()
            batch.records.create("account", {"name": unique_name("Batch A")})
            batch.records.create("account", {"name": unique_name("Batch B")})
            batch.query.sql("SELECT TOP 3 accountid, name FROM account ORDER BY name")

            result = batch.execute()
            assert not result.has_errors
            assert len(result.responses) == 3
            account_ids = list(result.entity_ids)
            assert len(account_ids) == 2

            query_response = result.responses[2]
            assert query_response.data is not None
            rows = query_response.data.get("value", [])
            print(f"Batch operations succeeded: {len(result.succeeded)}")
            print(f"Created account IDs: {account_ids}")
            print(f"SQL rows returned: {len(rows)}")
        finally:
            delete_records(client, "account", account_ids)
            print(f"Cleaned up {len(account_ids)} accounts")


if __name__ == "__main__":
    main()
