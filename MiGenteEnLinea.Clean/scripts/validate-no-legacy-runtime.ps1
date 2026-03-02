param(
    [string]$RepoRoot = "."
)

$ErrorActionPreference = "Stop"

function Test-NoMatches {
    param(
        [string]$Description,
        [string]$Pattern,
        [string[]]$Paths
    )

    $matches = & rg -n --glob "!**/bin/**" --glob "!**/obj/**" --glob "!**/Migrations/**" $Pattern @Paths
    if ($LASTEXITCODE -eq 0 -and $matches) {
        Write-Host "FAILED: $Description" -ForegroundColor Red
        $matches | ForEach-Object { Write-Host $_ -ForegroundColor Red }
        return $false
    }

    Write-Host "OK: $Description" -ForegroundColor Green
    return $true
}

Push-Location $RepoRoot
try {
    $allOk = $true

    $allOk = (Test-NoMatches `
        -Description "No Generated entities in Core/Presentation runtime" `
        -Pattern "Infrastructure\.Persistence\.Entities\.Generated|Generated\." `
        -Paths @("src/Core", "src/Presentation")) -and $allOk

    $allOk = (Test-NoMatches `
        -Description "No SqlQueryRaw/ExecuteSqlRaw in Core/Presentation runtime" `
        -Pattern "SqlQueryRaw|ExecuteSqlRaw" `
        -Paths @("src/Core", "src/Presentation")) -and $allOk

    $allowedInfraPaths = @(
        "src/Infrastructure/MiGenteEnLinea.Infrastructure/Persistence/Migrations",
        "src/Infrastructure/MiGenteEnLinea.Infrastructure/Persistence/Seeding"
    )

    $infraMatches = & rg -n --glob "!**/bin/**" --glob "!**/obj/**" "SqlQueryRaw|ExecuteSqlRaw" "src/Infrastructure"
    if ($LASTEXITCODE -eq 0 -and $infraMatches) {
        $disallowed = @()
        foreach ($line in $infraMatches) {
            $normalizedLine = $line.Replace('\', '/')
            $isAllowed = $false
            foreach ($allowedPath in $allowedInfraPaths) {
                if ($normalizedLine -like "*$allowedPath*") {
                    $isAllowed = $true
                    break
                }
            }

            if (-not $isAllowed) {
                $disallowed += $line
            }
        }

        if ($disallowed.Count -gt 0) {
            Write-Host "FAILED: SQL raw found in Infrastructure outside allowed folders" -ForegroundColor Red
            $disallowed | ForEach-Object { Write-Host $_ -ForegroundColor Red }
            $allOk = $false
        } else {
            Write-Host "OK: SQL raw in Infrastructure restricted to allowed folders" -ForegroundColor Green
        }
    } else {
        Write-Host "OK: No SQL raw found in Infrastructure" -ForegroundColor Green
    }

    if (-not $allOk) {
        throw "Legacy runtime guardrails failed."
    }

    Write-Host "Legacy runtime guardrails passed." -ForegroundColor Green
}
finally {
    Pop-Location
}
