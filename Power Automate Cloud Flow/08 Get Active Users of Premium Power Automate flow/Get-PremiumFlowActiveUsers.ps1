<#
.SYNOPSIS
    Identify active users of premium cloud flows across a set of Power Platform environments.

.DESCRIPTION
    For each environment:
      1. Reads the Dataverse `flowrun` table for the last N days (default 28).
      2. Collects the distinct parent flows (workflow ids) that had runs.
      3. Loads each flow's definition (workflow.clientdata) and enumerates the
         connectors used by its triggers and actions.
      4. Classifies the flow as PREMIUM if any connector is Premium tier,
         a custom connector, HTTP / HTTP+Swagger, or uses the on-prem gateway.
        5. For every run of a premium flow, records the run record's owning user
            (flowrun.owninguser -> systemuser: AAD Object Id / email / name).
      6. Emits one CSV row per (Environment, User) with counts and last-run
         timestamp, and a second CSV with the per-flow premium classification.

    Auth model: interactive Azure AD sign-in via Az.Accounts. The signed-in
    principal needs, per environment:
      - Power Platform admin OR Environment Admin (to enumerate connectors/APIs),
      - A Dataverse security role that can read `flowrun`, `workflow`,
        `connectionreference`, `systemuser` (e.g. System Administrator or
        Environment Maker + custom read privileges).

.PARAMETER TenantId
    Azure AD tenant id (GUID).

.PARAMETER EnvironmentIds
    One or more Power Platform environment ids (the GUIDs, not the friendly names).

.PARAMETER LookbackDays
    How many days of run history to inspect. Default 28 (standard flowrun
    retention). Pass a larger value only if extended retention is configured.

.PARAMETER OutputFolder
    Folder for the CSV outputs. Created if missing. Default: current directory.

.PARAMETER TreatCustomAndHttpAsPremium
    If $true (default) custom connectors, HTTP / HTTP+Swagger and on-prem gateway
    usage cause a flow to be classified as premium. Set $false to only count
    connectors whose tier is explicitly "Premium".

.EXAMPLE
    .\Get-PremiumFlowActiveUsers.ps1 `
        -TenantId 00000000-0000-0000-0000-000000000000 `
        -EnvironmentIds 11111111-1111-1111-1111-111111111111,22222222-2222-2222-2222-222222222222 `
        -LookbackDays 28 `
        -OutputFolder .\out
#>

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSAvoidUsingWriteHost', '', Justification = 'Colored progress messages are intentional user-facing output.')]
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$TenantId,

    [Parameter(Mandatory = $true)]
    [string[]]$EnvironmentIds,

    [int]$LookbackDays = 28,

    [string]$OutputFolder = (Get-Location).Path,

    [bool]$TreatCustomAndHttpAsPremium = $true
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# --------------------------- Prerequisites --------------------------- #

if (-not (Get-Module -ListAvailable -Name Az.Accounts)) {
    throw "Az.Accounts module is required. Install with: Install-Module Az.Accounts -Scope CurrentUser"
}
Import-Module Az.Accounts -ErrorAction Stop

if (-not (Test-Path $OutputFolder)) {
    New-Item -ItemType Directory -Path $OutputFolder | Out-Null
}

# ----------------------------- Sign in ------------------------------- #

$ctx = Get-AzContext -ErrorAction SilentlyContinue
if (-not $ctx -or $ctx.Tenant.Id -ne $TenantId) {
    Write-Host "Signing in to tenant $TenantId ..."
    Connect-AzAccount -TenantId $TenantId | Out-Null
}

function Get-Token {
    param([Parameter(Mandatory)][string]$Resource)
    # Az returns a SecureString in newer versions; handle both shapes.
    $t = Get-AzAccessToken -ResourceUrl $Resource -TenantId $TenantId -WarningAction SilentlyContinue
    if ($t.Token -is [System.Security.SecureString]) {
        return [System.Net.NetworkCredential]::new('', $t.Token).Password
    }
    return $t.Token
}

$bapToken       = Get-Token -Resource 'https://api.bap.microsoft.com/'
# api.powerapps.com validates tokens whose audience is the PowerApps *service* URI.
$powerAppsToken = Get-Token -Resource 'https://service.powerapps.com/'

