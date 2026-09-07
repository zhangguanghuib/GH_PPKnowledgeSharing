from collections.abc import Iterable
from uuid import uuid4

from azure.identity import InteractiveBrowserCredential
from PowerPlatform.Dataverse.client import DataverseClient


DATAVERSE_URL = "https://org5efadec2.crm.dynamics.com"


def create_client() -> DataverseClient:
    """Create a Dataverse client using the Learn article's browser credential."""
    return DataverseClient(DATAVERSE_URL, InteractiveBrowserCredential())


def unique_name(prefix: str) -> str:
    """Return a recognizable name that won't collide with existing rows."""
    return f"Python SDK verification - {prefix} - {uuid4().hex[:8]}"


def delete_records(client: DataverseClient, table: str, record_ids: Iterable[str]) -> None:
    """Delete verification rows synchronously so each sample leaves no data behind."""
    ids = list(record_ids)
    if ids:
        client.records.delete(table, ids, use_bulk_delete=False)
