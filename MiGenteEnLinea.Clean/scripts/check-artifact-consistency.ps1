param(
    [string]$Root = "."
)

$ErrorActionPreference = "Stop"

Push-Location $Root
try {
    $requiredMappings = @(
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/wwwroot/js/Custom.js"; Target = "artifacts/publish/Web/wwwroot/js/Custom.js" },
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/wwwroot/js/api-endpoints.js"; Target = "artifacts/publish/Web/wwwroot/js/api-endpoints.js" },
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/web.config"; Target = "artifacts/publish/Web/web.config" },
        @{ Source = "src/Presentation/MiGenteEnLinea.API/web.config"; Target = "artifacts/publish/API/web.config" },
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/appsettings.Production.json"; Target = "artifacts/publish/Web/appsettings.Production.json" },
        @{ Source = "src/Presentation/MiGenteEnLinea.API/appsettings.Production.json"; Target = "artifacts/publish/API/appsettings.Production.json" }
    )

    $optionalMappings = @(
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/Views/Shared/_Layout.cshtml"; Target = "artifacts/publish/Web/Views/Shared/_Layout.cshtml" },
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/Views/Shared/_LayoutAuth.cshtml"; Target = "artifacts/publish/Web/Views/Shared/_LayoutAuth.cshtml" },
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/Views/Shared/_LayoutLanding.cshtml"; Target = "artifacts/publish/Web/Views/Shared/_LayoutLanding.cshtml" },
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/Views/Shared/_LayoutEmpleador.cshtml"; Target = "artifacts/publish/Web/Views/Shared/_LayoutEmpleador.cshtml" },
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/Views/Shared/_LayoutContratista.cshtml"; Target = "artifacts/publish/Web/Views/Shared/_LayoutContratista.cshtml" },
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/Views/Auth/Activar.cshtml"; Target = "artifacts/publish/Web/Views/Auth/Activar.cshtml" },
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/Views/Auth/Login.cshtml"; Target = "artifacts/publish/Web/Views/Auth/Login.cshtml" },
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/Views/Auth/Registrar.cshtml"; Target = "artifacts/publish/Web/Views/Auth/Registrar.cshtml" },
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/Views/Contratista/Suscripciones.cshtml"; Target = "artifacts/publish/Web/Views/Contratista/Suscripciones.cshtml" },
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/Views/Empleador/Checkout.cshtml"; Target = "artifacts/publish/Web/Views/Empleador/Checkout.cshtml" },
        @{ Source = "src/Presentation/MiGenteEnLinea.Web/Views/Contratista/Checkout.cshtml"; Target = "artifacts/publish/Web/Views/Contratista/Checkout.cshtml" }
    )

    $failed = 0
    foreach ($map in $requiredMappings) {
        if (-not (Test-Path $map.Source)) {
            Write-Host "MISSING SOURCE: $($map.Source)" -ForegroundColor Red
            $failed++
            continue
        }
        if (-not (Test-Path $map.Target)) {
            Write-Host "MISSING TARGET: $($map.Target)" -ForegroundColor Red
            $failed++
            continue
        }

        $sourceHash = (Get-FileHash -Algorithm SHA256 $map.Source).Hash
        $targetHash = (Get-FileHash -Algorithm SHA256 $map.Target).Hash
        if ($sourceHash -ne $targetHash) {
            Write-Host "DIFF: $($map.Source) <> $($map.Target)" -ForegroundColor Red
            $failed++
        }
        else {
            Write-Host "OK: $($map.Source)" -ForegroundColor Green
        }
    }

    foreach ($map in $optionalMappings) {
        if (-not (Test-Path $map.Target)) {
            Write-Host "SKIP OPTIONAL TARGET (not published in this mode): $($map.Target)" -ForegroundColor Yellow
            continue
        }

        $sourceHash = (Get-FileHash -Algorithm SHA256 $map.Source).Hash
        $targetHash = (Get-FileHash -Algorithm SHA256 $map.Target).Hash
        if ($sourceHash -ne $targetHash) {
            Write-Host "OPTIONAL DIFF: $($map.Source) <> $($map.Target)" -ForegroundColor Yellow
        }
        else {
            Write-Host "OK OPTIONAL: $($map.Source)" -ForegroundColor Green
        }
    }

    if ($failed -gt 0) {
        throw "Artifact consistency failed with $failed mismatch(es). Re-run publish before deploy."
    }

    Write-Host "Artifact consistency check passed." -ForegroundColor Green
}
finally {
    Pop-Location
}
