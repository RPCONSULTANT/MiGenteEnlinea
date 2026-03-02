param(
    [string]$ProgramPath = "src\Presentation\MiGenteEnLinea.API\Program.cs"
)

$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path -Parent $PSScriptRoot

$resolvedProgramPath = $ProgramPath
if (-not [System.IO.Path]::IsPathRooted($ProgramPath)) {
    $resolvedProgramPath = Join-Path $RepoRoot $ProgramPath
}

if (-not (Test-Path -LiteralPath $resolvedProgramPath)) {
    throw "Program.cs no encontrado en ruta: $resolvedProgramPath"
}

$fullPath = [System.IO.Path]::GetFullPath($resolvedProgramPath)
$lines = Get-Content -LiteralPath $fullPath

$buildIndex = -1
for ($i = 0; $i -lt $lines.Count; $i++) {
    if ($lines[$i] -match "var\s+app\s*=\s*builder\.Build\(\)\s*;") {
        $buildIndex = $i
        break
    }
}

if ($buildIndex -lt 0) {
    throw "No se encontro 'var app = builder.Build();' en $fullPath"
}

$violations = New-Object System.Collections.Generic.List[string]
$patterns = @(
    "app\.Services\.Configure\s*<",
    "app\.Services\.Add[A-Za-z0-9_]*\s*\(",
    "builder\.Services\.[A-Za-z0-9_]+\s*\("
)

for ($i = $buildIndex + 1; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    foreach ($pattern in $patterns) {
        if ($line -match $pattern) {
            $violations.Add("line $($i + 1): $line")
            break
        }
    }
}

if ($violations.Count -gt 0) {
    Write-Host ""
    Write-Host "Startup composition validation FAILED" -ForegroundColor Red
    Write-Host "Program: $fullPath" -ForegroundColor Gray
    Write-Host "Build line: $($buildIndex + 1)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Violaciones detectadas despues de builder.Build():" -ForegroundColor Red
    foreach ($v in $violations) {
        Write-Host " - $v" -ForegroundColor Red
    }
    exit 1
}

Write-Host ""
Write-Host "Startup composition validation PASSED" -ForegroundColor Green
Write-Host "Program: $fullPath" -ForegroundColor Gray
Write-Host "Build line: $($buildIndex + 1)" -ForegroundColor Gray
Write-Host ""
exit 0
