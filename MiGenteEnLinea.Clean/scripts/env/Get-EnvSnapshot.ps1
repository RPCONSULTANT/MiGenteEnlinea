param(
    [ValidateSet("API", "Web", "E2E", "All")]
    [string]$Component = "All",
    [string]$SpecPath = "",
    [string]$OutputPath = ""
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($SpecPath)) {
    $SpecPath = Join-Path $PSScriptRoot "expected-env-vars.json"
}

if (-not (Test-Path $SpecPath)) {
    throw "Spec file not found: $SpecPath"
}

if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
    $OutputPath = Join-Path $PSScriptRoot "..\..\artifacts\env\env-snapshot-$timestamp.json"
}

$spec = Get-Content $SpecPath -Raw | ConvertFrom-Json
$envMap = @{}
Get-ChildItem Env: | ForEach-Object { $envMap[$_.Name] = $_.Value }

function Resolve-Components([string]$requested, $componentsNode) {
    if ($requested -eq "All") { return @("API", "Web", "E2E") }
    return @($requested)
}

function HasValue([string]$name) {
    if (-not $envMap.ContainsKey($name)) { return $false }
    return -not [string]::IsNullOrWhiteSpace([string]$envMap[$name])
}

$components = Resolve-Components -requested $Component -componentsNode $spec.components
$result = [ordered]@{
    generatedAt = (Get-Date).ToString("o")
    machineName = $env:COMPUTERNAME
    component = $Component
    present = @()
    missing = @()
}

foreach ($c in $components) {
    $node = $spec.components.$c
    foreach ($entry in $node.required) {
        if ($entry.name) {
            if (HasValue $entry.name) {
                $result.present += "$c::$($entry.name)"
            } else {
                $result.missing += "$c::$($entry.name)"
            }
        } elseif ($entry.anyOf) {
            $match = $null
            foreach ($alias in $entry.anyOf) {
                if (HasValue $alias) {
                    $match = $alias
                    break
                }
            }

            if ($match) {
                $result.present += "$c::$match"
            } else {
                $label = if ($entry.label) { $entry.label } else { "anyOf" }
                $result.missing += "$c::$label (one of: $($entry.anyOf -join ', '))"
            }
        }
    }

    foreach ($prefix in $node.requiredPrefixes) {
        $matched = $envMap.Keys | Where-Object { $_.StartsWith($prefix, [System.StringComparison]::OrdinalIgnoreCase) -and -not [string]::IsNullOrWhiteSpace($envMap[$_]) }
        if ($matched.Count -gt 0) {
            $result.present += "$c::$($matched[0])"
        } else {
            $result.missing += "$c::$prefix*"
        }
    }
}

$parent = Split-Path $OutputPath -Parent
if (-not (Test-Path $parent)) {
    New-Item -ItemType Directory -Path $parent -Force | Out-Null
}

$result | ConvertTo-Json -Depth 8 | Set-Content -Path $OutputPath -Encoding UTF8
Write-Host "Snapshot written: $OutputPath"
Write-Host "Present: $($result.present.Count) | Missing: $($result.missing.Count)"
