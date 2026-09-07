from work_data_common import create_client


def main() -> None:
    """Verify SQL, joins, aggregates, DataFrames, and column discovery."""
    with create_client() as client:
        basic = client.query.sql(
            "SELECT TOP 10 accountid, name FROM account "
            "WHERE statecode = 0 ORDER BY name"
        )
        print(f"Basic SQL query returned {len(basic)} rows")

        aggregate = client.query.sql(
            "SELECT a.name, COUNT(c.contactid) AS cnt "
            "FROM account a "
            "LEFT JOIN contact c ON a.accountid = c.parentcustomerid "
            "GROUP BY a.name ORDER BY a.name"
        )
        print(f"JOIN and aggregate query returned {len(aggregate)} rows")

        dataframe = client.dataframe.sql(
            "SELECT TOP 10 name, revenue FROM account ORDER BY revenue DESC"
        )
        assert len(dataframe) <= 10
        print(f"SQL DataFrame returned {len(dataframe)} rows")

        columns = client.query.sql_columns("account")
        column_names = [column["name"] for column in columns]
        assert "accountid" in column_names
        assert "name" in column_names
        print(f"Discovered {len(column_names)} SQL-usable account columns")

        discovered = client.dataframe.sql(
            f"SELECT TOP 10 {', '.join(column_names[:5])} FROM account"
        )
        assert len(discovered) <= 10
        print(f"Metadata-built SQL query returned {len(discovered)} rows")


if __name__ == "__main__":
    main()