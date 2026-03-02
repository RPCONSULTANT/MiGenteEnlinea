param(
    [string]$WebBaseUrl = "http://plattaformv2.migenteenlinea.do",
    [string]$ApiBaseUrl = "http://api2.migenteenlinea.do",
    [string]$EmpleadorEmail = "Rainierymc05@gmail.com",
    [string]$EmpleadorPassword = "Ray1234@",
    [string]$ContratistaEmail = "peverti00@gmail.com",
    [string]$ContratistaPassword = "Ray1234@",
    [string]$AdminEmail = "Rainierymc05@gmail.com",
    [string]$AdminPassword = "Ray1234@",
    [string]$SeedKey = "seed-placeholder",
    [string]$AllowWrite = "true",
    [string]$StrictRuntimeIssues = "true",
    [string]$RunId = "",
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$PlaywrightArgs
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($RunId)) {
    $RunId = "prod_run_{0}" -f (Get-Date -Format "yyyyMMdd_HHmmss")
}

$envValues = @{
    "E2E_WEB_BASE_URL"              = $WebBaseUrl
    "E2E_API_BASE_URL"              = $ApiBaseUrl
    "E2E_ALLOW_WRITE"               = $AllowWrite
    "E2E_RUN_ID"                    = $RunId
    "E2E_SEED_KEY"                  = $SeedKey
    "E2E_STRICT_RUNTIME_ISSUES"     = $StrictRuntimeIssues

    "E2E_USER_EMPLEADOR_EMAIL"      = $EmpleadorEmail
    "E2E_USER_EMPLEADOR_PASSWORD"   = $EmpleadorPassword
    "E2E_EMAIL_EMPLEADOR"           = $EmpleadorEmail
    "E2E_PASSWORD_EMPLEADOR"        = $EmpleadorPassword

    "E2E_USER_CONTRATISTA_EMAIL"    = $ContratistaEmail
    "E2E_USER_CONTRATISTA_PASSWORD" = $ContratistaPassword
    "E2E_EMAIL_CONTRATISTA"         = $ContratistaEmail
    "E2E_PASSWORD_CONTRATISTA"      = $ContratistaPassword

    "E2E_USER_ADMIN_EMAIL"          = $AdminEmail
    "E2E_USER_ADMIN_PASSWORD"       = $AdminPassword
    "E2E_EMAIL_ADMIN"               = $AdminEmail
    "E2E_PASSWORD_ADMIN"            = $AdminPassword
}

# Persist to user scope and current process so every run is prefilled.
foreach ($entry in $envValues.GetEnumerator()) {
    [System.Environment]::SetEnvironmentVariable($entry.Key, $entry.Value, "User")
    Set-Item -Path ("Env:{0}" -f $entry.Key) -Value $entry.Value
}

Write-Host "[E2E] Environment variables loaded."
Write-Host ("[E2E] RunId: {0}" -f $RunId)
Write-Host ("[E2E] AllowWrite: {0}" -f $env:E2E_ALLOW_WRITE)

$validateScript = Join-Path $PSScriptRoot "Validate-RequiredEnv.ps1"
& $validateScript -Component E2E
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

$testExitCode = 0
try {
    $npmArgs = @("run", "test:e2e:all")
    if ($PlaywrightArgs -and $PlaywrightArgs.Count -gt 0) {
        $npmArgs += "--"
        $npmArgs += $PlaywrightArgs
    }

    Write-Host ("[E2E] Executing: npm {0}" -f ($npmArgs -join " "))
    & npm @npmArgs
    $testExitCode = $LASTEXITCODE
}
finally {
    Write-Host "[E2E] Generating summary artifacts..."
    npm run report:summary
}

exit $testExitCode
