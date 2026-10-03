using System.Collections.Concurrent;
using System.Net;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.PowerPlatform.Dataverse.Client.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;

namespace XrmToolsLearning;

internal static class SampleOperations
{
    internal static Task ConnectionInfoAsync(ServiceClient service)
    {
        var whoAmI = (WhoAmIResponse)service.Execute(new WhoAmIRequest());
        var version = (RetrieveVersionResponse)service.Execute(new RetrieveVersionRequest());
        Entity user = service.Retrieve(
            "systemuser",
            whoAmI.UserId,
            new ColumnSet("fullname", "domainname"));

        Console.WriteLine($"Environment: {service.ConnectedOrgFriendlyName}");
        Console.WriteLine($"Unique name: {service.ConnectedOrgUniqueName}");
        Console.WriteLine($"Version:     {version.Version}");
        Console.WriteLine($"User:        {user.GetAttributeValue<string>("fullname")}");
        Console.WriteLine($"Domain:      {user.GetAttributeValue<string>("domainname")}");
        Console.WriteLine($"User ID:     {whoAmI.UserId}");
        return Task.CompletedTask;
    }

    internal static Task RetrieveAsync(ServiceClient service)
    {
        const string fetchXml =
            """
            <fetch returntotalrecordcount='true' count='10' page='1'>
              <entity name='account'>
                <attribute name='accountid' />
                <attribute name='name' />
                <order attribute='name' />
              </entity>
            </fetch>
            """;

        EntityCollection results = service.GetEntityDataByFetchSearchEC(fetchXml)
            ?? throw new InvalidOperationException(service.LastError);

        Console.WriteLine($"Account total record count: {results.TotalRecordCount}");
        foreach (Entity account in results.Entities)
        {
            Console.WriteLine(
                $"{account.Id}: {account.GetAttributeValue<string>("name") ?? "(no name)"}");
        }

        Entity? firstAccount = results.Entities.FirstOrDefault();
        if (firstAccount is null)
        {
            Console.WriteLine("No account is available for GetEntityDataById.");
            return Task.CompletedTask;
        }

        Dictionary<string, object> data = service.GetEntityDataById(
            "account",
            firstAccount.Id,
            ["name", "address1_city", "telephone1"]);
        Console.WriteLine("GetEntityDataById result:");
        foreach ((string key, object value) in data)
        {
            Console.WriteLine($"  {key}: {value}");
        }

        return Task.CompletedTask;
    }

    internal static Task CrudAsync(ServiceClient service)
    {
        Guid accountId = Guid.Empty;
        try
        {
            var createData = new Dictionary<string, DataverseDataTypeWrapper>
            {
                ["name"] = Text("XRM Tools Learning Account"),
                ["address1_city"] = Text("Redmond"),
                ["telephone1"] = Text("555-0160"),
            };

            accountId = service.CreateNewRecord("account", createData);
            EnsureId(accountId, service, "CreateNewRecord");
            Console.WriteLine($"Created account {accountId}.");

            var noteData = new Dictionary<string, DataverseDataTypeWrapper>
            {
                ["subject"] = Text("XRM Tools learning note"),
                ["notetext"] = Text("Created by the consolidated Microsoft Learn sample."),
            };
            Guid noteId = service.CreateAnnotation("account", accountId, noteData);
            EnsureId(noteId, service, "CreateAnnotation");
            Console.WriteLine($"Created note {noteId}.");

            PrintAccount(service, accountId, "Created account");

            var updateData = new Dictionary<string, DataverseDataTypeWrapper>
            {
                ["name"] = Text("Updated XRM Tools Learning Account"),
                ["address1_city"] = Text("Boston"),
                ["telephone1"] = Text("555-0161"),
            };
            EnsureSuccess(
                service.UpdateEntity("account", "accountid", accountId, updateData),
                service,
                "UpdateEntity");
            PrintAccount(service, accountId, "Updated account");

            EnsureSuccess(
                service.UpdateStateAndStatusForEntity("account", accountId, 1, 2),
                service,
                "Deactivate account");
            Console.WriteLine("Account state changed to Inactive.");

            EnsureSuccess(
                service.UpdateStateAndStatusForEntity("account", accountId, 0, 1),
                service,
                "Reactivate account");
            Console.WriteLine("Account state changed back to Active.");
        }
        finally
        {
            DeleteIfCreated(service, "account", ref accountId);
        }

        return Task.CompletedTask;
    }

    internal static Task MessagesAsync(ServiceClient service)
    {
        Guid accountId = Guid.Empty;
        try
        {
            var createRequest = new CreateRequest
            {
                Target = new Entity("account")
                {
                    ["name"] = "XRM Tools CreateRequest Account",
                },
            };
            var createResponse =
                (CreateResponse)service.ExecuteOrganizationRequest(createRequest);
            accountId = createResponse.id;
            EnsureId(accountId, service, "CreateRequest");
            Console.WriteLine($"CreateRequest created account {accountId}.");

            var retrieveRequest = new RetrieveMultipleRequest
            {
                Query = new QueryExpression("contact")
                {
                    ColumnSet = new ColumnSet("fullname"),
                    TopCount = 10,
                },
            };
            var retrieveResponse =
                (RetrieveMultipleResponse)service.ExecuteOrganizationRequest(retrieveRequest);

            Console.WriteLine("First contacts:");
            foreach (Entity contact in retrieveResponse.EntityCollection.Entities)
            {
                Console.WriteLine(
                    $"  {contact.GetAttributeValue<string>("fullname") ?? "(no name)"}");
            }
        }
        finally
        {
            DeleteIfCreated(service, "account", ref accountId);
        }

        return Task.CompletedTask;
    }

