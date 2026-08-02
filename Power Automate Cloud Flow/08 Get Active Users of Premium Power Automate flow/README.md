# Power Automate premium active users

`Get-PremiumFlowActiveUser.ps1` calls the same preview licensing endpoint used by
the Power Platform admin center to list active users who triggered premium flows.

> This endpoint is undocumented and can change without notice. Use it for
> administrative reporting with an authorized Power Platform or Global admin
> account, and validate its output against the admin center.

## Run

Prerequisites:

- PowerShell 7 or Windows PowerShell 5.1
- Azure CLI, signed in to the target tenant with
  `az login --tenant <tenant-id> --allow-no-subscriptions`
- A user authorized to view Power Platform licensing data

```powershell
az login --tenant '<Tenant Id>' --allow-no-subscriptions

.\Get-PremiumFlowActiveUser.ps1 -TenantId '<Tenant Id>' -Verbose
```

Only `-TenantId` is required. By default, the script queries the previous 30
days, ending at the current UTC time. To use a different period, optionally
provide both dates:

```powershell
.\Get-PremiumFlowActiveUser.ps1 `
  -TenantId '<Tenant Id>' `
  -StartTime '2026-07-03T13:52:29Z' `
  -EndTime '2026-08-02T13:52:29Z' `
  -Verbose
```

The script writes these files in the current directory:

- `premium-flow-active-users.csv`: one row per unique active user, with flow and
  environment counts and the user's email address
- `premium-flow-active-user-details.csv`: one row per user and premium flow,
  including the user's email address
- `premium-flow-user-email-mapping.csv`: reusable mapping of Entra user ID to
  email address and user principal name
- `premium-flow-active-users.raw.json`: unmodified API response

The raw file is retained because this preview API has no published response
schema.

Close generated CSV files in Excel or other applications before running the
script again, because Windows might prevent the script from overwriting them.

If Azure CLI cannot obtain a token for this internal resource, capture a fresh
token from your own authenticated Admin Center session and pass it through the
process environment rather than putting it in a script or shell history:

```powershell
$env:POWER_PLATFORM_LICENSING_TOKEN = Read-Host 'Access token' -MaskInput
.\Get-PremiumFlowActiveUser.ps1 -TenantId '<Tenant Id>'
Remove-Item Env:POWER_PLATFORM_LICENSING_TOKEN
```

`Read-Host -MaskInput` requires PowerShell 7. For Windows PowerShell 5.1, set the
environment variable through a secure local mechanism instead. Azure CLI is
still used to acquire a Microsoft Graph token for user email enrichment unless
`MICROSOFT_GRAPH_TOKEN` is also supplied through the environment.

The browser-only headers (`origin`, `referer`, `sec-*`, user agent, correlation
IDs, and session IDs) are intentionally omitted. They are not authentication
credentials and should not be replayed.

## Security

Never commit or share bearer tokens. A token included in a message, log, or
ticket should be considered exposed; revoke its session or wait for it to expire
before continuing with a newly acquired token.

## Notes

- `TenantId` is the only required parameter. `StartTime`, `EndTime`, `PageSize`,
  `MaximumPages`, `LicenseClassification`, token values, and output paths are
  optional.
- The default lookback is 30 days ending at the current UTC time, and only
  active users are requested.
- Generated CSV reports include only records where `isPremiumFlow` is `True`
  and `hasPremiumFeatures` is `1`. The raw JSON remains unfiltered.
- `lastRunDate`, `createdDate`, and `modifiedDate` are converted from Unix
  microseconds to ISO 8601 UTC values such as `2026-08-02T10:31:23.715300Z` in
  the generated CSV reports. The raw JSON retains the original numbers.
- User IDs are resolved through Microsoft Graph using the Azure CLI tenant
  login. The `mail` property is preferred; `userPrincipalName` is used when
  `mail` is empty. Deleted users remain in the reports with a blank email.
- The API ignores `pageNumber` and doesn't currently return a continuation
  token. The script requests up to 1,000 records in one call and stops with an
  explicit error rather than silently duplicating records if that limit is met.
- The default license classification matches the Admin Center request:
  `Power Automate per user with attended RPA`.
- Change `-LicenseClassification` if the Admin Center starts sending a different
  value for the Power Automate Premium SKU.
- HTTP 401 usually means an expired token or incorrect token audience. HTTP 403
  usually means the signed-in account lacks permission or Azure CLI's client is
  not permitted to request this internal API.