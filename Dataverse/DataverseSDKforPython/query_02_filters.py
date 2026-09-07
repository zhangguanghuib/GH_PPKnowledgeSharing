from PowerPlatform.Dataverse.models.filters import col, raw

from work_data_common import create_client


def main() -> None:
    """Verify comparison, string, set, range, negation, and raw filters."""
    with create_client() as client:
        filter_queries = {
            "comparison": (
                client.query.builder("account")
                .select("name", "revenue")
                .where(col("statecode") == 0)
                .where(col("revenue") > 1_000_000)
                .top(5)
            ),
            "contains": (
                client.query.builder("account")
                .select("name")
                .where(col("name").contains("a"))
                .top(5)
            ),
            "startswith": (
                client.query.builder("account")
                .select("name")
                .where(col("name").startswith("A"))
                .top(5)
            ),
            "endswith": (
                client.query.builder("account")
                .select("name")
                .where(col("name").endswith("s"))
                .top(5)
            ),
            "string-like": (
                client.query.builder("account")
                .select("name")
                .where(col("name").like("A%"))
                .top(5)
            ),
            "composed-or-and": (
                client.query.builder("account")
                .select("name", "revenue")
                .where(
                    ((col("statecode") == 0) | (col("statecode") == 1))
                    & (col("revenue") >= 0)
                )
                .top(5)
            ),
            "set-membership": (
                client.query.builder("account")
                .select("name", "statecode")
                .where(col("statecode").in_([0, 1]))
                .top(5)
            ),
            "not-in": (
                client.query.builder("account")
                .select("name", "statecode")
                .where(col("statecode").not_in([2, 3]))
                .top(5)
            ),
            "between": (
                client.query.builder("account")
                .select("name", "revenue")
                .where(col("revenue").between(0, 50_000_000))
                .top(5)
            ),
            "is-null": (
                client.query.builder("account")
                .select("name", "telephone1")
                .where(col("telephone1").is_null())
                .top(5)
            ),
            "range-and-not-null": (
                client.query.builder("account")
                .select("name", "revenue")
                .where(col("name").is_not_null())
                .where(col("revenue").not_between(-1, 0))
                .top(5)
            ),
            "raw-odata": (
                client.query.builder("account")
                .select("name", "createdon")
                .where(raw("Microsoft.Dynamics.CRM.Today(PropertyName='createdon')"))
                .top(5)
            ),
        }

        for name, query in filter_queries.items():
            print(f"{name} filter: {query.build().get('filter')}")
            results = query.execute()
            print(f"  Returned {len(results)} rows")

        negated = (
            client.query.builder("account")
            .select("name", "statecode")
            .where(~(col("statecode") == 1))
            .top(5)
            .execute()
        )
        print(f"explicit-negation returned {len(negated)} rows")


if __name__ == "__main__":
    main()
