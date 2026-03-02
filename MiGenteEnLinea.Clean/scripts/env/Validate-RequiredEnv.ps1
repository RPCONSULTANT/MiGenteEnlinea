param(
    [ValidateSet("API", "Web", "E2E", "All")]
    [string]$Component = "All",
    [string]$SpecPath = "",
    [switch]$IncludeOptional
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($SpecPath)) {
    $SpecPath = Join-Path $PSScriptRoot "expected-env-vars.json"
}

if (-not (Test-Path $SpecPath)) {
    throw "Spec file not found: $SpecPath"
}

$spec = Get-Content $SpecPath -Raw | ConvertFrom-Json
$envMap = @{}
Get-ChildItem Env: | ForEach-Object { $envMap[$_.Name] = $_.Value }

function Test-HasValue([string]$name) {
    if (-not $envMap.ContainsKey($name)) { return $false }
    $value = [string]$envMap[$name]
    return -not [string]::IsNullOrWhiteSpace($value)
}

function Resolve-Components([string]$requested, $componentsNode) {
    if ($requested -eq "All") {
        return @("API", "Web", "E2E")
    }

    if (-not $componentsNode.PSObject.Properties.Name.Contains($requested)) {
        throw "Component '$requested' not found in expected-env-vars.json"
    }

    return @($requested)
}

$components = Resolve-Components -requested $Component -componentsNode $spec.components
$missing = @()
$present = @()

foreach ($c in $components) {
    $node = $spec.components.$c

    foreach ($entry in $node.required) {
        if ($entry.name) {
            if (Test-HasValue $entry.name) {
                $present += "$c::$($entry.name)"
            } else {
                $missing += "$c::$($entry.name)"
            }
            continue
        }

        if ($entry.anyOf) {
            $found = $false
            foreach ($alias in $entry.anyOf) {
                if (Test-HasValue $alias) {
                    $present += "$c::$alias"
                    $found = $true
                    break
                }
            }
            if (-not $found) {
                $label = if ($entry.label) { $entry.label } else { "anyOf" }
                $missing += "$c::$label (one of: $($entry.anyOf -join ', '))"
            }
        }
    }

    foreach ($prefix in $node.requiredPrefixes) {
        $foundPrefix = $false
        foreach ($envName in $envMap.Keys) {
            if ($envName.StartsWith($prefix, [System.StringComparison]::OrdinalIgnoreCase) -and -not [string]::IsNullOrWhiteSpace($envMap[$envName])) {
                $present += "$c::$envName"
                $foundPrefix = $true
                break
            }
        }

        if (-not $foundPrefix) {
            $missing += "$c::$prefix*"
        }
    }

    if ($IncludeOptional) {
        foreach ($optionalName in $node.optional) {
            if (Test-HasValue $optionalName) {
                $present += "$c::$optionalName"
            } else {
                $missing += "$c::$optionalName (optional)"
            }
        }
    }
}

Write-Host "Environment validation summary"
Write-Host "  Component(s): $($components -join ', ')"
Write-Host "  Present: $($present.Count)"
Write-Host "  Missing: $($missing.Count)"

if ($missing.Count -gt 0) {
    Write-Host ""
    Write-Host "Missing variables:"
    $missing | Sort-Object | ForEach-Object { Write-Host " - $_" }
    exit 1
}

Write-Host "All required environment variables are present."
exit 0
