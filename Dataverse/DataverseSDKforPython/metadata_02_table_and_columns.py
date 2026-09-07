import os
from enum import IntEnum
from uuid import uuid4

from metadata_common import run_metadata_operation
from work_data_common import create_client


class Priority(IntEnum):
    LOW = 1
    MEDIUM = 2
    HIGH = 3


TABLE_SCHEMA_NAME = f"new_PyMetadata{uuid4().hex[:8]}"
PRIMARY_COLUMN = "new_RecordName"


def main() -> None:
    """Verify custom table creation, supported types, and column lifecycle."""
    table_created = False
    solution_name = os.getenv("DATAVERSE_SOLUTION_NAME") or None

    with create_client() as client:
        try:
            table_info = run_metadata_operation(
                lambda: client.tables.create(
                    TABLE_SCHEMA_NAME,
                    columns={
                        "new_Code": "string",
                        "new_Description": "memo",
                        "new_Quantity": "int",
                        "new_Price": "decimal",
                        "new_Ratio": "float",
                        "new_Active": "bool",
                        "new_DueDate": "datetime",
                        "new_Attachment": "file",
                        "new_Priority": Priority,
                    },
                    solution=solution_name,
                    primary_column=PRIMARY_COLUMN,
                    display_name="Python Metadata Verification",
                ),
                "creating the table",
            )
            table_created = True

            assert table_info.schema_name == TABLE_SCHEMA_NAME
            assert table_info["table_schema_name"] == TABLE_SCHEMA_NAME
            assert table_info.logical_name
            assert table_info.entity_set_name
            assert table_info.metadata_id
            print(f"Created schema: {table_info.schema_name}")
            print(f"Logical name: {table_info.logical_name}")
            print(f"Entity set: {table_info.entity_set_name}")
            print(f"Columns created: {table_info.columns_created}")

            retrieved = client.tables.get(TABLE_SCHEMA_NAME)
            assert retrieved is not None
            assert retrieved.metadata_id == table_info.metadata_id
            print("TableInfo property and legacy dict access verified")

            matching = client.tables.list(
                filter=f"SchemaName eq '{TABLE_SCHEMA_NAME}'",
                select=["LogicalName", "SchemaName", "EntitySetName"],
            )
            assert len(matching) == 1
            print("Filtered table listing verified")

            created = run_metadata_operation(
                lambda: client.tables.add_columns(
                    TABLE_SCHEMA_NAME,
                    {"new_Category": "string", "new_Notes": "multiline"},
                ),
                "adding columns",
            )
            assert set(created) == {"new_Category", "new_Notes"}
            print(f"Added columns: {created}")

            columns = client.tables.list_columns(
                TABLE_SCHEMA_NAME,
                select=["LogicalName", "SchemaName", "AttributeType"],
            )
            schema_names = {column.get("SchemaName") for column in columns}
            assert "new_Category" in schema_names
            assert "new_Notes" in schema_names
            print(f"Listed {len(columns)} columns on the custom table")

            removed = run_metadata_operation(
                lambda: client.tables.remove_columns(
                    TABLE_SCHEMA_NAME, ["new_Category", "new_Notes"]
                ),
                "removing columns",
            )
            assert set(removed) == {"new_Category", "new_Notes"}
            print(f"Removed columns: {removed}")
        finally:
            if table_created:
                run_metadata_operation(
                    lambda: client.tables.delete(TABLE_SCHEMA_NAME),
                    "deleting the table",
                )
                assert client.tables.get(TABLE_SCHEMA_NAME) is None
                print(f"Deleted custom table: {TABLE_SCHEMA_NAME}")


if __name__ == "__main__":
    main()
