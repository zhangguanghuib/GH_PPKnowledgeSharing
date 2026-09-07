import time
from collections.abc import Callable
from typing import Any, Protocol

from PowerPlatform.Dataverse.core.errors import HttpError


class RecordOperationsLike(Protocol):
    def create(self, table: str, data: dict[str, Any]) -> str: ...


class ClientLike(Protocol):
    records: RecordOperationsLike


def create_with_retry(
    client: ClientLike,
    table: str,
    data: dict[str, Any],
    max_retries: int = 3,
    sleep: Callable[[float], None] = time.sleep,
) -> str:
    """Retry transient Dataverse HTTP responses with exponential backoff."""
    for attempt in range(max_retries):
        try:
            return client.records.create(table, data)
        except HttpError as error:
            if error.is_transient and attempt < max_retries - 1:
                wait = error.details.get("retry_after") or 2**attempt
                print(
                    f"Transient error (HTTP {error.status_code}). "
                    f"Retrying in {wait}s..."
                )
                sleep(float(wait))
            else:
                raise

    raise AssertionError("Retry loop exited without a result")


class SimulatedRecords:
    """Produce two throttling responses before succeeding."""

    def __init__(self) -> None:
        self.attempts = 0

    def create(self, table: str, data: dict[str, Any]) -> str:
        self.attempts += 1
        if self.attempts < 3:
            raise HttpError(
                "Too many requests",
                status_code=429,
                is_transient=True,
                retry_after=self.attempts,
            )
        return "00000000-0000-0000-0000-000000000001"


class SimulatedClient:
    def __init__(self) -> None:
        self.records = SimulatedRecords()


def main() -> None:
    client = SimulatedClient()
    waits: list[float] = []
    record_id = create_with_retry(
        client,
        "account",
        {"name": "Retry sample"},
        sleep=waits.append,
    )

    assert record_id == "00000000-0000-0000-0000-000000000001"
    assert client.records.attempts == 3
    assert waits == [1.0, 2.0]
    print("Transient retry behavior verified without changing Dataverse data")


if __name__ == "__main__":
    main()
