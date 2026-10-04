# Dataverse samples: local entry points

This directory contains the complete `dataverse` tree imported from
`microsoft/PowerApps-Samples` at commit
`ce139158966e085cc8e0d7ee2c81f9ff893ec69e` (master).

## Validated solution

`Dataverse.ReadOnlySamples.sln` contains two read-only `WhoAmI` samples:

- `OrgServiceWhoAmI` uses `Microsoft.PowerPlatform.Dataverse.Client`.
- `WebApiWhoAmI` uses MSAL and the Dataverse Web API.

Both default to:

- Environment: `https://org5efadec2.crm.dynamics.com/`
- Login hint: `guazha@dynamicsFTEGCR.onmicrosoft.com`

Authentication is interactive OAuth. No password or client secret is stored.

## Build

```powershell
dotnet build .\Dataverse.ReadOnlySamples.sln
```

## Run

Run either sample and complete the Microsoft sign-in prompt:

```powershell
dotnet run --project .\orgsvc\CSharp-NETCore\ServiceClient\WhoAmI\WhoAmI.csproj
dotnet run --project .\webapi\CSharp-NETx\QuickStart\QuickStart.csproj
```

The Web API sample also supports temporary overrides:

```powershell
$env:DATAVERSE_URL = 'https://example.crm.dynamics.com'
$env:DATAVERSE_USERNAME = 'user@example.onmicrosoft.com'
dotnet run --project .\webapi\CSharp-NETx\QuickStart\QuickStart.csproj
```

Other upstream projects may create, update, or delete Dataverse data. Review
their README and source before running them against a shared environment.
