from work_data_common import create_client


def main() -> None:
    """Read table and column metadata without changing the environment."""
    with create_client() as client:
        account = client.tables.get("account")
        assert account is not None
        print(f"Account logical name: {account.logical_name}")
        print(f"Account entity set: {account.entity_set_name}")

        tables = client.tables.list(
            select=["LogicalName", "SchemaName", "EntitySetName"]
        )
        assert any(table.get("LogicalName") == "account" for table in tables)
        print(f"Discovered {len(tables)} non-private tables")
        for table in tables[:5]:
            print(f"  {table.get('LogicalName')} -> {table.get('EntitySetName')}")

        columns = client.tables.list_columns(
            "account",
            select=["LogicalName", "SchemaName", "AttributeType"],
            filter="AttributeType eq 'String'",
        )
        assert columns
        assert all(column.get("AttributeType") == "String" for column in columns)
        print(f"Discovered {len(columns)} string columns on account")
        for column in columns[:10]:
            print(f"  {column.get('LogicalName')} ({column.get('AttributeType')})")


if __name__ == "__main__":
    main()