function Invoke-Json {
    param(
        [Parameter(Mandatory)][string]$Uri,
        [Parameter(Mandatory)][string]$Token,
        [string]$Method = 'GET'
    )
    $headers = @{
        Authorization = "Bearer $Token"
        Accept        = 'application/json'
    }
    return Invoke-RestMethod -Uri $Uri -Headers $headers -Method $Method
}

# ------------------------- Helper functions -------------------------- #

function Test-HasProperty {
    <#
      Safe replacement for `$obj.PSObject.Properties.Name -contains 'X'`.
      In Windows PowerShell 5.1, under Set-StrictMode, member-enumerating
      .Name (or even .Count) on an EMPTY PSObject.Properties collection
      throws PropertyNotFoundStrict. Piping through ForEach-Object avoids it.
    #>
    param($InputObject, [Parameter(Mandatory)][string]$PropertyName)
    if ($null -eq $InputObject) { return $false }
    $names = @($InputObject.PSObject.Properties | ForEach-Object { $_.Name })
    return $names -contains $PropertyName
}

function Get-EnvironmentDetail {
    param([string]$EnvId)
    $uri = "https://api.bap.microsoft.com/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/$EnvId`?api-version=2020-10-01&`$expand=properties"
    return Invoke-Json -Uri $uri -Token $bapToken
}

function Get-PremiumConnectorSet {
    <#
      Returns a hashtable keyed by connector name (e.g. "shared_sql")
      whose value is the tier string. Used for classification.
    #>
    param([string]$EnvId)
    $uri = "https://api.powerapps.com/providers/Microsoft.PowerApps/apis?api-version=2016-11-01&`$filter=environment eq '$EnvId'&showApisWithToS=true"
    $resp = Invoke-Json -Uri $uri -Token $powerAppsToken
    $map = @{}
    foreach ($c in $resp.value) {
        $tier = $null
        if (Test-HasProperty $c.properties 'tier') { $tier = $c.properties.tier }
        if (-not $tier -and (Test-HasProperty $c.properties 'metadata') -and $c.properties.metadata) {
            if (Test-HasProperty $c.properties.metadata 'tier') {
                $tier = $c.properties.metadata.tier
            }
        }
        $map[$c.name] = [pscustomobject]@{
            DisplayName = $c.properties.displayName
            Tier        = $tier
            IsCustom    = ((Test-HasProperty $c.properties 'publisher') -and
                           $c.properties.publisher -ne 'Microsoft' -and
                           $c.name -notlike 'shared_*_*_*')  # heuristic; refined below
        }
    }
    return $map
}

# Built-in Logic Apps action / trigger types that mean "premium capability".
$PremiumBuiltInTypes = @(
    'Http', 'HttpWebhook', 'ApiConnectionWebhook'  # ApiConnectionWebhook alone isn't premium, but combined with premium connector it is; kept only for detection when apiId cannot be resolved
)

