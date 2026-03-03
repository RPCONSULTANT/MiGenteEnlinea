param(
    [string]$RepoRoot = "."
)

$ErrorActionPreference = "Stop"

Push-Location $RepoRoot
try {
    $runtimePaths = @(
        "src/Core",
        "src/Presentation",
        "src/Infrastructure"
    )

    $allowedPathFragments = @(
        "Persistence/Migrations/",
        "Persistence/Seeding/",
        # Transitional exception: legacy UoW transaction API remains for backward compatibility.
        "Persistence/Repositories/UnitOfWork.cs"
    )

    $beginTxMatches = & rg -n --glob "!**/bin/**" --glob "!**/obj/**" "Database\.BeginTransactionAsync\(" @runtimePaths
    if ($LASTEXITCODE -ne 0 -or -not $beginTxMatches) {
        Write-Host "OK: No BeginTransactionAsync usages found in runtime paths." -ForegroundColor Green
        exit 0
    }

    $violations = New-Object System.Collections.Generic.List[string]

    foreach ($match in $beginTxMatches) {
        $parts = $match.Split(":", 3)
        if ($parts.Count -lt 3) {
            continue
        }

        $filePath = $parts[0]
        $lineNumber = [int]$parts[1]
        $normalizedFilePath = $filePath.Replace("\", "/")

        $isAllowedPath = $false
        foreach ($allowed in $allowedPathFragments) {
            if ($normalizedFilePath.Contains($allowed)) {
                $isAllowedPath = $true
                break
            }
        }

        if ($isAllowedPath) {
            continue
        }

        $content = Get-Content -LiteralPath $filePath
        $start = [Math]::Max(0, $lineNumber - 31)
        $end = [Math]::Min($content.Count - 1, $lineNumber + 4)
        $window = $content[$start..$end] -join "`n"

        if ($window -notmatch "CreateExecutionStrategy\(") {
            $violations.Add(("{0}:{1} -> BeginTransactionAsync without nearby CreateExecutionStrategy" -f $filePath, $lineNumber))
        }
    }

    if ($violations.Count -gt 0) {
        Write-Host "FAILED: EF retry-transaction guardrail violations detected" -ForegroundColor Red
        foreach ($violation in $violations) {
            Write-Host " - $violation" -ForegroundColor Red
        }
        exit 1
    }

    Write-Host "OK: All runtime BeginTransactionAsync usages are wrapped with CreateExecutionStrategy (or are in allowed paths)." -ForegroundColor Green
}
finally {
    Pop-Location
}
