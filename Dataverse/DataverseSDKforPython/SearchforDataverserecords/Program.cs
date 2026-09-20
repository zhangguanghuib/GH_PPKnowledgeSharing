using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Identity.Client;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

internal static class Program
{
    private const string DefaultEnvironmentUrl = "https://org5efadec2.crm.dynamics.com/";
    private const string DefaultUserName = "guazha@dynamicsFTEGCR.onmicrosoft.com";
    private const string SampleClientId = "51f81489-12ee-4a9e-aaae-a2591f45987d";
    private static readonly JsonSerializerOptions PrettyJson = new() { WriteIndented = true };

    private static async Task<int> Main(string[] args)
    {
        string command = args.FirstOrDefault()?.ToLowerInvariant() ?? "query";
        if (command is "help" or "--help" or "-h")
        {
            ShowHelp();
            return 0;
        }

        try
        {
            string environmentUrl = (Environment.GetEnvironmentVariable("DATAVERSE_URL") ?? DefaultEnvironmentUrl).TrimEnd('/');
            string accessToken = await AcquireTokenAsync(environmentUrl);
            using HttpClient client = CreateHttpClient(accessToken);

            switch (command)
            {
                case "query":
                    await SearchQueryAsync(client, environmentUrl, args.ElementAtOrDefault(1) ?? "Contoso");
                    break;
                case "suggest":
                    await SuggestAsync(client, environmentUrl, args.ElementAtOrDefault(1) ?? "Cont");
                    break;
                case "autocomplete":
                    await AutocompleteAsync(client, environmentUrl, args.ElementAtOrDefault(1) ?? "Con");
                    break;
                case "status":
                    await GetSearchAsync(client, environmentUrl, "status", "Dataverse Search status");
                    break;
                case "statistics":
                    await GetSearchAsync(client, environmentUrl, "statistics", "Dataverse Search statistics");
                    break;
                case "all":
                    await RunAllReadOnlyAsync(client, environmentUrl, args.ElementAtOrDefault(1) ?? "Contoso");
                    break;
                case "enabled":
                    using (ServiceClient service = CreateServiceClient(environmentUrl, accessToken))
                    {
                        Console.WriteLine($"Dataverse Search enabled: {IsSearchEnabled(service)}");
                    }
                    break;
                case "table-settings":
                    using (ServiceClient service = CreateServiceClient(environmentUrl, accessToken))
                    {
                        ShowTableSettings(service, args.ElementAtOrDefault(1) ?? "account");
                    }
                    break;
                case "legacy":
                    await LegacyQueryAsync(client, environmentUrl, args.ElementAtOrDefault(1) ?? "Contoso");
                    break;
                case "analyzer-list":
                    using (ServiceClient service = CreateServiceClient(environmentUrl, accessToken))
                    {
                        ListAnalyzerSettings(service, args.ElementAtOrDefault(1), args.ElementAtOrDefault(2));
                    }
                    break;
                case "analyzer-set":
                    using (ServiceClient service = CreateServiceClient(environmentUrl, accessToken))
                    {
                        SetAnalyzer(service, args);
                    }
                    break;
                case "bulk-availability":
                    using (ServiceClient service = CreateServiceClient(environmentUrl, accessToken))
                    {
                        BulkOperations.ShowAvailability(service, args.ElementAtOrDefault(1) ?? "account");
                    }
                    break;
                case "bulk-create-update":
                    RequireConfirmation(args, "bulk-create-update [count] --confirm");
                    using (ServiceClient service = CreateServiceClient(environmentUrl, accessToken))
                    {
                        int count = int.TryParse(args.ElementAtOrDefault(1), out int requestedCount) ? requestedCount : 3;
                        BulkOperations.RunCreateUpdateDemo(service, count);
                    }
                    break;
                case "bulk-upsert":
                    RequireConfirmation(args, "bulk-upsert <table> <alternate-key-column> <description-column> --confirm");
                    if (args.Length < 5)
                    {
                        throw new ArgumentException("Usage: bulk-upsert <table> <alternate-key-column> <description-column> --confirm");
                    }
                    using (ServiceClient service = CreateServiceClient(environmentUrl, accessToken))
                    {
                        BulkOperations.RunUpsertDemo(service, args[1], args[2], args[3]);
                    }
                    break;
                case "bulk-delete-elastic":
                    RequireConfirmation(args, "bulk-delete-elastic <table> <id-column> <partition-id> <guid> [guid...] --confirm");
                    if (args.Length < 6)
                    {
                        throw new ArgumentException("Usage: bulk-delete-elastic <table> <id-column> <partition-id> <guid> [guid...] --confirm");
                    }
                    using (ServiceClient service = CreateServiceClient(environmentUrl, accessToken))
                    {
                        Guid[] ids = args[4..]
                            .Where(value => !value.Equals("--confirm", StringComparison.OrdinalIgnoreCase))
                            .Select(Guid.Parse)
                            .ToArray();
                        BulkOperations.RunDeleteMultiple(service, args[1], args[2], args[3], ids);
                    }
                    break;
                case string fileCommand when FileCommandRouter.Commands.Contains(fileCommand):
                    using (ServiceClient service = CreateServiceClient(environmentUrl, accessToken))
                    {
                        FileCommandRouter.Execute(service, args);
                    }
                    break;
                default:
                    Console.WriteLine($"Unknown command: {command}\n");
                    ShowHelp();
                    return 2;
            }

            return 0;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error: {exception.Message}");
            return 1;
        }
    }

