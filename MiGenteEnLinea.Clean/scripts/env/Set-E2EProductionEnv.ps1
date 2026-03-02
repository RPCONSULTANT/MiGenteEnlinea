param(
    [Parameter(Mandatory = $true)]
    [string]$EmpleadorEmail,
    [Parameter(Mandatory = $true)]
    [string]$EmpleadorPassword,
    [Parameter(Mandatory = $true)]
    [string]$ContratistaEmail,
    [Parameter(Mandatory = $true)]
    [string]$ContratistaPassword,
    [Parameter(Mandatory = $true)]
    [string]$AdminEmail,
    [Parameter(Mandatory = $true)]
    [string]$AdminPassword,
    [Parameter(Mandatory = $true)]
    [string]$SeedKey,
    [string]$WebBaseUrl = "http://plattaformv2.migenteenlinea.do",
    [string]$ApiBaseUrl = "http://api2.migenteenlinea.do",
    [string]$RunId = ""
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($RunId)) {
    $RunId = "prod_run_{0}" -f (Get-Date -Format "yyyyMMdd_HHmmss")
}

$values = @{
    "E2E_WEB_BASE_URL"                = $WebBaseUrl
    "E2E_API_BASE_URL"                = $ApiBaseUrl
    "E2E_ALLOW_WRITE"                 = "true"
    "E2E_RUN_ID"                      = $RunId
    "E2E_SEED_KEY"                    = $SeedKey
    "E2E_STRICT_RUNTIME_ISSUES"       = "true"

    "E2E_USER_EMPLEADOR_EMAIL"        = $EmpleadorEmail
    "E2E_USER_EMPLEADOR_PASSWORD"     = $EmpleadorPassword
    "E2E_EMAIL_EMPLEADOR"             = $EmpleadorEmail
    "E2E_PASSWORD_EMPLEADOR"          = $EmpleadorPassword

    "E2E_USER_CONTRATISTA_EMAIL"      = $ContratistaEmail
    "E2E_USER_CONTRATISTA_PASSWORD"   = $ContratistaPassword
    "E2E_EMAIL_CONTRATISTA"           = $ContratistaEmail
    "E2E_PASSWORD_CONTRATISTA"        = $ContratistaPassword

    "E2E_USER_ADMIN_EMAIL"            = $AdminEmail
    "E2E_USER_ADMIN_PASSWORD"         = $AdminPassword
    "E2E_EMAIL_ADMIN"                 = $AdminEmail
    "E2E_PASSWORD_ADMIN"              = $AdminPassword
}

foreach ($entry in $values.GetEnumerator()) {
    [System.Environment]::SetEnvironmentVariable($entry.Key, $entry.Value, "User")
    Set-Item -Path ("Env:{0}" -f $entry.Key) -Value $entry.Value
}

Write-Host "E2E production environment variables configured (User scope + current session)."
Write-Host "RunId: $RunId"
Write-Host ""
Write-Host "Validation command:"
Write-Host ".\\scripts\\env\\Validate-RequiredEnv.ps1 -Component E2E"
