import time
from collections.abc import Callable
from typing import TypeVar


Result = TypeVar("Result")

TRANSIENT_METADATA_MESSAGES = (
    "staged metadata",
    "previous [entitycustomization] running",
    "sql deadlock exception",
)


def run_metadata_operation(
    operation: Callable[[], Result], description: str
) -> Result:
    """Run a metadata write with bounded retries for Dataverse processing locks."""
    for attempt in range(1, 25):
        try:
            return operation()
        except Exception as error:
            message = str(error).lower()
            is_transient = any(
                marker in message for marker in TRANSIENT_METADATA_MESSAGES
            )
            if not is_transient or attempt == 24:
                raise
            print(f"Dataverse is processing metadata; {description} retry {attempt}/24")
            time.sleep(5)

    raise RuntimeError(f"Unable to finish {description}")
