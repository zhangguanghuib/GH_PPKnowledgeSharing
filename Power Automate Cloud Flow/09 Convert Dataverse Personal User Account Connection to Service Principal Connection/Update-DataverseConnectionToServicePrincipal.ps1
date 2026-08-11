[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'High')]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9a-fA-F-]{36}$')]
    [string]$EnvironmentId,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9a-fA-F]{32}$')]
    [string]$ConnectionId,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9a-fA-F-]{36}$')]
    [string]$TenantId,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[0-9a-fA-F-]{36}$')]
    [string]$ApplicationId,

    [Parameter()]
    [System.Security.SecureString]$ClientSecret
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not (Get-Command pac -ErrorAction SilentlyContinue)) {
    throw 'Power Platform CLI (pac) is not installed or is not available on PATH.'
}

Write-Host 'Current PAC authentication profile:' -ForegroundColor Cyan
& pac auth who
if ($LASTEXITCODE -ne 0) {
    throw "PAC authentication is unavailable. Run 'pac auth create --environment $EnvironmentId' as an administrator who can edit this connection."
}

Write-Host "`nConnections visible in environment $EnvironmentId`:" -ForegroundColor Cyan
$connections = @(& pac connection list --environment $EnvironmentId 2>&1)
if ($LASTEXITCODE -ne 0) {
    throw "Unable to list connections: $($connections -join [Environment]::NewLine)"
}
$connections | ForEach-Object { Write-Host $_ }

if (($connections -join [Environment]::NewLine) -notmatch [regex]::Escape($ConnectionId)) {
    throw "Connection '$ConnectionId' is not visible to the current PAC account in environment '$EnvironmentId'."
}

$target = "Dataverse connection $ConnectionId in environment $EnvironmentId"
if (-not $PSCmdlet.ShouldProcess($target, "Replace its authentication with service principal $ApplicationId")) {
    return
}

if ($null -eq $ClientSecret) {
    $ClientSecret = Read-Host 'Enter the NEW client secret (the exposed secret must be revoked)' -AsSecureString
}

$secretPointer = [IntPtr]::Zero
$plainTextSecret = $null

try {
    $secretPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($ClientSecret)
    $plainTextSecret = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($secretPointer)

    if ([string]::IsNullOrWhiteSpace($plainTextSecret)) {
        throw 'Client secret cannot be empty.'
    }

    Write-Host "`nUpdating $target..." -ForegroundColor Cyan
    & pac connection update `
        --environment $EnvironmentId `
        --connection-id $ConnectionId `
        --tenant-id $TenantId `
        --application-id $ApplicationId `
        --client-secret $plainTextSecret

    if ($LASTEXITCODE -ne 0) {
        throw "PAC connection update failed with exit code $LASTEXITCODE."
    }

    Write-Host "`nConnection update completed. Current connection listing:" -ForegroundColor Green
    & pac connection list --environment $EnvironmentId
    if ($LASTEXITCODE -ne 0) {
        throw 'The update succeeded, but the post-update connection listing failed.'
    }
}
finally {
    if ($secretPointer -ne [IntPtr]::Zero) {
        [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($secretPointer)
    }
    $plainTextSecret = $null
    $ClientSecret = $null
}