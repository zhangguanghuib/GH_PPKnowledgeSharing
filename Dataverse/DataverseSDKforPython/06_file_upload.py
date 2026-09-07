import os
from pathlib import Path

from work_data_common import create_client


def main() -> None:
    """Upload a file when explicit record, column, and file inputs are supplied."""
    account_id = os.getenv("DATAVERSE_TEST_ACCOUNT_ID")
    column_name = os.getenv("DATAVERSE_TEST_FILE_COLUMN")
    file_path_value = os.getenv("DATAVERSE_TEST_FILE_PATH")

    if not all((account_id, column_name, file_path_value)):
        print(
            "SKIPPED: Set DATAVERSE_TEST_ACCOUNT_ID, DATAVERSE_TEST_FILE_COLUMN, "
            "and DATAVERSE_TEST_FILE_PATH to run the upload example."
        )
        return

    file_path = Path(file_path_value)
    if not file_path.is_file():
        raise FileNotFoundError(file_path)

    with create_client() as client:
        client.files.upload(
            "account",
            account_id,
            column_name,
            str(file_path),
            mode="auto",
        )
        print(f"Uploaded {file_path.name} to account {account_id}, column {column_name}")


if __name__ == "__main__":
    main()