from PowerPlatform.Dataverse.models.query_builder import ExpandOption

from work_data_common import create_client


FORMATTED_VALUE = "OData.Community.Display.V1.FormattedValue"


def main() -> None:
    """Verify formatted annotations, metadata discovery, and nested expand."""
    with create_client() as client:
        formatted = (
            client.query.builder("account")
            .select("name", "statecode", "revenue")
            .include_formatted_values()
            .top(5)
            .execute()
        )
        for record in formatted:
            label = record.get(f"statecode@{FORMATTED_VALUE}")
            print(f"{record.get('name')}: {label}")
        print(f"Formatted-value query returned {len(formatted)} rows")

        listed = client.records.list(
            "account",
            select=["accountid", "name", "statecode"],
            include_annotations=FORMATTED_VALUE,
            top=5,
        )
        print(f"Annotated records.list returned {len(listed)} rows")

        first_account = listed.first()
        if first_account is not None:
            retrieved = client.records.retrieve(
                "account",
                first_account["accountid"],
                select=["name", "statuscode"],
                include_annotations=FORMATTED_VALUE,
            )
            assert retrieved is not None
            print(f"Retrieved status: {retrieved.get(f'statuscode@{FORMATTED_VALUE}')}")

        accounts_with_tasks = (
            client.query.builder("account")
            .select("name")
            .expand(
                ExpandOption("Account_Tasks")
                .select("subject", "createdon")
                .filter("contains(subject,'Task')")
                .order_by("createdon", descending=True)
                .top(5)
            )
            .top(5)
            .execute()
        )
        print(f"Nested Account_Tasks expansion returned {len(accounts_with_tasks)} accounts")

        navigation = client.query.odata_expands("contact")
        account_navigation = next(
            (
                item["nav_property"]
                for item in navigation
                if item["target_table"].lower() == "account"
            ),
            "parentcustomerid_account",
        )
        contacts = (
            client.query.builder("contact")
            .select("fullname")
            .expand(ExpandOption(account_navigation).select("name"))
            .top(5)
            .execute()
        )
        for contact in contacts:
            account = contact.get(account_navigation) or {}
            print(f"{contact.get('fullname')} -> {account.get('name', '<none>')}")
        print(f"Expand query returned {len(contacts)} contacts")


if __name__ == "__main__":
    main()
