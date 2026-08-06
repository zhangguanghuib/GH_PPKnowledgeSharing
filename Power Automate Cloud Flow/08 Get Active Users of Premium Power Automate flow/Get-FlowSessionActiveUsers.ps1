<#
.SYNOPSIS
    Export users and their valid Flow Session rows created in a recent time window.

.DESCRIPTION
    Queries the Dataverse Flow Session (flowsession) table and exports one CSV row
    per session. Rows are sorted by creator so each user's sessions are contiguous.
    A session is included only when its Run mode and ParentWorkflowId agree:
      - Local: ParentWorkflowId is empty.
      - Attended or Unattended: ParentWorkflowId is not empty.

    Requires Az.Accounts and a Dataverse security role with read access to the
    flowsession and systemuser tables.

.PARAMETER TenantId
    Microsoft Entra tenant ID.

.PARAMETER EnvironmentId
    Power Platform environment ID. Defaults to the environment from the request.

.PARAMETER LookbackDays
    Number of days to include, based on Created On. Defaults to 30.

.PARAMETER OutputPath
    CSV output path. The parent directory is created when necessary.

.EXAMPLE
    .\Get-FlowSessionActiveUsers.ps1 `
        -TenantId 00000000-0000-0000-0000-000000000000 `
        -OutputPath .\out\FlowSessionActiveUsers.csv
#>

[Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSAvoidUsingWriteHost', '', Justification = 'Progress messages are intentional user-facing output.')]
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9a-fA-F-]{36}$')]
    [string]$TenantId,

    [ValidatePattern('^[0-9a-fA-F-]{36}$')]
    [string]$EnvironmentId = '5e61b5c4-120e-e1fb-9fee-3073d48f0e86',

    [ValidateRange(1, 3650)]
    [int]$LookbackDays = 30,

    [string]$OutputPath = (Join-Path (Get-Location).Path 'FlowSessionActiveUsers.csv')
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

if (-not (Get-Module -ListAvailable -Name Az.Accounts)) {
    throw 'Az.Accounts is required. Install it with: Install-Module Az.Accounts -Scope CurrentUser'
}
Import-Module Az.Accounts -ErrorAction Stop

function Test-HasProperty {
    param($InputObject, [Parameter(Mandatory = $true)][string]$PropertyName)

    if ($null -eq $InputObject) { return $false }
    $propertyNames = @($InputObject.PSObject.Properties | ForEach-Object { $_.Name })
    return $propertyNames -contains $PropertyName
}

function Get-AccessTokenText {
    param([Parameter(Mandatory = $true)][string]$ResourceUrl)

    $tokenResult = Get-AzAccessToken -ResourceUrl $ResourceUrl -TenantId $TenantId -WarningAction SilentlyContinue
    if ($tokenResult.Token -is [System.Security.SecureString]) {
        return [System.Net.NetworkCredential]::new('', $tokenResult.Token).Password
    }
    return $tokenResult.Token
}

function Invoke-DataverseGetAll {
    param(
        [Parameter(Mandatory = $true)][string]$Uri,
        [Parameter(Mandatory = $true)][string]$Token
    )

    $headers = @{
        Authorization      = "Bearer $Token"
        Accept             = 'application/json'
        'OData-Version'    = '4.0'
        'OData-MaxVersion' = '4.0'
        Prefer             = 'odata.include-annotations="OData.Community.Display.V1.FormattedValue",odata.maxpagesize=5000'
    }
    $rows = New-Object System.Collections.Generic.List[object]
    $nextLink = $Uri

    while ($nextLink) {
        try {
            $response = Invoke-RestMethod -Uri $nextLink -Headers $headers -Method GET
        } catch {
            $details = if ($_.ErrorDetails -and $_.ErrorDetails.Message) { $_.ErrorDetails.Message } else { '' }
            throw "Dataverse GET failed: $($_.Exception.Message)`nURL: $nextLink`nBody: $details"
        }

        if ($response.value) {
            $rows.AddRange([object[]]$response.value)
        }
        $nextLink = $null
        if (Test-HasProperty -InputObject $response -PropertyName '@odata.nextLink') {
            $nextLink = $response.'@odata.nextLink'
        }
    }

    return $rows
}

function Get-FormattedValue {
    param(
        [Parameter(Mandatory = $true)]$Row,
        [Parameter(Mandatory = $true)][string]$Column,
        $Fallback = ''
    )

    $annotation = "$Column@OData.Community.Display.V1.FormattedValue"
    if (Test-HasProperty -InputObject $Row -PropertyName $annotation) {
        return $Row.$annotation
    }
    return $Fallback
}

function Get-PropertyValue {
    param(
        [Parameter(Mandatory = $true)]$Row,
        [Parameter(Mandatory = $true)][string]$Column,
        $Fallback = $null
    )

    if (Test-HasProperty -InputObject $Row -PropertyName $Column) {
        return $Row.$Column
    }
    return $Fallback
}

$currentContext = Get-AzContext -ErrorAction SilentlyContinue
if (-not $currentContext -or $currentContext.Tenant.Id -ne $TenantId) {
    Write-Host "Signing in to tenant $TenantId ..."
    Connect-AzAccount -TenantId $TenantId | Out-Null
}

$bapToken = Get-AccessTokenText -ResourceUrl 'https://api.bap.microsoft.com/'
$environmentUri = "https://api.bap.microsoft.com/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/$EnvironmentId`?api-version=2020-10-01&`$expand=properties"
$environmentHeaders = @{ Authorization = "Bearer $bapToken"; Accept = 'application/json' }

