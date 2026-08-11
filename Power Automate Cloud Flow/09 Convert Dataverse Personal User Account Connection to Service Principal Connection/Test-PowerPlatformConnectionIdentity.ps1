[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9a-fA-F-]{36}$')]
    [string]$TenantId,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9a-fA-F-]{36}$')]
    [string]$EnvironmentId,

    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$ConnectionId
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-OptionalPropertyValue {
    param(
        [AllowNull()]
        [object]$InputObject,

        [Parameter(Mandatory = $true)]
        [string]$Name
    )

    if ($null -eq $InputObject) {
        return $null
    }

    if ($InputObject -is [System.Collections.IDictionary]) {
        if ($InputObject.Contains($Name)) {
            return $InputObject[$Name]
        }
        return $null
    }

    $property = $InputObject.PSObject.Properties[$Name]
    if ($null -ne $property) {
        return $property.Value
    }

    return $null
}

function Get-ConnectionParameterValue {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Properties,

        [Parameter(Mandatory = $true)]
        [string]$Name
    )

    $parameters = Get-OptionalPropertyValue -InputObject $Properties -Name 'connectionParameters'
    $value = Get-OptionalPropertyValue -InputObject $parameters -Name $Name
    if ($null -ne $value) {
        return [string]$value
    }

    $parameterSet = Get-OptionalPropertyValue -InputObject $Properties -Name 'connectionParametersSet'
    $values = Get-OptionalPropertyValue -InputObject $parameterSet -Name 'values'
    $entry = Get-OptionalPropertyValue -InputObject $values -Name $Name
    $value = Get-OptionalPropertyValue -InputObject $entry -Name 'value'
    if ($null -ne $value) {
        return [string]$value
    }

    return $null
}

if (-not (Get-Command Get-AdminPowerAppConnection -ErrorAction SilentlyContinue)) {
    throw "Get-AdminPowerAppConnection is unavailable. Install the Microsoft.PowerApps.Administration.PowerShell module."
}

try {
    $matches = @(Get-AdminPowerAppConnection -EnvironmentName $EnvironmentId -ErrorAction Stop |
        Where-Object { $_.ConnectionName -eq $ConnectionId })
}
catch {
    throw "Unable to query connections. Run 'Add-PowerAppsAccount' with an environment administrator account, then retry. $($_.Exception.Message)"
}

if ($matches.Count -eq 0) {
    throw "Connection '$ConnectionId' was not found in environment '$EnvironmentId', or the current account cannot view it."
}

if ($matches.Count -gt 1) {
    throw "More than one connection matched '$ConnectionId'; classification is ambiguous."
}

$connection = $matches[0]
$properties = Get-OptionalPropertyValue -InputObject $connection.Internal -Name 'properties'
$grantType = Get-ConnectionParameterValue -Properties $properties -Name 'token:grantType'
$applicationId = Get-ConnectionParameterValue -Properties $properties -Name 'token:clientId'
$authenticationTenantId = Get-ConnectionParameterValue -Properties $properties -Name 'token:TenantId'
$parameterSet = Get-OptionalPropertyValue -InputObject $properties -Name 'connectionParametersSet'
$parameterSetName = Get-OptionalPropertyValue -InputObject $parameterSet -Name 'name'

$createdBy = Get-OptionalPropertyValue -InputObject $properties -Name 'createdBy'
$createdByTenantId = Get-OptionalPropertyValue -InputObject $createdBy -Name 'tenantId'
$effectiveTenantId = if (-not [string]::IsNullOrWhiteSpace($authenticationTenantId)) {
    $authenticationTenantId
}
else {
    $createdByTenantId
}

$classification = 'Unknown'
$evidence = 'The connection metadata does not expose a recognized authentication mode.'

if ($grantType -eq 'client_credentials' -or $parameterSetName -match 'ServicePrincipal') {
    $classification = 'ServicePrincipal'
    $evidence = "token:grantType='$grantType'; parameter set='$parameterSetName'."
}
elseif (-not [string]::IsNullOrWhiteSpace($grantType)) {
    $classification = 'UserAccount'
    $evidence = "token:grantType='$grantType', which is not client_credentials."
}
elseif ($null -ne $createdBy -and [string]::IsNullOrWhiteSpace($applicationId)) {
    $classification = 'UserAccount'
    $evidence = 'No service-principal client ID or client_credentials grant was found.'
}

$tenantMatches = $null
if (-not [string]::IsNullOrWhiteSpace($effectiveTenantId)) {
    $tenantMatches = $effectiveTenantId -eq $TenantId
}

$result = [PSCustomObject]@{
    Classification         = $classification
    DisplayName            = $connection.DisplayName
    ConnectionId           = $connection.ConnectionName
    ConnectorName          = $connection.ConnectorName
    EnvironmentId          = $connection.EnvironmentName
    SuppliedTenantId       = $TenantId
    AuthenticationTenantId = $authenticationTenantId
    TenantMatches          = $tenantMatches
    ApplicationId          = $applicationId
    GrantType              = $grantType
    ParameterSet           = $parameterSetName
    Status                 = (@($connection.Statuses | ForEach-Object { $_.status }) -join ', ')
    Evidence               = $evidence
}

$result | Format-List

if ($tenantMatches -eq $false) {
    Write-Warning "The connection metadata tenant '$effectiveTenantId' does not match supplied tenant '$TenantId'."
}

if ($classification -eq 'Unknown') {
    exit 2
}