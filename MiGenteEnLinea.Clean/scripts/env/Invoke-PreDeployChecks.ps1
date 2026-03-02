param(
    [ValidateSet("Local", "Staging", "Production")]
    [string]$EnvironmentName = "Production",
    [string]$ApiHealthUrl = "",
    [string]$ApiCorsProbeUrl = "",
    [string]$WebUrl = "",
    [switch]$SkipE2E
)

$ErrorActionPreference = "Stop"

function Invoke-Step([string]$name, [scriptblock]$action) {
    Write-Host ""
    Write-Host "==> $name"
    & $action
}

function Get-RequiredCorsOrigins {
    return @(
        "http://plattaformv2.migenteenlinea.do",
        "https://plattaformv2.migenteenlinea.do"
    )
}

function Get-ApiProductionSettingsPath {
    return Join-Path $PSScriptRoot "..\..\src\Presentation\MiGenteEnLinea.API\appsettings.Production.json"
}

function Get-ConfiguredCorsOriginsFromAppSettings([string]$settingsPath) {
    if (-not (Test-Path $settingsPath)) {
        throw "No se encontró appsettings.Production.json en: $settingsPath"
    }

    $json = Get-Content $settingsPath -Raw | ConvertFrom-Json
    $origins = @($json.CorsConfiguration.AllowedOrigins)
    if (-not $origins -or $origins.Count -eq 0) {
        throw "CorsConfiguration.AllowedOrigins está vacío en $settingsPath"
    }

    return $origins
}

function Get-ConfiguredCorsOriginsFromEnv {
    $prefix = "CorsConfiguration__AllowedOrigins__"
    $origins = Get-ChildItem Env: |
        Where-Object { $_.Name -like "$prefix*" -and -not [string]::IsNullOrWhiteSpace($_.Value) } |
        Sort-Object Name |
        ForEach-Object { $_.Value.Trim() }
    return @($origins)
}

function Assert-ContainsRequiredOrigins([string[]]$configuredOrigins, [string[]]$requiredOrigins, [string]$sourceName) {
    $configuredNormalized = @($configuredOrigins | ForEach-Object { $_.Trim().ToLowerInvariant() })
    $missing = @()
    foreach ($origin in $requiredOrigins) {
        if ($configuredNormalized -notcontains $origin.ToLowerInvariant()) {
            $missing += $origin
        }
    }

    if ($missing.Count -gt 0) {
        throw "$sourceName no contiene todos los orígenes críticos requeridos. Faltan: $($missing -join ', ')"
    }
}

function Resolve-CorsProbeUrl([string]$probeUrl, [string]$healthUrl) {
    if (-not [string]::IsNullOrWhiteSpace($probeUrl)) {
        return $probeUrl.Trim()
    }

    if ([string]::IsNullOrWhiteSpace($healthUrl)) {
        return ""
    }

    $uri = [Uri]$healthUrl
    $portPart = if (($uri.Scheme -eq "http" -and $uri.Port -eq 80) -or ($uri.Scheme -eq "https" -and $uri.Port -eq 443)) {
        ""
    } else {
        ":$($uri.Port)"
    }

    return "$($uri.Scheme)://$($uri.Host)$portPart/api/auth/login"
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

if ($EnvironmentName -eq "Production") {
    Invoke-Step "Validate runtime environment is Production" {
        $runtimeEnvironment = [string]$env:ASPNETCORE_ENVIRONMENT
        if ([string]::IsNullOrWhiteSpace($runtimeEnvironment)) {
            throw "ASPNETCORE_ENVIRONMENT no está definido en el entorno actual."
        }

        if (-not $runtimeEnvironment.Equals("Production", [System.StringComparison]::OrdinalIgnoreCase)) {
            throw "ASPNETCORE_ENVIRONMENT inválido para deploy productivo. Valor actual: '$runtimeEnvironment'. Debe ser 'Production'."
        }
    }

    Invoke-Step "Validate CORS critical origins (source-of-truth: appsettings.Production.json)" {
        $requiredOrigins = Get-RequiredCorsOrigins
        $settingsPath = Get-ApiProductionSettingsPath
        $fileOrigins = Get-ConfiguredCorsOriginsFromAppSettings -settingsPath $settingsPath
        Assert-ContainsRequiredOrigins -configuredOrigins $fileOrigins -requiredOrigins $requiredOrigins -sourceName "appsettings.Production.json"

        $envOrigins = Get-ConfiguredCorsOriginsFromEnv
        if ($envOrigins.Count -gt 0) {
            Assert-ContainsRequiredOrigins -configuredOrigins $envOrigins -requiredOrigins $requiredOrigins -sourceName "Variables de entorno CorsConfiguration__AllowedOrigins__*"
            Write-Host "CORS env overrides detectados y válidos para orígenes críticos."
        } else {
            Write-Host "Sin overrides CORS en variables de entorno. Se usará appsettings.Production.json."
        }
    }
}

if (-not [string]::IsNullOrWhiteSpace($ApiHealthUrl)) {
    Invoke-Step "API health check: $ApiHealthUrl" {
        $res = Invoke-WebRequest -Uri $ApiHealthUrl -Method Get -UseBasicParsing -TimeoutSec 30
        if ($res.StatusCode -ne 200) {
            throw "API health failed. StatusCode=$($res.StatusCode)"
        }
    }
}

$resolvedCorsProbeUrl = Resolve-CorsProbeUrl -probeUrl $ApiCorsProbeUrl -healthUrl $ApiHealthUrl
if (-not [string]::IsNullOrWhiteSpace($resolvedCorsProbeUrl)) {
    Invoke-Step "CORS preflight checks: $resolvedCorsProbeUrl" {
        $requiredOrigins = Get-RequiredCorsOrigins

        foreach ($origin in $requiredOrigins) {
            $headers = @{
                "Origin" = $origin
                "Access-Control-Request-Method" = "POST"
                "Access-Control-Request-Headers" = "content-type,authorization"
            }

            $res = Invoke-WebRequest -Uri $resolvedCorsProbeUrl -Method Options -Headers $headers -UseBasicParsing -TimeoutSec 30
            $acao = [string]$res.Headers["Access-Control-Allow-Origin"]
            $acam = [string]$res.Headers["Access-Control-Allow-Methods"]

            if ([string]::IsNullOrWhiteSpace($acao)) {
                throw "Preflight sin Access-Control-Allow-Origin para Origin '$origin'."
            }

            if (-not $acao.Equals($origin, [System.StringComparison]::OrdinalIgnoreCase)) {
                throw "Access-Control-Allow-Origin inesperado para Origin '$origin'. Valor recibido: '$acao'."
            }

            if ([string]::IsNullOrWhiteSpace($acam) -or ($acam -notmatch "(?i)\bPOST\b")) {
                throw "Access-Control-Allow-Methods no incluye POST para Origin '$origin'. Valor recibido: '$acam'."
            }
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
