from PowerPlatform.Dataverse.models.filters import col

from work_data_common import create_client


def main() -> None:
    """Run the article's recommended fluent QueryBuilder patterns."""
    with create_client() as client:
        results = (
            client.query.builder("account")
            .select("accountid", "name", "revenue")
            .where(col("statecode") == 0)
            .order_by("revenue", descending=True)
            .top(10)
            .page_size(5)
            .execute()
        )

        print(f"QueryBuilder returned {len(results)} active accounts")
        for record in results:
            print(f"  {record.get('name')}: {record.get('revenue')}")

        dataframe = (
            client.query.builder("account")
            .select("name", "telephone1")
            .where(col("statecode") == 0)
            .top(10)
            .execute()
            .to_dataframe()
        )
        assert len(dataframe) <= 10
        print(f"DataFrame conversion returned {len(dataframe)} rows")


if __name__ == "__main__":
    main()
