from uuid import uuid4

from metadata_common import run_metadata_operation
from work_data_common import create_client


SUFFIX = uuid4().hex[:8]
TABLE_SCHEMA_NAME = f"new_PyKey{SUFFIX}"
KEY_SCHEMA_NAME = f"new_pykey_code_{SUFFIX}"


def main() -> None:
    """Verify creation, discovery, and deletion of an alternate key."""
    table_created = False
    key_id: str | None = None

    with create_client() as client:
        try:
            run_metadata_operation(
                lambda: client.tables.create(
                    TABLE_SCHEMA_NAME,
                    columns={
                        "new_Code": "string",
                        "new_Description": "string",
                    },
                    primary_column="new_RecordName",
                    display_name="Python Alternate Key Verification",
                ),
                "creating the alternate-key table",
            )
            table_created = True

            key = run_metadata_operation(
                lambda: client.tables.create_alternate_key(
                    TABLE_SCHEMA_NAME,
                    KEY_SCHEMA_NAME,
                    ["new_code"],
                    display_name="Code",
                ),
                "creating the alternate key",
            )
            key_id = key.metadata_id
            assert key.schema_name == KEY_SCHEMA_NAME
            assert key.key_attributes == ["new_code"]
            print(f"Created key {key.schema_name} ({key.metadata_id})")
            print(f"Initial status: {key.status}")

            keys = client.tables.get_alternate_keys(TABLE_SCHEMA_NAME)
            matching = [item for item in keys if item.schema_name == KEY_SCHEMA_NAME]
            assert len(matching) == 1
            print(f"Current status: {matching[0].status}")

            run_metadata_operation(
                lambda: client.tables.delete_alternate_key(
                    TABLE_SCHEMA_NAME, key_id
                ),
                "deleting the alternate key",
            )
            key_id = None
            remaining = client.tables.get_alternate_keys(TABLE_SCHEMA_NAME)
            assert all(item.schema_name != KEY_SCHEMA_NAME for item in remaining)
            print("Alternate key deletion verified")
        finally:
            if key_id is not None:
                run_metadata_operation(
                    lambda: client.tables.delete_alternate_key(
                        TABLE_SCHEMA_NAME, key_id
                    ),
                    "cleaning up the alternate key",
                )
            if table_created:
                run_metadata_operation(
                    lambda: client.tables.delete(TABLE_SCHEMA_NAME),
                    "deleting the alternate-key table",
                )
                assert client.tables.get(TABLE_SCHEMA_NAME) is None
                print(f"Deleted custom table: {TABLE_SCHEMA_NAME}")


if __name__ == "__main__":
    main()