    internal static Task AssociationAsync(ServiceClient service)
    {
        Guid accountId = Guid.Empty;
        Guid leadId = Guid.Empty;
        const string relationship = "accountleads_association";

        try
        {
            accountId = service.Create(
                new Entity("account") { ["name"] = "XRM Tools Association Account" });
            leadId = service.Create(
                new Entity("lead")
                {
                    ["subject"] = "XRM Tools Association Lead",
                    ["lastname"] = "Learning Sample",
                });

            EnsureSuccess(
                service.CreateEntityAssociation(
                    "account",
                    accountId,
                    "lead",
                    leadId,
                    relationship),
                service,
                "CreateEntityAssociation");
            Console.WriteLine("Created account-to-lead association.");

            EnsureSuccess(
                service.DeleteEntityAssociation(
                    "account",
                    accountId,
                    "lead",
                    leadId,
                    relationship),
                service,
                "DeleteEntityAssociation");
            Console.WriteLine("Deleted account-to-lead association.");
        }
        finally
        {
            DeleteIfCreated(service, "lead", ref leadId);
            DeleteIfCreated(service, "account", ref accountId);
        }

        return Task.CompletedTask;
    }

    internal static Task WebApiAsync(ServiceClient service)
    {
        Guid accountId = Guid.NewGuid();
        bool created = false;
        try
        {
            using HttpResponseMessage createResponse = service.ExecuteWebRequest(
                HttpMethod.Post,
                "accounts",
                new JObject
                {
                    ["accountid"] = accountId,
                    ["name"] = "XRM Tools Web API Account",
                }.ToString(),
                CreateWebApiHeaders(),
                "application/json",
                CancellationToken.None);

            EnsureHttpSuccess(createResponse, "Web API create");
            created = true;
            Uri accountUri = createResponse.Headers.Location
                ?? new Uri(createResponse.Headers.GetValues("OData-EntityId").Single());
            Console.WriteLine($"Created account: {accountUri}");
        }
        finally
        {
            if (created)
            {
                using HttpResponseMessage deleteResponse = service.ExecuteWebRequest(
                    HttpMethod.Delete,
                    $"accounts({accountId})",
                    string.Empty,
                    CreateWebApiHeaders(),
                    "application/json",
                    CancellationToken.None);
                EnsureHttpSuccess(deleteResponse, $"Web API delete account {accountId}");
                Console.WriteLine("Deleted the Web API sample account.");
            }
        }

        return Task.CompletedTask;
    }

    // The SDK mutates custom headers, so each request needs its own dictionary.
    private static Dictionary<string, List<string>> CreateWebApiHeaders() => new()
    {
        ["Accept"] = ["application/json"],
        ["OData-MaxVersion"] = ["4.0"],
        ["OData-Version"] = ["4.0"],
        ["If-None-Match"] = ["null"],
    };

    internal static Task ParallelAsync(ServiceClient service)
    {
        List<Entity> accounts = Enumerable.Range(1, 10)
            .Select(
                number => new Entity("account")
                {
                    ["name"] = $"XRM Tools Parallel Account {number}",
                })
            .ToList();
        var created = new ConcurrentBag<EntityReference>();

        try
        {
            Parallel.ForEach(
                accounts,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = service.RecommendedDegreesOfParallelism,
                },
                () => service.Clone(),
                (account, _, localService) =>
                {
                    Guid id = localService.Create(account);
                    created.Add(new EntityReference(account.LogicalName, id));
                    return localService;
                },
                localService => localService.Dispose());

            Console.WriteLine($"Created {created.Count} accounts in parallel.");
        }
        finally
        {
            Parallel.ForEach(
                created,
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = service.RecommendedDegreesOfParallelism,
                },
                () => service.Clone(),
                (entityReference, _, localService) =>
                {
                    localService.Delete(entityReference.LogicalName, entityReference.Id);
                    return localService;
                },
                localService => localService.Dispose());
            Console.WriteLine($"Deleted {created.Count} accounts in parallel.");
        }

        return Task.CompletedTask;
    }

    private static DataverseDataTypeWrapper Text(string value) =>
        new(value, DataverseFieldType.String);

    private static void PrintAccount(ServiceClient service, Guid accountId, string heading)
    {
        Dictionary<string, object> data = service.GetEntityDataById(
            "account",
            accountId,
            ["name", "address1_city", "telephone1", "statecode", "statuscode"]);

        Console.WriteLine($"{heading}:");
        foreach ((string key, object value) in data)
        {
            Console.WriteLine($"  {key}: {value}");
        }
    }

    private static void DeleteIfCreated(
        ServiceClient service,
        string logicalName,
        ref Guid recordId)
    {
        if (recordId == Guid.Empty)
        {
            return;
        }

        Guid idToDelete = recordId;
        recordId = Guid.Empty;
        EnsureSuccess(
            service.DeleteEntity(logicalName, idToDelete),
            service,
            $"Delete {logicalName} {idToDelete}");
        Console.WriteLine($"Deleted temporary {logicalName} {idToDelete}.");
    }

    private static void EnsureId(Guid id, ServiceClient service, string operation)
    {
        if (id == Guid.Empty)
        {
            throw new InvalidOperationException($"{operation} failed: {service.LastError}");
        }
    }

    private static void EnsureSuccess(bool success, ServiceClient service, string operation)
    {
        if (!success)
        {
            throw new InvalidOperationException($"{operation} failed: {service.LastError}");
        }
    }

    private static void EnsureHttpSuccess(HttpResponseMessage response, string operation)
    {
        if (!response.IsSuccessStatusCode)
        {
            string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            throw new HttpRequestException(
                $"{operation} failed with {(int)response.StatusCode} ({response.StatusCode}): {responseBody}",
                inner: null,
                response.StatusCode);
        }
    }
}
