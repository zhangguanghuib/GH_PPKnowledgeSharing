import pandas as pd

from work_data_common import create_client, unique_name


def main() -> None:
    """Verify the SDK's pandas create, update, query, and delete wrappers."""
    created_ids = pd.Series(dtype="object")

    with create_client() as client:
        try:
            new_accounts = pd.DataFrame(
                [
                    {"name": unique_name("DataFrame A"), "telephone1": "555-0100"},
                    {"name": unique_name("DataFrame B"), "telephone1": "555-0200"},
                ]
            )
            created_ids = client.dataframe.create("account", new_accounts)
            new_accounts["accountid"] = created_ids
            assert len(created_ids) == 2
            print("Created DataFrame rows:")
            print(new_accounts[["accountid", "name"]].to_string(index=False))

            updates = new_accounts[["accountid"]].copy()
            updates["telephone1"] = ["555-0199", "555-0299"]
            client.dataframe.update("account", updates, id_column="accountid")

            queried = (
                client.query.builder("account")
                .select("accountid", "name", "telephone1")
                .top(5)
                .execute()
                .to_dataframe()
            )
            print(f"Read-only query returned {len(queried)} account rows")

            for row in updates.itertuples(index=False):
                account = client.records.retrieve(
                    "account", row.accountid, select=["telephone1"]
                )
                assert account is not None
                assert account["telephone1"] == row.telephone1
            print("DataFrame update verified")
        finally:
            if not created_ids.empty:
                client.dataframe.delete("account", created_ids)
                print(f"Cleaned up {len(created_ids)} accounts")


if __name__ == "__main__":
    main()