function Get-FlowConnectorUsage {
    <#
      Given a workflow.clientdata JSON string, returns:
        @{
           ApiIds        = string[]   # e.g. "/providers/Microsoft.PowerApps/apis/shared_sql"
           UsesHttp      = bool
           UsesGateway   = bool
        }
    #>
    param([string]$ClientData)

    $result = [ordered]@{
        ApiIds      = New-Object System.Collections.Generic.HashSet[string]
        UsesHttp    = $false
        UsesGateway = $false
    }

    if ([string]::IsNullOrWhiteSpace($ClientData)) { return $result }

    try {
        # -Depth is only supported on ConvertFrom-Json in PowerShell 6+; Windows PowerShell 5.1 lacks it.
        if ($PSVersionTable.PSVersion.Major -ge 6) {
            $client = $ClientData | ConvertFrom-Json -Depth 100
        } else {
            $client = $ClientData | ConvertFrom-Json
        }
    } catch {
        Write-Verbose "Could not parse clientdata: $($_.Exception.Message)"
        return $result
    }

    $definition = $null
    if ((Test-HasProperty $client 'properties') -and
        (Test-HasProperty $client.properties 'definition')) {
        $definition = $client.properties.definition
    } elseif (Test-HasProperty $client 'definition') {
        $definition = $client.definition
    }
    if (-not $definition) { return $result }

    $stack = New-Object System.Collections.Stack
    $stack.Push($definition)

    while ($stack.Count -gt 0) {
        $node = $stack.Pop()
        if ($null -eq $node) { continue }

        if ($node -is [string] -or $node.GetType().IsValueType) { continue }

        if ($node -is [System.Collections.IEnumerable] -and -not ($node -is [string])) {
            foreach ($item in $node) { $stack.Push($item) }
            continue
        }

        # PSCustomObject
        $propNames = @($node.PSObject.Properties | ForEach-Object { $_.Name })

        # Detect action/trigger "type"
        if ($propNames -contains 'type') {
            $t = [string]$node.type
            if ($t -in $PremiumBuiltInTypes) { $result.UsesHttp = $true }
        }

        # apiId lives in inputs.host.apiId (OpenApiConnection) or inputs.host.api.id (older schema)
        if ($propNames -contains 'apiId' -and $node.apiId) {
            [void]$result.ApiIds.Add([string]$node.apiId)
        }
        if ($propNames -contains 'api' -and $node.api -and
            (Test-HasProperty $node.api 'id')) {
            [void]$result.ApiIds.Add([string]$node.api.id)
        }

        # On-prem data gateway
        if ($propNames -contains 'gateway' -and $node.gateway) {
            $result.UsesGateway = $true
        }

        foreach ($p in $node.PSObject.Properties) { $stack.Push($p.Value) }
    }

    return $result
}

function Get-ConnectorNameFromApiId {
    param([string]$ApiId)
    if ([string]::IsNullOrWhiteSpace($ApiId)) { return $null }
    return ($ApiId -split '/')[-1]  # last segment, e.g. "shared_sql"
}

function Test-FlowIsPremium {
    param(
        [Parameter(Mandatory)]$Usage,
        [Parameter(Mandatory)][hashtable]$PremiumConnectorMap,
        [Parameter(Mandatory)][bool]$TreatCustomAndHttpAsPremium
    )
    if ($TreatCustomAndHttpAsPremium -and ($Usage.UsesHttp -or $Usage.UsesGateway)) {
        return @{ IsPremium = $true; Reason = if ($Usage.UsesGateway) { 'OnPremGateway' } else { 'HttpBuiltIn' } }
    }
    foreach ($apiId in $Usage.ApiIds) {
        $name = Get-ConnectorNameFromApiId -ApiId $apiId
        if (-not $name) { continue }
        if ($PremiumConnectorMap.ContainsKey($name)) {
            $meta = $PremiumConnectorMap[$name]
            if ($meta.Tier -eq 'Premium') {
                return @{ IsPremium = $true; Reason = "PremiumConnector:$name" }
            }
            if ($TreatCustomAndHttpAsPremium -and $meta.IsCustom) {
                return @{ IsPremium = $true; Reason = "CustomConnector:$name" }
            }
        } else {
            if ($TreatCustomAndHttpAsPremium) {
                return @{ IsPremium = $true; Reason = "UnknownConnector:$name" }
            }
        }
    }
    return @{ IsPremium = $false; Reason = '' }
}

function Invoke-DataverseGetAll {
    <#
      Follows @odata.nextLink for paged Dataverse queries.
    #>
    param(
        [Parameter(Mandatory)][string]$Uri,
        [Parameter(Mandatory)][string]$Token
    )
    $all = New-Object System.Collections.Generic.List[object]
    $headers = @{
        Authorization      = "Bearer $Token"
        Accept             = 'application/json'
        'OData-Version'    = '4.0'
        'OData-MaxVersion' = '4.0'
        Prefer             = 'odata.include-annotations="*",odata.maxpagesize=1000'
    }
    $next = $Uri
    while ($next) {
        try {
            $resp = Invoke-RestMethod -Uri $next -Headers $headers -Method GET
        } catch {
            $body = ''
            if ($_.ErrorDetails -and $_.ErrorDetails.Message) {
                $body = $_.ErrorDetails.Message
            } elseif ($_.Exception.Response) {
                try {
                    $stream = $_.Exception.Response.GetResponseStream()
                    if ($stream.CanSeek) { $stream.Position = 0 }
                    $sr = New-Object System.IO.StreamReader($stream)
                    $body = $sr.ReadToEnd()
                } catch {
                    Write-Verbose "Could not read Dataverse error body: $($_.Exception.Message)"
                }
            }
            throw "Dataverse GET failed: $($_.Exception.Message)`nURL: $next`nBody: $body"
        }
        if ($resp.value) { $all.AddRange([object[]]$resp.value) }
        $next = $null
        if (Test-HasProperty $resp '@odata.nextLink') {
            $next = $resp.'@odata.nextLink'
        }
    }
    return $all
}

