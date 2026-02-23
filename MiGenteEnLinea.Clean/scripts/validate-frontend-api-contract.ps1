param(
    [string]$Root = "."
)

$ErrorActionPreference = "Stop"

function Assert-NoMatches {
    param(
        [string]$Name,
        [string]$Pattern,
        [string[]]$Paths
    )

    $matches = @()
    foreach ($path in $Paths) {
        if (Test-Path $path) {
            $out = rg $Pattern $path -n 2>$null
            if ($LASTEXITCODE -eq 0 -and $out) {
                $matches += $out
            }
        }
    }

    if ($matches.Count -gt 0) {
        Write-Host "FAIL: $Name" -ForegroundColor Red
        $matches | Select-Object -First 50 | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
        throw "$Name validation failed."
    }

    Write-Host "OK: $Name" -ForegroundColor Green
}

function Assert-HasMatches {
    param(
        [string]$Name,
        [string]$Pattern,
        [string[]]$Paths
    )

    foreach ($path in $Paths) {
        if (Test-Path $path) {
            rg $Pattern $path -n 2>$null | Out-Null
            if ($LASTEXITCODE -eq 0) {
                Write-Host "OK: $Name" -ForegroundColor Green
                return
            }
        }
    }

    throw "Missing expected contract: $Name."
}

Push-Location $Root
try {
    $webSrc = "src/Presentation/MiGenteEnLinea.Web"
    $criticalViews = @(
        "$webSrc/Views/Auth",
        "$webSrc/Views/Empleador/AdquirirPlan.cshtml",
        "$webSrc/Views/Empleador/Checkout.cshtml",
        "$webSrc/Views/Contratista/AdquirirPlan.cshtml",
        "$webSrc/Views/Contratista/Checkout.cshtml",
        "$webSrc/Views/Contratista/Suscripciones.cshtml"
    )

    Assert-NoMatches -Name "No localhost hardcodes in frontend views" `
        -Pattern "http://localhost:5015" `
        -Paths @("$webSrc/Views")

    Assert-NoMatches -Name "No legacy suscripciones route patterns" `
        -Pattern "/suscripciones/(usuario|ventas/usuario)/" `
        -Paths @($webSrc)

    Assert-NoMatches -Name "No legacy empleadores/perfil route usage" `
        -Pattern "/empleadores/perfil/" `
        -Paths @($webSrc)

    Assert-NoMatches -Name "No legacy empleados activos query param" `
        -Pattern "activos=true" `
        -Paths @("$webSrc/Views")

    Assert-NoMatches -Name "No direct response.json await in views" `
        -Pattern "=\s*await\s*response\.json\(" `
        -Paths @("$webSrc/Views")

    Assert-NoMatches -Name "No direct response.json then-chain in views" `
        -Pattern "then\(\s*response\s*=>\s*response\.json\(" `
        -Paths @("$webSrc/Views")

    Assert-NoMatches -Name "No legacy consultar-padron route usage" `
        -Pattern "/empleados/consultar-padron/" `
        -Paths @($webSrc)

    Assert-HasMatches -Name "API endpoints catalog included" `
        -Pattern "window\.API_ENDPOINTS" `
        -Paths @("$webSrc/wwwroot/js/api-endpoints.js")

    Assert-HasMatches -Name "Endpoint catalog includes contratos/dashboard/catalogos modules" `
        -Pattern "CONTRATACIONES|DASHBOARD|CATALOGOS|CALIFICACIONES|UTILITARIOS|NOMINAS" `
        -Paths @("$webSrc/wwwroot/js/api-endpoints.js")

    Assert-HasMatches -Name "Custom.js exposes requestApi helper" `
        -Pattern "window\.requestApi" `
        -Paths @("$webSrc/wwwroot/js/Custom.js")

    Assert-HasMatches -Name "Critical views use readApiResponse fallback path" `
        -Pattern "window\.readApiResponse" `
        -Paths $criticalViews

    Write-Host ""
    Write-Host "Frontend/API contract validation passed." -ForegroundColor Green
}
finally {
    Pop-Location
}