Write-Host "Resolving environment $EnvironmentId ..."
$environment = Invoke-RestMethod -Uri $environmentUri -Headers $environmentHeaders -Method GET
$dataverseUrl = $null
if ((Test-HasProperty -InputObject $environment.properties -PropertyName 'linkedEnvironmentMetadata') -and
    $environment.properties.linkedEnvironmentMetadata) {
    $dataverseUrl = $environment.properties.linkedEnvironmentMetadata.instanceApiUrl
}
if ([string]::IsNullOrWhiteSpace($dataverseUrl)) {
    throw "Environment $EnvironmentId does not have a Dataverse instance URL."
}
$dataverseUrl = $dataverseUrl.TrimEnd('/')
$dataverseToken = Get-AccessTokenText -ResourceUrl $dataverseUrl

$sinceUtc = (Get-Date).ToUniversalTime().AddDays(-$LookbackDays).ToString('yyyy-MM-ddTHH:mm:ssZ')
$modeRule = "(runmode eq 0 and (parentworkflowid eq null or parentworkflowid eq '')) or ((runmode eq 1 or runmode eq 2) and parentworkflowid ne null and parentworkflowid ne '')"
$filter = "createdon ge $sinceUtc and ($modeRule)"
$select = 'flowsessionid,createdon,_createdby_value,runmode,triggertype,parentworkflowid,sessionusername,_regardingobjectid_value'
$sessionUri = "$dataverseUrl/api/data/v9.2/flowsessions?`$select=$select&`$filter=$([System.Uri]::EscapeDataString($filter))&`$orderby=createdon desc"

Write-Host "Querying Flow Sessions created since $sinceUtc UTC ..."
$sessions = @(Invoke-DataverseGetAll -Uri $sessionUri -Token $dataverseToken)
Write-Host "  Found $($sessions.Count) matching sessions."

$creatorIds = @($sessions | ForEach-Object { Get-PropertyValue -Row $_ -Column '_createdby_value' } | Where-Object { $_ } | Select-Object -Unique)
$creatorMap = @{}
if ($creatorIds.Count -gt 0) {
    Write-Host "Resolving $($creatorIds.Count) creators ..."
    for ($offset = 0; $offset -lt $creatorIds.Count; $offset += 50) {
        $lastIndex = [Math]::Min($offset + 49, $creatorIds.Count - 1)
        $creatorBatch = @($creatorIds[$offset..$lastIndex])
        $userFilter = ($creatorBatch | ForEach-Object { "systemuserid eq $_" }) -join ' or '
        $userUri = "$dataverseUrl/api/data/v9.2/systemusers?`$select=systemuserid,fullname,internalemailaddress,domainname&`$filter=$([System.Uri]::EscapeDataString($userFilter))"
        $users = @(Invoke-DataverseGetAll -Uri $userUri -Token $dataverseToken)
        foreach ($user in $users) {
            $creatorMap[[string]$user.systemuserid] = $user
        }
    }
}

$sessionCounts = @{}
foreach ($session in $sessions) {
    $creatorId = [string](Get-PropertyValue -Row $session -Column '_createdby_value')
    if (-not $sessionCounts.ContainsKey($creatorId)) { $sessionCounts[$creatorId] = 0 }
    $sessionCounts[$creatorId]++
}

$outputRows = foreach ($session in $sessions) {
    $creatorId = [string](Get-PropertyValue -Row $session -Column '_createdby_value')
    $creator = if ($creatorMap.ContainsKey($creatorId)) { $creatorMap[$creatorId] } else { $null }
    $creatorName = if ($creator) { $creator.fullname } else {
        Get-FormattedValue -Row $session -Column '_createdby_value' -Fallback '(unresolved user)'
    }
    $creatorEmail = if ($creator -and $creator.internalemailaddress) { $creator.internalemailaddress } elseif ($creator) { $creator.domainname } else { '' }
    $regardingId = [string](Get-PropertyValue -Row $session -Column '_regardingobjectid_value')
    $regardingName = Get-FormattedValue -Row $session -Column '_regardingobjectid_value' -Fallback $regardingId

    [pscustomobject][ordered]@{
        CreatedByName     = $creatorName
        CreatedByEmail    = $creatorEmail
        CreatedById       = $creatorId
        UserSessionCount  = $sessionCounts[$creatorId]
        FlowSessionId     = $session.flowsessionid
        CreatedOn         = ([datetime]$session.createdon).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
        RunMode           = Get-FormattedValue -Row $session -Column 'runmode' -Fallback (Get-PropertyValue -Row $session -Column 'runmode')
        TriggerType       = Get-FormattedValue -Row $session -Column 'triggertype' -Fallback (Get-PropertyValue -Row $session -Column 'triggertype')
        ParentWorkflowId  = Get-PropertyValue -Row $session -Column 'parentworkflowid' -Fallback ''
        SessionUserName   = Get-PropertyValue -Row $session -Column 'sessionusername' -Fallback ''
        Regarding         = $regardingName
        RegardingId       = $regardingId
    }
}

$outputDirectory = Split-Path -Parent $OutputPath
if ($outputDirectory -and -not (Test-Path -LiteralPath $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null
}

$csvRows = @(@($outputRows) | Sort-Object CreatedByName, CreatedByEmail, @{ Expression = 'CreatedOn'; Descending = $true })
if ($csvRows.Count -gt 0) {
    $csvRows | Export-Csv -LiteralPath $OutputPath -NoTypeInformation -Encoding UTF8
} else {
    '"CreatedByName","CreatedByEmail","CreatedById","UserSessionCount","FlowSessionId","CreatedOn","RunMode","TriggerType","ParentWorkflowId","SessionUserName","Regarding","RegardingId"' |
        Set-Content -LiteralPath $OutputPath -Encoding UTF8
}

$userCount = $creatorIds.Count
Write-Host "Exported $($sessions.Count) sessions for $userCount users to: $OutputPath" -ForegroundColor Green