param(
    [string]$SourceRoot = ".\src",
    [string]$ArtifactsRoot = ".\artifacts\publish"
)

$ErrorActionPreference = "Stop"

function Get-HashSafe {
    param([string]$Path)
    if (-not (Test-Path -LiteralPath $Path)) {
        return $null
    }

    return (Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash
}

function Test-WebConfigArtifact {
    param(
        [string]$Path,
        [string]$ExpectedDllName,
        [bool]$AllowOutOfProcess = $false
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        return @{
            IsValid = $false
            Error = "artifact file missing ($Path)"
        }
    }

    try {
        [xml]$xml = Get-Content -LiteralPath $Path -Raw
        $aspNetCore = $xml.SelectSingleNode("//aspNetCore")
        if ($null -eq $aspNetCore) {
            return @{ IsValid = $false; Error = "aspNetCore node not found" }
        }

        $processPathRaw = $aspNetCore.GetAttribute("processPath")
        $argumentsRaw = $aspNetCore.GetAttribute("arguments")
        $hostingModelRaw = $aspNetCore.GetAttribute("hostingModel")

        $processPath = ""
        if ($null -ne $processPathRaw) { $processPath = [string]$processPathRaw }
        $processPath = $processPath.Trim()

        $arguments = ""
        if ($null -ne $argumentsRaw) { $arguments = [string]$argumentsRaw }
        $arguments = $arguments.Trim().ToLowerInvariant()

        $hostingModel = ""
        if ($null -ne $hostingModelRaw) { $hostingModel = [string]$hostingModelRaw }
        $hostingModel = $hostingModel.Trim().ToLowerInvariant()

        if ($processPath.ToLowerInvariant() -ne "dotnet") {
            return @{ IsValid = $false; Error = "processPath must be 'dotnet' (actual: '$processPath')" }
        }

        $expectedArgument = ".\$ExpectedDllName".ToLowerInvariant()
        if ($arguments -ne $expectedArgument) {
            return @{ IsValid = $false; Error = "arguments must be '$expectedArgument' (actual: '$arguments')" }
        }

        if ($AllowOutOfProcess) {
            if ($hostingModel -ne "inprocess" -and $hostingModel -ne "outofprocess") {
                return @{ IsValid = $false; Error = "hostingModel must be 'inprocess' or 'outofprocess' (actual: '$hostingModel')" }
            }
        }
        else {
            if ($hostingModel -ne "inprocess") {
                return @{ IsValid = $false; Error = "hostingModel must be 'inprocess' (actual: '$hostingModel')" }
            }
        }

        return @{ IsValid = $true; Error = $null }
    }
    catch {
        return @{
            IsValid = $false
            Error = "invalid xml: $($_.Exception.Message)"
        }
    }
}

$sourceRootPath = [System.IO.Path]::GetFullPath($SourceRoot)
$artifactsRootPath = [System.IO.Path]::GetFullPath($ArtifactsRoot)

$checks = @(
    @{
        Name = "API appsettings.Production.json"
        Mode = "hash"
        Source = Join-Path $sourceRootPath "Presentation\MiGenteEnLinea.API\appsettings.Production.json"
        Artifact = Join-Path $artifactsRootPath "API\appsettings.Production.json"
    },
    @{
        Name = "API web.config"
        Mode = "webconfig"
        ExpectedDllName = "MiGenteEnLinea.API.dll"
        AllowOutOfProcess = $false
        Source = Join-Path $sourceRootPath "Presentation\MiGenteEnLinea.API\web.config"
        Artifact = Join-Path $artifactsRootPath "API\web.config"
    },
    @{
        Name = "Web appsettings.Production.json"
        Mode = "hash"
        Source = Join-Path $sourceRootPath "Presentation\MiGenteEnLinea.Web\appsettings.Production.json"
        Artifact = Join-Path $artifactsRootPath "Web\appsettings.Production.json"
    },
    @{
        Name = "Web web.config"
        Mode = "webconfig"
        ExpectedDllName = "MiGenteEnLinea.Web.dll"
        AllowOutOfProcess = $true
        Source = Join-Path $sourceRootPath "Presentation\MiGenteEnLinea.Web\web.config"
        Artifact = Join-Path $artifactsRootPath "Web\web.config"
    },
    @{
        Name = "Web Custom.js"
        Mode = "hash"
        Source = Join-Path $sourceRootPath "Presentation\MiGenteEnLinea.Web\wwwroot\js\Custom.js"
        Artifact = Join-Path $artifactsRootPath "Web\wwwroot\js\Custom.js"
    }
)

$mismatches = New-Object System.Collections.Generic.List[string]

Write-Host ""
Write-Host "Artifact Integrity Check" -ForegroundColor Cyan
Write-Host "Source: $sourceRootPath" -ForegroundColor Gray
Write-Host "Artifacts: $artifactsRootPath" -ForegroundColor Gray
Write-Host ""

foreach ($check in $checks) {
    if ($check.Mode -eq "webconfig") {
        $webConfigResult = Test-WebConfigArtifact -Path $check.Artifact -ExpectedDllName $check.ExpectedDllName -AllowOutOfProcess $check.AllowOutOfProcess
        if (-not $webConfigResult.IsValid) {
            $mismatches.Add("$($check.Name): $($webConfigResult.Error)")
            Write-Host " - $($check.Name): MISMATCH" -ForegroundColor Yellow
            continue
        }

        Write-Host " - $($check.Name): OK" -ForegroundColor Green
        continue
    }

    $sourceHash = Get-HashSafe -Path $check.Source
    $artifactHash = Get-HashSafe -Path $check.Artifact

    if ([string]::IsNullOrWhiteSpace($sourceHash)) {
        $mismatches.Add("$($check.Name): source file missing ($($check.Source))")
        Write-Host " - $($check.Name): SOURCE MISSING" -ForegroundColor Red
        continue
    }

    if ([string]::IsNullOrWhiteSpace($artifactHash)) {
        $mismatches.Add("$($check.Name): artifact file missing ($($check.Artifact))")
        Write-Host " - $($check.Name): ARTIFACT MISSING" -ForegroundColor Red
        continue
    }

    if ($sourceHash -ne $artifactHash) {
        $mismatches.Add("$($check.Name): hash mismatch")
        Write-Host " - $($check.Name): MISMATCH" -ForegroundColor Yellow
        continue
    }

    Write-Host " - $($check.Name): OK" -ForegroundColor Green
}

Write-Host ""

if ($mismatches.Count -gt 0) {
    Write-Host "Integrity check failed:" -ForegroundColor Red
    foreach ($item in $mismatches) {
        Write-Host " - $item" -ForegroundColor Red
    }
    exit 1
}

Write-Host "Integrity check passed." -ForegroundColor Green
exit 0
