import requests
from azure.identity import InteractiveBrowserCredential
from PowerPlatform.Dataverse.client import DataverseClient
from PowerPlatform.Dataverse.core import DataverseConfig
from PowerPlatform.Dataverse.core.errors import DataverseError

from work_data_common import DATAVERSE_URL


def main() -> None:
    config = DataverseConfig(
        http_timeout=120,
        http_retries=3,
        http_backoff=1.0,
    )
    credential = InteractiveBrowserCredential()

    try:
        with DataverseClient(DATAVERSE_URL, credential, config=config) as client:
            try:
                accounts = client.records.list(
                    "account",
                    select=["accountid", "name"],
                    top=1,
                )
            except requests.exceptions.Timeout:
                print(
                    "Request timed out. Consider increasing http_timeout "
                    "in DataverseConfig."
                )
                raise
            except requests.exceptions.RequestException as error:
                print(f"Network error: {error}")
                raise
            except DataverseError as error:
                print(f"SDK error: {error.message}")
                raise
    finally:
        credential.close()

    assert len(accounts) <= 1
    assert config.http_timeout == 120
    assert config.http_retries == 3
    assert config.http_backoff == 1.0
    print(f"Configured client request succeeded with {len(accounts)} account")


if __name__ == "__main__":
    main()
