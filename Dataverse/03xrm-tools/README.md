# XRM Tools learning project

This .NET 8 console project consolidates the runnable C# examples from the
[XRM tooling documentation](https://learn.microsoft.com/power-apps/developer/data-platform/xrm-tooling/build-windows-client-applications-xrm-tools)
and its child pages. It also adapts the linked QuickStart and Task Parallel
Library samples from the official
[PowerApps-Samples repository](https://github.com/microsoft/PowerApps-Samples/tree/master/dataverse/Xrm%20Tooling).

The documentation recommends `ServiceClient` for new development, so the
runnable examples use `Microsoft.PowerPlatform.Dataverse.Client.ServiceClient`
while preserving the same XRM tooling operations.

## Authentication

The project is preconfigured for:

- Environment: `https://org5efadec2.crm.dynamics.com/`
- User: `guazha@dynamicsFTEGCR.onmicrosoft.com`

It uses Microsoft's documented development/prototyping application ID and
interactive OAuth. The password is never stored in source or configuration.
Microsoft's sign-in UI opens in the system browser when authentication is
required, including MFA. The redirect URI is `http://localhost`, as used by
Microsoft's .NET ServiceClient samples. The older `app://...` redirect used by
XRM tooling desktop examples is not supported by .NET system-browser login
and causes the MSAL `loopback_redirect_uri` error.
The token cache is stored under the current user's local application data.

Override either setting when needed:

```powershell
$env:DATAVERSE_URL = 'https://yourorg.crm.dynamics.com/'
$env:DATAVERSE_USERNAME = 'user@contoso.onmicrosoft.com'
```

## Run

```powershell
dotnet restore
dotnet build
dotnet run
dotnet run -- all
dotnet run -- --list
dotnet run -- connection
dotnet run -- retrieve
dotnet run -- crud
dotnet run -- messages
dotnet run -- association
dotnet run -- webapi
dotnet run -- parallel
```

`connection` and `retrieve` are read-only. The other samples create temporary
records and delete them before returning. Use a test environment and an account
with the required table privileges.

Running without arguments or with `all` executes every sample in this order,
using one authenticated connection: `connection`, `retrieve`, `crud`,
`messages`, `association`, `webapi`, `parallel`. This includes write operations.
Execution stops on the first failure and returns a nonzero exit code. You can
still pass an individual sample name to run only that sample.

The FetchXML example uses `count='10'` and `page='1'`, not `top`, because
Dataverse does not allow `top` together with `returntotalrecordcount`.
The Web API example supplies fresh custom headers for each request because
the SDK mutates the headers dictionary, and deletes its temporary account
in a `finally` block.

## Debug in VS Code

Open this project directory (`Dataverse\03xrm-tools`) as the VS Code workspace
folder and install Microsoft's C# or C# Dev Kit extension.

1. Set a breakpoint in `SampleRunner.cs` or `SampleOperations.cs`.
2. Open Run and Debug and select **Debug XRM learning sample**.
3. Press **F5** and choose `all` (the default) to run every sample, or choose
   an individual sample such as the read-only `connection`.
4. The pre-launch task builds the project in Debug configuration automatically.
5. Execution pauses at entry. Use **F10** to step over, **F11** to step into,
   and **F5** to continue to your breakpoint.

The configuration is in [.vscode/launch.json](.vscode/launch.json), and its
build task is in [.vscode/tasks.json](.vscode/tasks.json).
Allow write samples to finish normally: stopping the debugger forcibly can
prevent cleanup and leave temporary records in Dataverse.

## Documentation coverage

| Documentation example | Runnable sample |
|---|---|
| Connection string and connection status | `connection` |
| `GetEntityDataById` and FetchXML retrieval | `retrieve` |
| `CreateNewRecord` and `CreateAnnotation` | `crud` |
| `UpdateEntity` and `UpdateStateAndStatusForEntity` | `crud` |
| `DeleteEntity` | cleanup in every write sample |
| `DeleteEntityAssociation` | `association` |
| `CreateRequest` and `RetrieveMultipleRequest` | `messages` |
| `ExecuteWebRequest` | `webapi` |
| QuickStart create/retrieve/update/delete | `crud` |
| Task Parallel Library and `ServiceClient.Clone` | `parallel` |
| Common WPF login control | `LegacyWpfLoginControlExample.cs.txt` |
| XRM tooling tracing configuration | `XrmToolingTrace.config.example` |

The WPF login control requires a separate .NET Framework 4.8 WPF application
and the `Microsoft.CrmSdk.XrmTooling.WpfControls` package. Its page snippet is
kept as a noncompiled reference because it cannot run inside this .NET 8
console executable.