    private static async Task<string> AcquireTokenAsync(string environmentUrl)
    {
        string userName = Environment.GetEnvironmentVariable("DATAVERSE_USER") ?? DefaultUserName;
        string clientId = Environment.GetEnvironmentVariable("DATAVERSE_CLIENT_ID") ?? SampleClientId;
        string[] scopes = [$"{environmentUrl}/.default"];
        IPublicClientApplication application = PublicClientApplicationBuilder
            .Create(clientId)
            .WithAuthority("https://login.microsoftonline.com/organizations")
            .Build();

        IAccount? account = (await application.GetAccountsAsync()).FirstOrDefault();
        if (account is not null)
        {
            try
            {
                return (await application.AcquireTokenSilent(scopes, account).ExecuteAsync()).AccessToken;
            }
            catch (MsalUiRequiredException)
            {
            }
        }

        AuthenticationResult result = await application
            .AcquireTokenWithDeviceCode(scopes, deviceCode =>
            {
                Console.WriteLine("Microsoft sign-in is required.");
                Console.WriteLine($"Open: {deviceCode.VerificationUrl}");
                Console.WriteLine($"Code: {deviceCode.UserCode}");
                Console.WriteLine($"Account: {userName}\n");
                return Task.CompletedTask;
            })
            .ExecuteAsync();
        return result.AccessToken;
    }

