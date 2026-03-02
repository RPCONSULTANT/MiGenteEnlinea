param(
    [ValidateSet("Local", "Staging", "Production")]
    [string]$EnvironmentName = "Production",
    [string]$ApiHealthUrl = "",
    [string]$WebUrl = "",
    [switch]$SkipE2E
)

$ErrorActionPreference = "Stop"

function Invoke-Step([string]$name, [scriptblock]$action) {
    Write-Host ""
    Write-Host "==> $name"
    & $action
}

Invoke-Step "Validate required env vars: API" {
    & (Join-Path $PSScriptRoot "Validate-RequiredEnv.ps1") -Component API
}

Invoke-Step "Validate required env vars: Web" {
    & (Join-Path $PSScriptRoot "Validate-RequiredEnv.ps1") -Component Web
}

if (-not $SkipE2E) {
    Invoke-Step "Validate required env vars: E2E" {
        & (Join-Path $PSScriptRoot "Validate-RequiredEnv.ps1") -Component E2E
    }
}

Invoke-Step "Generate env snapshot (names only)" {
    & (Join-Path $PSScriptRoot "Get-EnvSnapshot.ps1") -Component All
}

if (-not [string]::IsNullOrWhiteSpace($ApiHealthUrl)) {
    Invoke-Step "API health check: $ApiHealthUrl" {
        $res = Invoke-WebRequest -Uri $ApiHealthUrl -Method Get -UseBasicParsing -TimeoutSec 30
        if ($res.StatusCode -ne 200) {
            throw "API health failed. StatusCode=$($res.StatusCode)"
        }
    }
}

if (-not [string]::IsNullOrWhiteSpace($WebUrl)) {
    Invoke-Step "Web availability check: $WebUrl" {
        $res = Invoke-WebRequest -Uri $WebUrl -Method Get -UseBasicParsing -TimeoutSec 30
        if ($res.StatusCode -lt 200 -or $res.StatusCode -ge 400) {
            throw "Web check failed. StatusCode=$($res.StatusCode)"
        }
    }
}

if (-not $SkipE2E) {
    Invoke-Step "Run E2E smoke + summary" {
        $e2ePath = Join-Path $PSScriptRoot "..\..\tests\MiGenteEnLinea.E2E"
        Push-Location $e2ePath
        try {
            npm run test:e2e:smoke
            npm run report:summary
        } finally {
            Pop-Location
        }
    }
}

Write-Host ""
Write-Host "Pre-deploy checks completed for $EnvironmentName."
