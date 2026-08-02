[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ValidatePattern('^[0-9a-fA-F-]{36}$')]
    [string]$TenantId,

    [datetime]$StartTime = (Get-Date).ToUniversalTime().AddDays(-30),

    [datetime]$EndTime = (Get-Date).ToUniversalTime(),

    [ValidateRange(1, 1000)]
    [int]$PageSize = 1000,

    [ValidateRange(1, 10000)]
    [int]$MaximumPages = 1000,

    [string]$LicenseClassification = 'Power Automate per user with attended RPA',

    [string]$AccessToken = $env:POWER_PLATFORM_LICENSING_TOKEN,

    [string]$GraphAccessToken = $env:MICROSOFT_GRAPH_TOKEN,

    [string]$CsvPath = (Join-Path $PWD 'premium-flow-active-users.csv'),

    [string]$DetailsCsvPath = (Join-Path $PWD 'premium-flow-active-user-details.csv'),

    [string]$UserMappingCsvPath = (Join-Path $PWD 'premium-flow-user-email-mapping.csv'),

    [string]$RawJsonPath = (Join-Path $PWD 'premium-flow-active-users.raw.json')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$resource = 'https://licensing.powerplatform.microsoft.com'
$uri = "$resource/v1.0/tenants/$TenantId/ManagedEnvironment/PowerAutomate/GetUslLicenseDetails"

function ConvertFrom-JwtPayload {
    param([Parameter(Mandatory)][string]$Token)

    $segments = $Token.Split('.')
    if ($segments.Count -ne 3) {
        return $null
    }

    try {
        $payload = $segments[1].Replace('-', '+').Replace('_', '/')
        $payload = $payload.PadRight($payload.Length + ((4 - ($payload.Length % 4)) % 4), '=')
        $json = [Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($payload))
        return $json | ConvertFrom-Json
    }
    catch {
        return $null
    }
}

function ConvertFrom-UnixMicroseconds {
    param($Value)

    if ($null -eq $Value -or [string]::IsNullOrWhiteSpace([string]$Value)) {
        return $null
    }

    $microseconds = [long]$Value
    $remainingMicroseconds = [long]0
    $milliseconds = [Math]::DivRem($microseconds, 1000, [ref]$remainingMicroseconds)
    return [DateTimeOffset]::FromUnixTimeMilliseconds($milliseconds).
        AddTicks($remainingMicroseconds * 10).
        UtcDateTime.ToString('yyyy-MM-ddTHH:mm:ss.ffffffZ')
}

function Get-LicensingAccessToken {
    if (-not [string]::IsNullOrWhiteSpace($AccessToken)) {
        return $AccessToken.Trim()
    }

    if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
        throw "Azure CLI was not found. Install it, run 'az login --tenant $TenantId', or set POWER_PLATFORM_LICENSING_TOKEN."
    }

    $token = & az account get-access-token `
        --tenant $TenantId `
        --resource $resource `
        --query accessToken `
        --output tsv 2>&1

    if ($LASTEXITCODE -ne 0) {
        throw "Azure CLI could not acquire a licensing token: $($token -join [Environment]::NewLine)"
    }

    return ($token -join '').Trim()
}

function Get-GraphAccessToken {
    if (-not [string]::IsNullOrWhiteSpace($GraphAccessToken)) {
        return $GraphAccessToken.Trim()
    }

    if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
        throw "Azure CLI was not found. Install it, run 'az login --tenant $TenantId --allow-no-subscriptions', or set MICROSOFT_GRAPH_TOKEN."
    }

    $graphToken = & az account get-access-token `
        --tenant $TenantId `
        --resource 'https://graph.microsoft.com' `
        --query accessToken `
        --output tsv 2>&1

    if ($LASTEXITCODE -ne 0) {
        throw "Azure CLI could not acquire a Microsoft Graph token: $($graphToken -join [Environment]::NewLine)"
    }

    return ($graphToken -join '').Trim()
}