    private static HttpClient CreateHttpClient(string accessToken)
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return client;
    }

    private static ServiceClient CreateServiceClient(string environmentUrl, string accessToken)
    {
        ServiceClient service = new(new Uri(environmentUrl), _ => Task.FromResult(accessToken), useUniqueInstance: true);
        if (!service.IsReady)
        {
            string details = service.LastException?.Message ?? service.LastError;
            service.Dispose();
            throw new InvalidOperationException($"Dataverse SDK connection failed. {details}");
        }

        return service;
    }

    private static async Task SearchQueryAsync(HttpClient client, string environmentUrl, string searchTerm)
    {
        ValidateSearchTerm(searchTerm, 1, "Query");
        object[] entities =
        [
            new { name = "account", selectColumns = new[] { "name", "createdon", "address1_city" }, searchColumns = new[] { "name", "address1_city" }, filter = "statecode eq 0" },
            new { name = "contact", selectColumns = new[] { "fullname", "createdon", "emailaddress1" }, searchColumns = new[] { "fullname", "emailaddress1" }, filter = "statecode eq 0" }
        ];
        var body = new
        {
            search = searchTerm,
            count = true,
            top = 7,
            skip = 0,
            entities = JsonSerializer.Serialize(entities),
            facets = JsonSerializer.Serialize(new[] { "entityname,count:100" })
        };
        await PostSearchAsync(client, environmentUrl, "query", body, $"Search query: {searchTerm}");
    }

    private static async Task SuggestAsync(HttpClient client, string environmentUrl, string searchTerm)
    {
        ValidateSearchTerm(searchTerm, 3, "Suggest");
        await PostSearchAsync(client, environmentUrl, "suggest", new { search = searchTerm, top = 3, fuzzy = false }, $"Search suggestions: {searchTerm}");
    }

    private static async Task AutocompleteAsync(HttpClient client, string environmentUrl, string searchTerm)
    {
        ValidateSearchTerm(searchTerm, 1, "Autocomplete");
        object[] entities =
        [
            new { name = "account", selectColumns = new[] { "name", "createdon" }, searchColumns = new[] { "name" }, filter = (string?)null }
        ];
        await PostSearchAsync(client, environmentUrl, "autocomplete", new
        {
            search = searchTerm,
            fuzzy = true,
            entities = JsonSerializer.Serialize(entities),
            filter = (string?)null
        }, $"Autocomplete: {searchTerm}");
    }

    private static async Task RunAllReadOnlyAsync(HttpClient client, string environmentUrl, string searchTerm)
    {
        ValidateSearchTerm(searchTerm, 1, "Query");
        await SearchQueryAsync(client, environmentUrl, searchTerm);
        await RespectSearchRateLimitAsync();
        string suggestion = searchTerm.Length >= 3 ? searchTerm[..Math.Min(4, searchTerm.Length)] : searchTerm.PadRight(3, searchTerm[^1]);
        await SuggestAsync(client, environmentUrl, suggestion);
        await RespectSearchRateLimitAsync();
        await AutocompleteAsync(client, environmentUrl, searchTerm[..Math.Min(3, searchTerm.Length)]);
        await RespectSearchRateLimitAsync();
        await GetSearchAsync(client, environmentUrl, "status", "Dataverse Search status");
        await RespectSearchRateLimitAsync();
        await GetSearchAsync(client, environmentUrl, "statistics", "Dataverse Search statistics");
    }

    private static async Task PostSearchAsync(HttpClient client, string environmentUrl, string operation, object body, string title)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync($"{environmentUrl}/api/search/v2.0/{operation}", body);
        await PrintResponseAsync(response, title);
    }

    private static async Task GetSearchAsync(HttpClient client, string environmentUrl, string operation, string title)
    {
        using HttpResponseMessage response = await client.GetAsync($"{environmentUrl}/api/search/v2.0/{operation}");
        await PrintResponseAsync(response, title);
    }

    private static async Task LegacyQueryAsync(HttpClient client, string environmentUrl, string searchTerm)
    {
        ValidateSearchTerm(searchTerm, 1, "Legacy query");
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            $"{environmentUrl}/api/search/v1.0/query",
            new { search = searchTerm, returntotalrecordcount = true, top = 7 });
        await PrintResponseAsync(response, $"Legacy v1 query: {searchTerm}");
    }

    private static async Task PrintResponseAsync(HttpResponseMessage response, string title)
    {
        string content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"{(int)response.StatusCode} {response.ReasonPhrase}: {content}");
        }

        using JsonDocument outer = JsonDocument.Parse(content);
        JsonElement value = outer.RootElement;
        Console.WriteLine($"== {title} ==");
        if (value.TryGetProperty("response", out JsonElement wrapped) && wrapped.ValueKind == JsonValueKind.String)
        {
            using JsonDocument inner = JsonDocument.Parse(wrapped.GetString()!);
            Console.WriteLine(JsonSerializer.Serialize(inner.RootElement, PrettyJson));
            return;
        }
        Console.WriteLine(JsonSerializer.Serialize(value, PrettyJson));
    }

    private static bool IsSearchEnabled(IOrganizationService service)
    {
        QueryExpression query = new("organization") { ColumnSet = new ColumnSet("isexternalsearchindexenabled"), TopCount = 1 };
        return service.RetrieveMultiple(query).Entities.Single().GetAttributeValue<bool>("isexternalsearchindexenabled");
    }

    private static void ShowTableSettings(IOrganizationService service, string logicalName)
    {
        RetrieveMetadataChangesRequest request = new()
        {
            Query = new EntityQueryExpression
            {
                Properties = new MetadataPropertiesExpression("LogicalName", "CanEnableSyncToExternalSearchIndex", "ChangeTrackingEnabled", "SyncToExternalSearchIndex")
            }
        };
        request.Query.Criteria.Conditions.Add(new MetadataConditionExpression("LogicalName", MetadataConditionOperator.Equals, logicalName));
        EntityMetadata table = ((RetrieveMetadataChangesResponse)service.Execute(request)).EntityMetadata.SingleOrDefault()
            ?? throw new InvalidOperationException($"Table '{logicalName}' was not found.");
        Console.WriteLine($"Table: {table.LogicalName}");
        Console.WriteLine($"Can enable search: {table.CanEnableSyncToExternalSearchIndex?.Value}");
        Console.WriteLine($"Change tracking enabled: {table.ChangeTrackingEnabled}");
        Console.WriteLine($"Included in search index: {table.SyncToExternalSearchIndex}");
    }

    private static void ListAnalyzerSettings(IOrganizationService service, string? entityName, string? attributeName)
    {
        QueryExpression query = new("searchattributesettings")
        {
            ColumnSet = new ColumnSet("entityname", "attributename", "settings"),
            TopCount = 100
        };
        if (!string.IsNullOrWhiteSpace(entityName)) query.Criteria.AddCondition("entityname", ConditionOperator.Equal, entityName);
        if (!string.IsNullOrWhiteSpace(attributeName)) query.Criteria.AddCondition("attributename", ConditionOperator.Equal, attributeName);
        EntityCollection records = service.RetrieveMultiple(query);
        Console.WriteLine($"Analyzer settings found: {records.Entities.Count}");
        foreach (Entity record in records.Entities)
        {
            Console.WriteLine($"{record.GetAttributeValue<string>("entityname")}.{record.GetAttributeValue<string>("attributename")}: {record.GetAttributeValue<string>("settings")}");
        }
    }

    private static void SetAnalyzer(IOrganizationService service, string[] args)
    {
        if (args.Length < 5 || !args.Contains("--confirm", StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Usage: analyzer-set <table> <column> <analyzer> --confirm [--overwrite]");
        }
        string entityName = args[1];
        string attributeName = args[2];
        string analyzer = args[3];
        bool overwrite = args.Contains("--overwrite", StringComparer.OrdinalIgnoreCase);
        RetrieveEntityResponse metadata = (RetrieveEntityResponse)service.Execute(new RetrieveEntityRequest
        {
            LogicalName = entityName,
            EntityFilters = EntityFilters.Entity
        });
        if (string.Equals(metadata.EntityMetadata.PrimaryNameAttribute, attributeName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Microsoft advises against setting an analyzer on a primary name column.");
        }

        QueryExpression query = new("searchattributesettings") { ColumnSet = new ColumnSet("settings") };
        query.Criteria.AddCondition("entityname", ConditionOperator.Equal, entityName);
        query.Criteria.AddCondition("attributename", ConditionOperator.Equal, attributeName);
        EntityCollection existing = service.RetrieveMultiple(query);
        if (existing.Entities.Count > 1) throw new InvalidOperationException("Duplicate analyzer settings exist for this table and column.");
        if (existing.Entities.Count == 1 && !overwrite) throw new InvalidOperationException("A setting exists. Add --overwrite to replace it.");
        if (existing.Entities.Count == 1) service.Delete("searchattributesettings", existing.Entities[0].Id);

        string settings = JsonSerializer.Serialize(new Dictionary<string, string> { ["analyzer"] = analyzer });
        Entity record = new("searchattributesettings")
        {
            ["entityname"] = entityName,
            ["attributename"] = attributeName,
            ["settings"] = settings
        };
        Guid id = service.Create(record);
        Console.WriteLine($"Created analyzer setting {id}: {entityName}.{attributeName} = {settings}");
        Console.WriteLine("Index changes can take 15 minutes or longer. Use status to monitor sync.");
    }

    private static void ValidateSearchTerm(string value, int minimumLength, string operation)
    {
        if (value.Length < minimumLength || value.Length > 100)
        {
            throw new ArgumentException($"{operation} text must be {minimumLength}-100 characters.");
        }
    }

    private static void RequireConfirmation(string[] args, string usage)
    {
        if (!args.Contains("--confirm", StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"This command changes Dataverse data. Usage: {usage}");
        }
    }

    private static Task RespectSearchRateLimitAsync() => Task.Delay(TimeSpan.FromMilliseconds(1100));

    private static void ShowHelp()
    {
        Console.WriteLine(string.Join(Environment.NewLine,
        [
                "Search for Dataverse records",
                        "",
                        "Usage: dotnet run -- [command] [arguments]",
                        "",
                        "Read-only commands:",
                        "  query [text]                       Search accounts and contacts (default)",
                        "  suggest [text]                     Return up to three suggestions",
                        "  autocomplete [text]                Complete an account name",
                        "  status                             Show index, table, and column status",
                        "  statistics                         Show index storage and document count",
                        "  all [text]                         Run all five modern search operations",
                        "  enabled                            Read the organization enablement flag",
                        "  table-settings [table]             Show table search metadata",
                        "  legacy [text]                      Run the supported legacy v1 query",
                        "  analyzer-list [table] [column]     List analyzer configuration rows",
                        "",
                        "Configuration command (writes data):",
                        "  analyzer-set <table> <column> <analyzer> --confirm [--overwrite]",
                        "",
                        "Bulk operation commands:",
                        "  bulk-availability [table]          Check message support (read-only)",
                        "  bulk-create-update [count] --confirm",
                        "  bulk-upsert <table> <key> <description-column> --confirm",
                        "  bulk-delete-elastic <table> <id-column> <partition-id> <guid> [guid...] --confirm",
                        "",
                        "File and image commands:",
                        "  file-max <table> <file-column>",
                        "  file-info <table> <guid> <file-column>",
                        "  file-related <table> <guid>",
                        "  file-upload <table> <guid> <column> <path> --confirm",
                        "  file-download <table> <guid> <column> <output-path>",
                        "  file-delete <file-guid> --confirm",
                        "  image-max <table> <image-column>",
                        "  image-full-columns | image-primary-columns",
                        "  image-thumbnail <table> <guid> <column> <output-path>",
                        "  image-upload <table> <guid> <column> <path> --confirm",
                        "  image-update <table> <guid> <column> <path> --confirm",
                        "  image-delete <table> <guid> <column> --confirm",
                        "",
                        "Attachment, note, and SAS commands:",
                        "  attachment-upload <email-guid> <path> --confirm",
                        "  attachment-upload-direct <email-guid> <path> --confirm",
                        "  attachment-download <attachment-guid> <output-path>",
                        "  note-upload <table> <guid> <path> --confirm",
                        "  note-upload-direct <table> <guid> <path> --confirm",
                        "  note-download <note-guid> <output-path>",
                        "  attachment-max",
                        "  file-sas-url <table> <guid> [column] [retained|bin] --confirm",
                        "",
                        "Optional environment variables:",
                        "  DATAVERSE_URL",
                        "  DATAVERSE_USER",
                        "  DATAVERSE_CLIENT_ID"
        ]));
    }
}