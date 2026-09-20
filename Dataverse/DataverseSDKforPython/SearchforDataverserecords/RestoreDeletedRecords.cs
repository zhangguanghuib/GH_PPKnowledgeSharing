using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

internal static class RestoreDeletedRecords
{
    public static EntityCollection GetDeletedAccountRecordsFetchXml(
        IOrganizationService service,
        int topCount = 3)
    {
        ValidateTopCount(topCount);
        string fetchXml = $"""
            <fetch top='{topCount}' datasource='bin'>
              <entity name='account'>
                <attribute name='accountid' />
                <attribute name='name' />
              </entity>
            </fetch>
            """;
        return service.RetrieveMultiple(new FetchExpression(fetchXml));
    }

    public static EntityCollection GetDeletedAccountRecordsQueryExpression(
        IOrganizationService service,
        int topCount = 3)
    {
        ValidateTopCount(topCount);
        QueryExpression query = new("account")
        {
            ColumnSet = new ColumnSet("accountid", "name"),
            DataSource = "bin",
            TopCount = topCount
        };
        return service.RetrieveMultiple(query);
    }

    public static void PrintDeletedAccounts(EntityCollection records)
    {
        Console.WriteLine($"Deleted account records found: {records.Entities.Count}");
        foreach (Entity account in records.Entities)
        {
            Console.WriteLine($"{account.Id} | {account.GetAttributeValue<string>("name") ?? "(no name)"}");
        }
    }

    public static Guid RestoreAccountRecordLateBound(
        IOrganizationService service,
        Guid accountId,
        string? restoredName = null)
    {
        Entity accountToRestore = new("account", accountId);
        if (!string.IsNullOrWhiteSpace(restoredName))
        {
            accountToRestore["name"] = restoredName;
        }

        OrganizationRequest request = new("Restore")
        {
            Parameters = { ["Target"] = accountToRestore }
        };
        OrganizationResponse response = service.Execute(request);
        return (Guid)response.Results["id"];
    }

    public static EntityCollection GetEnabledTables(IOrganizationService service)
    {
        const string fetchXml = """
            <fetch>
              <entity name='recyclebinconfig'>
                <attribute name='name' />
                <attribute name='cleanupintervalindays' />
                <filter type='and'>
                  <condition attribute='statecode' operator='eq' value='0' />
                  <condition attribute='isreadyforrecyclebin' operator='eq' value='1' />
                </filter>
                <link-entity name='entity' from='entityid' to='extensionofrecordid' link-type='inner' alias='entity'>
                  <attribute name='logicalname' />
                  <order attribute='logicalname' />
                </link-entity>
              </entity>
            </fetch>
            """;
        return service.RetrieveMultiple(new FetchExpression(fetchXml));
    }

    public static void PrintEnabledTables(EntityCollection records)
    {
        Console.WriteLine($"Tables enabled for deleted record keeping: {records.Entities.Count}");
        foreach (Entity record in records.Entities)
        {
            string logicalName = record.GetAttributeValue<AliasedValue>("entity.logicalname")?.Value?.ToString()
                ?? record.GetAttributeValue<string>("name")
                ?? "(unknown)";
            int? retentionDays = record.GetAttributeValue<int?>("cleanupintervalindays");
            string retention = retentionDays is null or -1 ? "organization default" : $"{retentionDays} days";
            Console.WriteLine($"{logicalName} | retention: {retention}");
        }
    }

    public static void SetCleanupIntervalInDays(
        IOrganizationService service,
        Guid entityId,
        string tableLogicalName,
        int cleanupIntervalInDays)
    {
        if (cleanupIntervalInDays is < 1 or > 30)
        {
            throw new ArgumentOutOfRangeException(nameof(cleanupIntervalInDays), "Retention must be 1-30 days.");
        }

        QueryExpression query = new("recyclebinconfig")
        {
            ColumnSet = new ColumnSet("recyclebinconfigid"),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                Conditions =
                {
                    new ConditionExpression("extensionofrecordid", ConditionOperator.Equal, entityId)
                }
            }
        };
        EntityCollection records = service.RetrieveMultiple(query);
        if (records.Entities.Count != 1)
        {
            throw new InvalidOperationException($"Deleted record keeping configuration for table '{tableLogicalName}' was not found.");
        }

        service.Update(new Entity("recyclebinconfig", records.Entities[0].Id)
        {
            ["cleanupintervalindays"] = cleanupIntervalInDays
        });
        Console.WriteLine($"Set {tableLogicalName} deleted-record retention to {cleanupIntervalInDays} days.");
    }

    private static void ValidateTopCount(int topCount)
    {
        if (topCount is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(topCount), "Top count must be 1-100.");
        }
    }
}