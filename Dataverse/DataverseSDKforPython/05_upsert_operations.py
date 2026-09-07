import os

from PowerPlatform.Dataverse.models.upsert import UpsertItem

from work_data_common import create_client, unique_name


def main() -> None:
    """Run an alternate-key upsert when a configured test key is supplied."""
    account_number = os.getenv("DATAVERSE_TEST_ACCOUNTNUMBER")
    if not account_number:
        print(
            "SKIPPED: Set DATAVERSE_TEST_ACCOUNTNUMBER to a disposable value only "
            "after accountnumber is configured as an alternate key."
        )
        return

    with create_client() as client:
        client.records.upsert(
            "account",
            [
                UpsertItem(
                    alternate_key={"accountnumber": account_number},
                    record={"name": unique_name("Upsert")},
                )
            ],
        )

        matches = client.records.list(
            "account",
            select=["accountid", "name", "accountnumber"],
            filter=f"accountnumber eq '{account_number.replace("'", "''")}'",
            top=1,
        )
        assert len(matches) == 1
        print(f"Upsert verified: {matches[0]['accountid']}")
        print("The upserted row was retained because the supplied alternate key may be shared.")


if __name__ == "__main__":
    main()
