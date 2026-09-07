from PowerPlatform.Dataverse.models.filters import col

from work_data_common import create_client


def main() -> None:
    """Verify eager lists, streaming pages, and count-enabled requests."""
    with create_client() as client:
        total = 0
        for page_number, page in enumerate(
            client.query.builder("account")
            .select("accountid", "name", "revenue")
            .where(col("statecode") == 0)
            .order_by("name")
            .top(5)
            .page_size(2)
            .execute_pages(),
            start=1,
        ):
            total += len(page)
            print(f"QueryBuilder page {page_number}: {len(page)} records")
        assert total <= 5

        eager = client.records.list(
            "account",
            filter="statecode eq 0",
            select=["name", "revenue"],
            orderby=["name asc"],
            top=5,
            page_size=2,
            count=True,
        )
        assert len(eager) <= 5
        print(f"records.list returned {len(eager)} records")

        streamed = 0
        for page_number, page in enumerate(
            client.records.list_pages(
                "account",
                filter="statecode eq 0",
                select=["name"],
                orderby=["name asc"],
                top=5,
                page_size=2,
            ),
            start=1,
        ):
            streamed += len(page)
            print(f"records.list_pages page {page_number}: {len(page)} records")
        assert streamed <= 5

        counted = (
            client.query.builder("account")
            .select("accountid")
            .where(col("statecode") == 0)
            .top(5)
            .count()
            .execute()
        )
        print(f"Count-enabled QueryBuilder request returned {len(counted)} bounded rows")


if __name__ == "__main__":
    main()