function Get-UserEmailMappings {
    param(
        [Parameter(Mandatory)][string[]]$UserIds,
        [Parameter(Mandatory)][string]$Token
    )

    $graphHeaders = @{
        Accept = 'application/json'
        Authorization = "Bearer $Token"
    }

    foreach ($userId in $UserIds) {
        $encodedUserId = [Uri]::EscapeDataString($userId)
        $userUri = "https://graph.microsoft.com/v1.0/users/$encodedUserId`?`$select=id,mail,userPrincipalName"
        Write-Verbose "Resolving user $userId through Microsoft Graph"

        try {
            $user = Invoke-RestMethod -Uri $userUri -Method Get -Headers $graphHeaders
            $userEmail = if (-not [string]::IsNullOrWhiteSpace($user.mail)) {
                $user.mail
            }
            else {
                $user.userPrincipalName
            }

            [pscustomobject]@{
                UserId = $user.id
                UserEmail = $userEmail
                UserPrincipalName = $user.userPrincipalName
            }
        }
        catch {
            $statusCode = 'unknown'
            if ($null -ne $_.Exception.Response -and $null -ne $_.Exception.Response.StatusCode) {
                $statusCode = [int]$_.Exception.Response.StatusCode
            }

            if ($statusCode -eq 404) {
                Write-Warning "User '$userId' no longer exists in Microsoft Entra ID. Its email will be blank."
                [pscustomobject]@{
                    UserId = $userId
                    UserEmail = $null
                    UserPrincipalName = $null
                }
                continue
            }

            throw "Microsoft Graph user lookup failed with HTTP $statusCode. The signed-in account needs permission to read basic user profiles. $($_.Exception.Message)"
        }
    }
}

function Test-LicensingAccessToken {
    param([Parameter(Mandatory)][string]$Token)

    $payload = ConvertFrom-JwtPayload -Token $Token
    if ($null -eq $payload) {
        Write-Warning 'The access token could not be decoded locally; the service will validate it.'
        return
    }

    if ($payload.PSObject.Properties.Name -contains 'exp') {
        $expiresAt = [DateTimeOffset]::FromUnixTimeSeconds([long]$payload.exp)
        if ($expiresAt -le [DateTimeOffset]::UtcNow) {
            throw "The access token expired at $($expiresAt.UtcDateTime.ToString('o'))."
        }
    }

    if (($payload.PSObject.Properties.Name -contains 'tid') -and $payload.tid -ne $TenantId) {
        throw "The access token belongs to tenant '$($payload.tid)', not '$TenantId'."
    }

    if ($payload.PSObject.Properties.Name -contains 'aud') {
        $audience = $payload.aud.TrimEnd('/')
        if ($audience -ne $resource) {
            throw "The access token audience is '$($payload.aud)', not '$resource'."
        }
    }
}

function Get-ResponseItems {
    param([Parameter(Mandatory)]$Response)

    if ($Response -is [array]) {
        return $Response
    }

    foreach ($propertyName in @('records', 'value', 'items', 'results', 'licenseDetails', 'users')) {
        if ($Response.PSObject.Properties.Name -contains $propertyName) {
            return @($Response.$propertyName)
        }
    }

    if ($Response.PSObject.Properties.Name -contains 'data') {
        $data = $Response.data
        if ($null -eq $data) {
            throw 'The response data property was null.'
        }
        if ($data -is [array]) {
            return $data
        }

        foreach ($propertyName in @('records', 'value', 'items', 'results', 'licenseDetails', 'users')) {
            if ($data.PSObject.Properties.Name -contains $propertyName) {
                return @($data.$propertyName)
            }
        }
    }

    throw 'The response item collection was not recognized. Inspect the raw response or update Get-ResponseItems for the current API schema.'
}

if ($StartTime.ToUniversalTime() -ge $EndTime.ToUniversalTime()) {
    throw 'StartTime must be earlier than EndTime.'
}

$token = Get-LicensingAccessToken
Test-LicensingAccessToken -Token $token

$headers = @{
    Accept = 'application/json'
    Authorization = "Bearer $token"
    'x-ms-client-tenant-id' = $TenantId
}

$allItems = [Collections.Generic.List[object]]::new()
$rawPages = [Collections.Generic.List[object]]::new()
$continuationToken = ''