function Resolve-CloudFlowRunSchema {
    <#
            Validates the documented columns on the `flowrun` (cloud flow run) table.
      Notes:
        - `flowsession` is for Power Automate Desktop runs and is deliberately NOT used here.
                - Schema: https://learn.microsoft.com/power-apps/developer/data-platform/reference/entities/flowrun
                - EntityDefinitions is queried to fail clearly when the table or a required
                    documented column isn't available in an environment.
    #>
    param(
        [Parameter(Mandatory)][string]$DvBase,
        [Parameter(Mandatory)][string]$Token
    )
    $entitySet   = 'flowruns'
    $logicalName = 'flowrun'
    $headers     = @{ Authorization = "Bearer $Token"; Accept = 'application/json' }

    try {
        $attrsUri = "$DvBase/api/data/v9.2/EntityDefinitions(LogicalName='$logicalName')/Attributes?`$select=LogicalName,AttributeType,SchemaName"
        $attrs = Invoke-RestMethod -Uri $attrsUri -Headers $headers -Method GET
    } catch {
        Write-Warning "Cannot read schema of '$logicalName' table: $($_.Exception.Message)"
        return $null
    }

    $names = @($attrs.value | ForEach-Object { $_.LogicalName })

    $requiredColumns = @('flowrunid', 'workflow', 'owninguser', 'starttime')
    $missingColumns = @($requiredColumns | Where-Object { $names -notcontains $_ })
    if ($missingColumns.Count -gt 0) {
        Write-Warning ("'flowrun' schema is missing documented columns: {0}." -f ($missingColumns -join ', '))
        return $null
    }

    return [pscustomobject]@{
        EntitySet   = $entitySet
        LogicalName = $logicalName
        Pk          = 'flowrunid'
        Workflow    = '_workflow_value'
        User        = '_owninguser_value'
        Started     = 'starttime'
        Ended       = if ($names -contains 'endtime') { 'endtime' } else { $null }
        Status      = if ($names -contains 'status') { 'status' } else { $null }
        HasName     = ($names -contains 'name')
    }
}

# ------------------------------ Main --------------------------------- #

$sinceUtc = (Get-Date).ToUniversalTime().AddDays(-$LookbackDays).ToString('yyyy-MM-ddTHH:mm:ssZ')
Write-Host "Lookback window: since $sinceUtc UTC ($LookbackDays days)"

$activeUserRows = New-Object System.Collections.Generic.List[object]
$flowClassificationRows = New-Object System.Collections.Generic.List[object]

