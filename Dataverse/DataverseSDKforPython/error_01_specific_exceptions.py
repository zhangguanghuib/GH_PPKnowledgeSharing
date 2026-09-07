from collections.abc import Callable

import requests
from PowerPlatform.Dataverse.client import DataverseClient
from PowerPlatform.Dataverse.core.errors import (
    DataverseError,
    HttpError,
    MetadataError,
    ValidationError,
)

from work_data_common import create_client


def run_and_report(operation: Callable[[], object]) -> type[DataverseError]:
    """Run an operation and report the article's specific exception fields."""
    try:
        operation()
    except ValidationError as error:
        print(f"Validation error: {error.message} (subcode={error.subcode})")
        return type(error)
    except MetadataError as error:
        print(f"Metadata error: {error.message} (subcode={error.subcode})")
        return type(error)
    except HttpError as error:
        print(f"HTTP {error.status_code}: {error.message}")
        print(f"Service request id: {error.details.get('service_request_id')}")
        if error.is_transient:
            print(f"Transient - retry after {error.details.get('retry_after')}s")
        return type(error)
    except requests.exceptions.Timeout as error:
        print(f"Request timed out: {error}")
        raise
    except DataverseError as error:
        print(f"Dataverse error [{error.code}]: {error.message}")
        return type(error)

    raise AssertionError("The operation unexpectedly succeeded")


def add_column_to_unknown_table(client: DataverseClient) -> None:
    """Fail metadata resolution before any customization request is sent."""
    client.tables.add_columns(
        "invalid_table_for_error_sample",
        {"new_ErrorSample": "string"},
    )


def main() -> None:
    with create_client() as client:
        validation_type = run_and_report(lambda: client.query.sql(""))
        metadata_type = run_and_report(lambda: add_column_to_unknown_table(client))
        http_type = run_and_report(
            lambda: client.records.delete(
                "account", "00000000-0000-0000-0000-000000000000"
            )
        )

    assert validation_type is ValidationError
    assert metadata_type is MetadataError
    assert http_type is HttpError
    print("Specific exception handling verified")


if __name__ == "__main__":
    main()