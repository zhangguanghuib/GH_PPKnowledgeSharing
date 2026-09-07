from pathlib import Path
from tempfile import TemporaryDirectory

from azure.identity import InteractiveBrowserCredential
from PowerPlatform.Dataverse.client import DataverseClient
from PowerPlatform.Dataverse.core import DataverseConfig, LogConfig

from work_data_common import DATAVERSE_URL


def main() -> None:
    credential = InteractiveBrowserCredential()

    try:
        with TemporaryDirectory(prefix="dataverse_http_logs_") as log_folder:
            log_config = LogConfig(
                log_folder=log_folder,
                log_file_prefix="crm_debug",
                max_body_bytes=4096,
            )
            config = DataverseConfig(log_config=log_config)

            with DataverseClient(DATAVERSE_URL, credential, config=config) as client:
                accounts = client.records.list(
                    "account",
                    select=["accountid", "name"],
                    top=1,
                )

            log_files = list(Path(log_folder).glob("crm_debug_*.log"))
            assert len(log_files) == 1
            contents = log_files[0].read_text(encoding="utf-8")
            assert ">>> REQUEST  GET" in contents
            assert "<<< RESPONSE 200 GET" in contents
            assert "Authorization: [REDACTED]" in contents
            assert "Bearer " not in contents
            assert len(accounts) <= 1
            print(f"Diagnostics file verified: {log_files[0].name}")
            print("Authorization redaction verified; temporary log removed")
    finally:
        credential.close()


if __name__ == "__main__":
    main()