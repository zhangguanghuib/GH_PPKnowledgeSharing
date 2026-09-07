from work_data_common import create_client


FETCH_XML = """
<fetch top="10">
  <entity name="account">
    <attribute name="accountid" />
    <attribute name="name" />
    <attribute name="revenue" />
    <filter>
      <condition attribute="statecode" operator="eq" value="0" />
    </filter>
    <order attribute="name" />
  </entity>
</fetch>
"""


def main() -> None:
    """Verify eager and streaming execution of an inert FetchXML query."""
    with create_client() as client:
        query = client.query.fetchxml(FETCH_XML)
        result = query.execute()
        dataframe = result.to_dataframe()
        assert len(result) <= 10
        print(f"FetchXML eager execution returned {len(result)} rows")
        print(f"FetchXML DataFrame contains {len(dataframe)} rows")

        streamed = 0
        for page_number, page in enumerate(
            client.query.fetchxml(FETCH_XML).execute_pages(), start=1
        ):
            streamed += len(page)
            print(f"FetchXML page {page_number}: {len(page)} rows")
        assert streamed == len(result)


if __name__ == "__main__":
    main()
