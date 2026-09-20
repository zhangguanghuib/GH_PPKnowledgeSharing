using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

internal static class BulkOperations
{
    public static void ShowAvailability(IOrganizationService service, string tableLogicalName)
    {
        bool createAvailable = IsMessageAvailable(service, tableLogicalName, "CreateMultiple");
        bool updateAvailable = IsMessageAvailable(service, tableLogicalName, "UpdateMultiple");
        Console.WriteLine($"Table: {tableLogicalName}");
        Console.WriteLine($"CreateMultiple: {createAvailable}");
        Console.WriteLine($"UpdateMultiple: {updateAvailable}");
        Console.WriteLine($"UpsertMultiple: {createAvailable && updateAvailable}");
        Console.WriteLine("DeleteMultiple: elastic tables only");
    }

    public static void RunCreateUpdateDemo(IOrganizationService service, int recordCount)
    {
        if (recordCount is < 1 or > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(recordCount), "Record count must be 1-1000.");
        }
        if (!IsMessageAvailable(service, "account", "CreateMultiple") ||
            !IsMessageAvailable(service, "account", "UpdateMultiple"))
        {
            throw new InvalidOperationException("The account table does not support both CreateMultiple and UpdateMultiple.");
        }

        string runId = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmssfff");
        List<Entity> accounts = Enumerable.Range(1, recordCount)
            .Select(index => new Entity("account")
            {
                ["name"] = $"Bulk operation sample {runId}-{index:D4}"
            })
            .ToList();
        Guid[] createdIds = [];

        try
        {
            createdIds = CreateMultiple(service, accounts);
            Console.WriteLine($"CreateMultiple created {createdIds.Length} account records.");

            List<Entity> updates = createdIds
                .Select(id => new Entity("account", id)
                {
                    ["description"] = "Updated with UpdateMultiple"
                })
                .ToList();
            UpdateMultiple(service, updates);
            Console.WriteLine($"UpdateMultiple updated {updates.Count} account records.");
        }
        finally
        {
            foreach (Guid id in createdIds)
            {
                service.Delete("account", id);
            }
            if (createdIds.Length > 0)
            {
                Console.WriteLine($"Cleanup deleted {createdIds.Length} sample account records.");
            }
        }
    }

    public static void RunUpsertDemo(
        IOrganizationService service,
        string tableLogicalName,
        string alternateKeyColumn,
        string descriptionColumn)
    {
        if (!IsMessageAvailable(service, tableLogicalName, "CreateMultiple") ||
            !IsMessageAvailable(service, tableLogicalName, "UpdateMultiple"))
        {
            throw new InvalidOperationException($"The {tableLogicalName} table does not support UpsertMultiple.");
        }

        string runId = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmssfff");
        string updateKey = $"Bulk update {runId}";
        string createKey = $"Bulk create {runId}";
        Guid seedId = service.Create(new Entity(tableLogicalName)
        {
            [alternateKeyColumn] = updateKey,
            [descriptionColumn] = "A record to update using UpsertMultiple"
        });
        HashSet<Guid> cleanupIds = [seedId];

        try
        {
            Entity toUpdate = new(tableLogicalName, alternateKeyColumn, updateKey)
            {
                [descriptionColumn] = "Updated using UpsertMultiple"
            };
            Entity toCreate = new(tableLogicalName, alternateKeyColumn, createKey)
            {
                [descriptionColumn] = "Created using UpsertMultiple"
            };

            UpsertResponse[] results = UpsertMultiple(service, [toUpdate, toCreate]);
            foreach (UpsertResponse result in results)
            {
                if (result.Target.Id != Guid.Empty)
                {
                    cleanupIds.Add(result.Target.Id);
                }
                Console.WriteLine($"Record {(result.RecordCreated ? "created" : "updated")}: {result.Target.Id}");
            }
        }
        finally
        {
            foreach (Guid id in cleanupIds)
            {
                service.Delete(tableLogicalName, id);
            }
            Console.WriteLine($"Cleanup deleted {cleanupIds.Count} sample records.");
        }
    }

    public static Guid[] CreateMultiple(IOrganizationService service, List<Entity> recordsToCreate)
    {
        EntityCollection entities = CreateEntityCollection(recordsToCreate);
        CreateMultipleResponse response = (CreateMultipleResponse)service.Execute(
            new CreateMultipleRequest { Targets = entities });
        return response.Ids;
    }

    public static void UpdateMultiple(IOrganizationService service, List<Entity> recordsToUpdate)
    {
        EntityCollection entities = CreateEntityCollection(recordsToUpdate);
        service.Execute(new UpdateMultipleRequest { Targets = entities });
    }

    public static UpsertResponse[] UpsertMultiple(IOrganizationService service, List<Entity> recordsToUpsert)
    {
        EntityCollection entities = CreateEntityCollection(recordsToUpsert);
        UpsertMultipleResponse response = (UpsertMultipleResponse)service.Execute(
            new UpsertMultipleRequest { Targets = entities });
        return response.Results.ToArray();
    }

    public static void DeleteMultiple(
        IOrganizationService service,
        string tableLogicalName,
        string idColumnLogicalName,
        IEnumerable<(Guid Id, string PartitionId)> recordsToDelete)
    {
        List<EntityReference> references = recordsToDelete
            .Select(record => new EntityReference(
                tableLogicalName,
                new KeyAttributeCollection
                {
                    [idColumnLogicalName] = record.Id,
                    ["partitionid"] = record.PartitionId
                }))
            .ToList();

        if (references.Count == 0)
        {
            throw new ArgumentException("At least one record must be supplied.", nameof(recordsToDelete));
        }

        OrganizationRequest request = new("DeleteMultiple")
        {
            Parameters = { ["Targets"] = new EntityReferenceCollection(references) }
        };
        service.Execute(request);
    }

    public static void RunDeleteMultiple(
        IOrganizationService service,
        string tableLogicalName,
        string idColumnLogicalName,
        string partitionId,
        IEnumerable<Guid> recordIds)
    {
        List<(Guid Id, string PartitionId)> records = recordIds
            .Select(id => (id, partitionId))
            .ToList();
        DeleteMultiple(service, tableLogicalName, idColumnLogicalName, records);
        Console.WriteLine($"DeleteMultiple submitted {records.Count} elastic-table records.");
    }

    public static bool IsMessageAvailable(
        IOrganizationService service,
        string entityLogicalName,
        string messageName)
    {
        QueryExpression query = new("sdkmessagefilter")
        {
            ColumnSet = new ColumnSet("sdkmessagefilterid"),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                Conditions =
                {
                    new ConditionExpression(
                        "primaryobjecttypecode",
                        ConditionOperator.Equal,
                        entityLogicalName)
                }
            },
            LinkEntities =
            {
                new LinkEntity(
                    "sdkmessagefilter",
                    "sdkmessage",
                    "sdkmessageid",
                    "sdkmessageid",
                    JoinOperator.Inner)
                {
                    LinkCriteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression("name", ConditionOperator.Equal, messageName)
                        }
                    }
                }
            }
        };

        return service.RetrieveMultiple(query).Entities.Count == 1;
    }

    private static EntityCollection CreateEntityCollection(List<Entity> records)
    {
        if (records.Count == 0)
        {
            throw new ArgumentException("At least one record must be supplied.", nameof(records));
        }

        string tableLogicalName = records[0].LogicalName;
        if (records.Any(record => record.LogicalName != tableLogicalName))
        {
            throw new ArgumentException("All records in a bulk request must use the same table.", nameof(records));
        }

        return new EntityCollection(records) { EntityName = tableLogicalName };
    }
}