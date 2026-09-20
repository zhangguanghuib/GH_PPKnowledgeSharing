# Query Data Using FetchXML

A .NET 8 console application for running FetchXML examples against Microsoft Dataverse with the official `Microsoft.PowerPlatform.Dataverse.Client` SDK.

The defaults are configured for:

- Environment: `https://org5efadec2.crm.dynamics.com/`
- Sign-in hint: `guazha@dynamicsFTEGCR.onmicrosoft.com`

No password, access token, or client secret is stored. The application uses Microsoft OAuth device-code sign-in and prints the Microsoft sign-in URL and one-time code in the terminal.

## Run

From this folder:

```powershell
dotnet restore
dotnet run
```

The default command queries five Account rows. Other examples:

```powershell
dotnet run -- filter "Adventure"
dotnet run -- join
dotnet run -- aggregate
dotnet run -- count
dotnet run -- paged 3 2
dotnet run -- paged 5000 0
dotnet run -- custom .\my-query.fetchxml
dotnet run -- help
```

For `paged`, the second number is the maximum number of pages. Use `0` only when you intend to retrieve all matching rows.

## Configuration

Override any default for the current PowerShell session:

```powershell
$env:DATAVERSE_URL = "https://yourorg.crm.dynamics.com/"
$env:DATAVERSE_USER = "you@contoso.com"
$env:DATAVERSE_CLIENT_ID = "your-app-registration-client-id"
dotnet run
```

The built-in client ID is Microsoft's value for development samples. Register a tenant-owned Microsoft Entra public client application with device-code flow enabled and use `DATAVERSE_CLIENT_ID` for production code.

## Examples Covered

- Select specific columns and sort rows
- Filter with an XML-safe, dynamically constructed condition
- Outer join a related table and read aliased values
- Group and aggregate data
- Request the total matching record count
- Page deterministically with the server-provided paging cookie
- Execute a custom FetchXML document
- Render common SDK values and formatted labels

Dataverse omits null attributes from SDK results, so the output only shows attributes actually returned. The examples intentionally avoid `all-attributes`, unsupported `top` combinations, and SQL query hints.

## Documentation

- [Query data using FetchXML](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/overview)
- [Retrieve data](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/retrieve-data)
- [Select columns](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/select-columns)
- [Join tables](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/join-tables)
- [Order rows](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/order-rows)
- [Filter rows](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/filter-rows)
- [Page results](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/page-results)
- [Aggregate data](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/aggregate-data)
- [Count rows](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/count-rows)
- [Optimize performance](https://learn.microsoft.com/power-apps/developer/data-platform/fetchxml/optimize-performance)