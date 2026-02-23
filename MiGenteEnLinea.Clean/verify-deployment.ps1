param(
    [string]$ApiBaseUrl = "https://api.migenteenlinea.com",
    [string]$WebBaseUrl = "https://www.migenteenlinea.com",
    [switch]$SkipApi,
    [switch]$SkipWeb
)

$ErrorActionPreference = "Stop"

$ColorSuccess = "Green"
$ColorError = "Red"
$ColorWarning = "Yellow"
$ColorInfo = "Cyan"

$failed = 0
$passed = 0

function Run-Test {
    param(
        [string]$Name,
        [scriptblock]$Action
    )

    Write-Host " - $Name..." -NoNewline
    try {
        & $Action
        Write-Host " OK" -ForegroundColor $ColorSuccess
        return $true
    }
    catch {
        Write-Host " FAIL" -ForegroundColor $ColorError
        Write-Host "   $($_.Exception.Message)" -ForegroundColor $ColorError
        return $false
    }
}

Clear-Host
Write-Host ""
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host " Deployment Verification (myASP)" -ForegroundColor $ColorInfo
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host ""
Write-Host "API: $ApiBaseUrl" -ForegroundColor Gray
Write-Host "Web: $WebBaseUrl" -ForegroundColor Gray
Write-Host ""

if (-not $SkipApi) {
    Write-Host "API Checks" -ForegroundColor $ColorInfo

    if (Run-Test -Name "Health endpoint /health returns 200" -Action {
        $response = Invoke-WebRequest -Uri "$ApiBaseUrl/health" -Method Get -TimeoutSec 30 -UseBasicParsing
        if ($response.StatusCode -ne 200) {
            throw "Expected HTTP 200, got $($response.StatusCode)."
        }
    }) { $passed++ } else { $failed++ }

    if (Run-Test -Name "Swagger JSON /swagger/v1/swagger.json returns 200" -Action {
        $response = Invoke-WebRequest -Uri "$ApiBaseUrl/swagger/v1/swagger.json" -Method Get -TimeoutSec 30 -UseBasicParsing
        if ($response.StatusCode -ne 200) {
            throw "Expected HTTP 200, got $($response.StatusCode)."
        }
    }) { $passed++ } else { $failed++ }

    if (Run-Test -Name "API root returns a valid response" -Action {
        $response = Invoke-WebRequest -Uri "$ApiBaseUrl/" -Method Get -TimeoutSec 30 -UseBasicParsing
        if ($response.StatusCode -lt 200 -or $response.StatusCode -ge 500) {
            throw "Unexpected HTTP status at root: $($response.StatusCode)."
        }
    }) { $passed++ } else { $failed++ }

    Write-Host ""
}

if (-not $SkipWeb) {
    Write-Host "Web Checks" -ForegroundColor $ColorInfo

    if (Run-Test -Name "Web home / returns 200" -Action {
        $response = Invoke-WebRequest -Uri "$WebBaseUrl/" -Method Get -TimeoutSec 30 -UseBasicParsing
        if ($response.StatusCode -ne 200) {
            throw "Expected HTTP 200, got $($response.StatusCode)."
        }
    }) { $passed++ } else { $failed++ }

    if (Run-Test -Name "Web can fetch API health from browser perspective (CORS sanity)" -Action {
        $apiHealth = Invoke-WebRequest -Uri "$ApiBaseUrl/health" -Method Get -TimeoutSec 30 -UseBasicParsing
        if ($apiHealth.StatusCode -ne 200) {
            throw "API health is not reachable, status $($apiHealth.StatusCode)."
        }
    }) { $passed++ } else { $failed++ }

    Write-Host ""
}

Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host " Summary" -ForegroundColor $ColorInfo
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host ""
Write-Host "Passed: $passed" -ForegroundColor $ColorSuccess
Write-Host "Failed: $failed" -ForegroundColor $(if ($failed -gt 0) { $ColorError } else { $ColorSuccess })
Write-Host ""

if ($failed -gt 0) {
    Write-Host "Troubleshooting map:" -ForegroundColor $ColorWarning
    Write-Host " - 400 Invalid Hostname: fix host headers/bindings in myASP." -ForegroundColor Gray
    Write-Host " - 500.35: API and Web cannot share same in-process app pool." -ForegroundColor Gray
    Write-Host " - 404: validate site domain -> physical path mapping." -ForegroundColor Gray
    Write-Host " - 500: inspect stdout logs in /MigenteApi/api/logs and /MigenteApi/web/logs." -ForegroundColor Gray
    exit 1
}

exit 0