foreach ($envId in $EnvironmentIds) {
    Write-Host ""
    Write-Host "=== Environment $envId ===" -ForegroundColor Cyan

    try {
        $env = Get-EnvironmentDetail -EnvId $envId
    } catch {
        Write-Warning "Cannot read environment $envId : $($_.Exception.Message). Skipping."
        continue
    }

    $envDisplayName = $env.properties.displayName
    $instanceUrl = $null
    if ((Test-HasProperty $env.properties 'linkedEnvironmentMetadata') -and
        $env.properties.linkedEnvironmentMetadata) {
        $instanceUrl = $env.properties.linkedEnvironmentMetadata.instanceApiUrl
    }
    if (-not $instanceUrl) {
        Write-Warning "Environment '$envDisplayName' ($envId) has no Dataverse instance. Solution flows require Dataverse. Skipping."
        continue
    }
    Write-Host "Dataverse: $instanceUrl"

    # Per-environment tokens
    $dvResource = ($instanceUrl.TrimEnd('/'))
    $dvToken = Get-Token -Resource $dvResource

    # Premium connector map
    Write-Host "Loading connector catalog..."
    $connectorMap = Get-PremiumConnectorSet -EnvId $envId
    $premiumConnectorCount = @($connectorMap.Values | Where-Object { $_.Tier -eq 'Premium' }).Count
    Write-Host "  $($connectorMap.Count) connectors ( $premiumConnectorCount premium )"

    # Discover the flowrun table columns for this environment.
    $schema = Resolve-CloudFlowRunSchema -DvBase $dvResource -Token $dvToken
    if (-not $schema) {
        Write-Warning "'flowrun' table is not available in this environment. Skipping."
        continue
    }
    Write-Host "Run history table: $($schema.EntitySet) (workflow=$($schema.Workflow); user=$($schema.User); started=$($schema.Started))"

    $startedCol = $schema.Started
    $filter = "$startedCol ge $sinceUtc"
    $selectCols = @($schema.Pk, $startedCol, $schema.Workflow, $schema.User)
    if ($schema.Ended)   { $selectCols += $schema.Ended }
    if ($schema.Status)  { $selectCols += $schema.Status }
    if ($schema.HasName) { $selectCols += 'name' }
    $select = ($selectCols -join ',')

    # Resolve users via a separate systemusers query rather than $expand — many
    # newer tables (flowrun included) reject certain owner expands with 400.
    $flowrunUri = "$dvResource/api/data/v9.2/$($schema.EntitySet)?`$select=$select&`$filter=$([System.Uri]::EscapeDataString($filter))"

    Write-Host "Querying $($schema.EntitySet) ..."
    $runs = Invoke-DataverseGetAll -Uri $flowrunUri -Token $dvToken
    Write-Host "  $($runs.Count) runs in window."

    if ($runs.Count -eq 0) { continue }

    $wfCol = $schema.Workflow
    $userCol = $schema.User
    $flowIds = @($runs | ForEach-Object { $_.$wfCol } | Where-Object { $_ } | Select-Object -Unique)
    Write-Host "  $($flowIds.Count) distinct flows."

    # Resolve systemuser identities for the users referenced by these runs.
    $userIds = @($runs | ForEach-Object { $_.$userCol } | Where-Object { $_ } | Select-Object -Unique)
    $userLookupMap = @{}
    if ($userIds.Count -gt 0) {
        $orClause = ($userIds | ForEach-Object { "systemuserid eq $_" }) -join ' or '
        $usersUri = "$dvResource/api/data/v9.2/systemusers?`$select=systemuserid,azureactivedirectoryobjectid,internalemailaddress,fullname,domainname&`$filter=$([System.Uri]::EscapeDataString($orClause))"
        try {
            $userRows = Invoke-DataverseGetAll -Uri $usersUri -Token $dvToken
            foreach ($u in $userRows) { $userLookupMap[$u.systemuserid] = $u }
        } catch {
            Write-Warning "Could not resolve systemuser identities: $($_.Exception.Message)"
        }
    }

    # Classify each flow
    $flowPremiumCache = @{}   # workflowId -> @{IsPremium; Reason; Name}
    $flowIdx = 0
    foreach ($fid in $flowIds) {
        $flowIdx++
        Write-Progress -Activity "Classifying flows in $envDisplayName" -Status "$flowIdx / $($flowIds.Count)" -PercentComplete (($flowIdx / [double]$flowIds.Count) * 100)
        try {
            $wfUri = "$dvResource/api/data/v9.2/workflows($fid)?`$select=name,category,clientdata,statecode"
            $headers = @{ Authorization = "Bearer $dvToken"; Accept = 'application/json' }
            $wf = Invoke-RestMethod -Uri $wfUri -Headers $headers -Method GET
        } catch {
            Write-Verbose "Workflow $fid not readable: $($_.Exception.Message)"
            $flowPremiumCache[$fid] = @{ IsPremium = $false; Reason = 'DefinitionUnavailable'; Name = '(unknown)' }
            continue
        }
        # category 5 = Modern (cloud) flow. Keep others too but tag.
        $usage = Get-FlowConnectorUsage -ClientData $wf.clientdata
        $decision = Test-FlowIsPremium -Usage $usage -PremiumConnectorMap $connectorMap -TreatCustomAndHttpAsPremium $TreatCustomAndHttpAsPremium
        $flowPremiumCache[$fid] = @{
            IsPremium = $decision.IsPremium
            Reason    = $decision.Reason
            Name      = $wf.name
            ApiIds    = ($usage.ApiIds -join ';')
        }

        $flowClassificationRows.Add([pscustomobject]@{
            EnvironmentId   = $envId
            EnvironmentName = $envDisplayName
            FlowId          = $fid
            FlowName        = $wf.name
            IsPremium       = $decision.IsPremium
            Reason          = $decision.Reason
            ConnectorsUsed  = ($usage.ApiIds -join ';')
        })
    }
    Write-Progress -Activity "Classifying flows in $envDisplayName" -Completed

    $premiumFlowIds = @($flowPremiumCache.GetEnumerator() |
        Where-Object { $_.Value.IsPremium } |
        Select-Object -ExpandProperty Key)
    Write-Host "  $($premiumFlowIds.Count) premium flows."

    if ($premiumFlowIds.Count -eq 0) { continue }

    # Aggregate active users from runs of premium flows
    $premiumRuns = @($runs | Where-Object { $premiumFlowIds -contains $_.$wfCol })
    $blankOwnerRunCount = @($premiumRuns | Where-Object { -not $_.$userCol }).Count
    Write-Host "  $($premiumRuns.Count) premium-flow runs ($blankOwnerRunCount with no owninguser recorded)."
    $byUser = @($premiumRuns | Group-Object -Property $userCol)

    $addedUserCount = 0
    foreach ($g in $byUser) {
        $lastRun = ($g.Group | Measure-Object -Property $startedCol -Maximum).Maximum
        $distinctFlows = @($g.Group | ForEach-Object { $_.$wfCol } | Select-Object -Unique).Count

        $aadId = $null; $email = $null; $fullName = $null; $upn = $null
        $systemUserId = $g.Name
        if ([string]::IsNullOrEmpty($g.Name)) {
            $systemUserId = '(no owninguser recorded)'
        } elseif ($userLookupMap.ContainsKey($g.Name)) {
            $u = $userLookupMap[$g.Name]
            $aadId    = $u.azureactivedirectoryobjectid
            $email    = $u.internalemailaddress
            $fullName = $u.fullname
            $upn      = $u.domainname
        }

        $activeUserRows.Add([pscustomobject]@{
            EnvironmentId       = $envId
            EnvironmentName     = $envDisplayName
            SystemUserId        = $systemUserId
            AzureAdObjectId     = $aadId
            UserPrincipalName   = $upn
            Email               = $email
            FullName            = $fullName
            PremiumFlowRunCount = $g.Count
            DistinctPremiumFlows= $distinctFlows
            LastPremiumRunUtc   = $lastRun
        })
        $addedUserCount++
    }
    Write-Host "  $addedUserCount distinct premium-flow active users (rows written)."
}

# ---------------------------- Output --------------------------------- #

$stamp = (Get-Date).ToString('yyyyMMdd-HHmmss')
$usersCsv = Join-Path $OutputFolder "PremiumFlowActiveUsers-$stamp.csv"
$flowsCsv = Join-Path $OutputFolder "PremiumFlowClassification-$stamp.csv"

$activeUserRows | Sort-Object EnvironmentName, FullName |
    Export-Csv -Path $usersCsv -NoTypeInformation -Encoding UTF8
$flowClassificationRows | Sort-Object EnvironmentName, FlowName |
    Export-Csv -Path $flowsCsv -NoTypeInformation -Encoding UTF8

Write-Host ""
Write-Host "Active users -> $usersCsv" -ForegroundColor Green
Write-Host "Flow classification -> $flowsCsv" -ForegroundColor Green
Write-Host "Done."