for ($pageNumber = 0; $pageNumber -lt $MaximumPages; $pageNumber++) {
    $requestBody = @{
        isTenantLevel = $true
        environmentId = $null
        pageNumber = $pageNumber
        pageSize = $PageSize
        startTime = $StartTime.ToUniversalTime().ToString('o')
        endTime = $EndTime.ToUniversalTime().ToString('o')
        activeUsersOnly = $true
        continuationToken = $continuationToken
        licenseClassification = $LicenseClassification
    } | ConvertTo-Json

    Write-Verbose "Requesting page $pageNumber from $uri"
    try {
        $response = Invoke-RestMethod -Uri $uri -Method Post -Headers $headers `
            -ContentType 'application/json' -Body $requestBody
    }
    catch {
        $statusCode = 'unknown'
        if ($null -ne $_.Exception.Response -and $null -ne $_.Exception.Response.StatusCode) {
            $statusCode = [int]$_.Exception.Response.StatusCode
        }
        throw "Licensing API request failed with HTTP $statusCode. The API is undocumented and requires an authorized Power Platform admin account. $($_.Exception.Message)"
    }

    $rawPages.Add($response)
    $pageItems = @(Get-ResponseItems -Response $response)
    foreach ($item in $pageItems) {
        $allItems.Add($item)
    }

    $nextToken = ''
    foreach ($propertyName in @('continuationToken', 'nextContinuationToken', 'nextToken')) {
        if (($response.PSObject.Properties.Name -contains $propertyName) -and $response.$propertyName) {
            $nextToken = [string]$response.$propertyName
            break
        }
        if (($response.PSObject.Properties.Name -contains 'data') -and
            ($response.data.PSObject.Properties.Name -contains $propertyName) -and $response.data.$propertyName) {
            $nextToken = [string]$response.data.$propertyName
            break
        }
    }

    if ($nextToken) {
        if ($nextToken -eq $continuationToken) {
            throw 'The API returned the same continuation token twice; pagination was stopped to avoid an infinite loop.'
        }
        $continuationToken = $nextToken
    }
    elseif ($pageItems.Count -lt $PageSize) {
        break
    }
    else {
        throw "The API returned a full page of $PageSize records without a continuation token. It ignores pageNumber, so continuing would duplicate records."
    }
}

if ($pageNumber -eq $MaximumPages) {
    throw "Stopped after MaximumPages ($MaximumPages). Increase the limit if more pages are expected."
}

$filteredItems = @($allItems | Where-Object {
    [string]$_.isPremiumFlow -eq 'True' -and [int]$_.hasPremiumFeatures -eq 1
})

$userIds = @($filteredItems.userId | Where-Object { $_ } | Sort-Object -Unique)
$userMappings = @(Get-UserEmailMappings -UserIds $userIds -Token (Get-GraphAccessToken))
$emailByUserId = @{}
foreach ($mapping in $userMappings) {
    $emailByUserId[$mapping.UserId] = $mapping.UserEmail
}

$detailRecords = @($filteredItems | ForEach-Object {
    $record = $_ | Select-Object *
    $record.lastRunDate = ConvertFrom-UnixMicroseconds $record.lastRunDate
    $record.createdDate = ConvertFrom-UnixMicroseconds $record.createdDate
    $record.modifiedDate = ConvertFrom-UnixMicroseconds $record.modifiedDate
    $record | Select-Object *, @{
        Name = 'userEmail'
        Expression = { $emailByUserId[[string]$_.userId] }
    }
})

$activeUsers = @($filteredItems | Group-Object -Property userId | ForEach-Object {
    $userRecords = @($_.Group)
    [pscustomobject]@{
        UserId = $_.Name
        UserEmail = $emailByUserId[$_.Name]
        FlowCount = @($userRecords.flowId | Sort-Object -Unique).Count
        EnvironmentCount = @($userRecords.environmentId | Where-Object { $_ } | Sort-Object -Unique).Count
        LatestRunDate = ConvertFrom-UnixMicroseconds ($userRecords.lastRunDate | Where-Object { $_ } | Sort-Object -Descending | Select-Object -First 1)
    }
} | Sort-Object -Property UserId)

$rawPages | ConvertTo-Json -Depth 30 | Set-Content -LiteralPath $RawJsonPath -Encoding UTF8
$detailRecords | Export-Csv -LiteralPath $DetailsCsvPath -NoTypeInformation -Encoding UTF8
$activeUsers | Export-Csv -LiteralPath $CsvPath -NoTypeInformation -Encoding UTF8
$userMappings | Sort-Object -Property UserId | Export-Csv -LiteralPath $UserMappingCsvPath -NoTypeInformation -Encoding UTF8

Write-Host "Retrieved $($activeUsers.Count) active user(s) across $($filteredItems.Count) premium flow record(s)."
Write-Host "Users CSV:   $CsvPath"
Write-Host "Details CSV: $DetailsCsvPath"
Write-Host "User mapping: $UserMappingCsvPath"
Write-Host "Raw JSON:    $RawJsonPath"
$activeUsers