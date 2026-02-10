# ========================================
# MiGente En Línea - Deployment Verification
# ========================================

param(
    [string]$ApiBaseUrl = "https://api.migenteenlinea.com",
    [string]$WebBaseUrl = "https://www.migenteenlinea.com",
    [switch]$SkipApi,
    [switch]$SkipWeb
)

$ColorSuccess = "Green"
$ColorError = "Red"
$ColorWarning = "Yellow"
$ColorInfo = "Cyan"

$failed = 0
$passed = 0

Clear-Host
Write-Host ""
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host "  🔍 Deployment Verification" -ForegroundColor $ColorInfo
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host ""

# ========================================
# API Tests
# ========================================

if (-not $SkipApi) {
    Write-Host "📡 API Tests ($ApiBaseUrl)" -ForegroundColor $ColorInfo
    Write-Host ""

    # Test 1: Health endpoint
    Write-Host "   🔹 Testing health endpoint..." -NoNewline
    try {
        $response = Invoke-WebRequest -Uri "$ApiBaseUrl/health" -Method Get -TimeoutSec 30 -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Host " ✅ PASSED" -ForegroundColor $ColorSuccess
            $passed++
            
            $content = $response.Content | ConvertFrom-Json
            if ($content.status -eq "Healthy") {
                Write-Host "      Status: Healthy" -ForegroundColor Gray
            } else {
                Write-Host "      ⚠️  Status: $($content.status)" -ForegroundColor $ColorWarning
            }
        } else {
            Write-Host " ❌ FAILED (HTTP $($response.StatusCode))" -ForegroundColor $ColorError
            $failed++
        }
    } catch {
        Write-Host " ❌ FAILED" -ForegroundColor $ColorError
        Write-Host "      Error: $($_.Exception.Message)" -ForegroundColor $ColorError
        $failed++
    }

    # Test 2: Swagger UI
    Write-Host "   🔹 Testing Swagger UI..." -NoNewline
    try {
        $response = Invoke-WebRequest -Uri "$ApiBaseUrl/" -Method Get -TimeoutSec 30 -UseBasicParsing
        if ($response.StatusCode -eq 200 -and $response.Content -like "*swagger*") {
            Write-Host " ✅ PASSED" -ForegroundColor $ColorSuccess
            $passed++
        } else {
            Write-Host " ❌ FAILED (No Swagger content found)" -ForegroundColor $ColorError
            $failed++
        }
    } catch {
        Write-Host " ❌ FAILED" -ForegroundColor $ColorError
        Write-Host "      Error: $($_.Exception.Message)" -ForegroundColor $ColorError
        $failed++
    }

    # Test 3: Sample API endpoint (public plans)
    Write-Host "   🔹 Testing API endpoint /api/planes..." -NoNewline
    try {
        $response = Invoke-WebRequest -Uri "$ApiBaseUrl/api/planes" -Method Get -TimeoutSec 30 -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Host " ✅ PASSED" -ForegroundColor $ColorSuccess
            $passed++
            
            $plans = $response.Content | ConvertFrom-Json
            Write-Host "      Found $($plans.Count) plans" -ForegroundColor Gray
        } else {
            Write-Host " ❌ FAILED (HTTP $($response.StatusCode))" -ForegroundColor $ColorError
            $failed++
        }
    } catch {
        Write-Host " ⚠️  SKIPPED (endpoint may require auth)" -ForegroundColor $ColorWarning
        Write-Host "      Error: $($_.Exception.Message)" -ForegroundColor Gray
    }

    Write-Host ""
}

# ========================================
# Web Tests
# ========================================

if (-not $SkipWeb) {
    Write-Host "🌐 Web Tests ($WebBaseUrl)" -ForegroundColor $ColorInfo
    Write-Host ""

    # Test 1: Homepage
    Write-Host "   🔹 Testing homepage..." -NoNewline
    try {
        $response = Invoke-WebRequest -Uri "$WebBaseUrl/" -Method Get -TimeoutSec 30 -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Host " ✅ PASSED" -ForegroundColor $ColorSuccess
            $passed++
            
            if ($response.Content -like "*MiGente*" -or $response.Content -like "*migente*") {
                Write-Host "      Content verified" -ForegroundColor Gray
            }
        } else {
            Write-Host " ❌ FAILED (HTTP $($response.StatusCode))" -ForegroundColor $ColorError
            $failed++
        }
    } catch {
        Write-Host " ❌ FAILED" -ForegroundColor $ColorError
        Write-Host "      Error: $($_.Exception.Message)" -ForegroundColor $ColorError
        $failed++
    }

    # Test 2: Static files (CSS)
    Write-Host "   🔹 Testing static files..." -NoNewline
    try {
        $response = Invoke-WebRequest -Uri "$WebBaseUrl/css/Custom.css" -Method Get -TimeoutSec 30 -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Host " ✅ PASSED" -ForegroundColor $ColorSuccess
            $passed++
        } else {
            Write-Host " ❌ FAILED (HTTP $($response.StatusCode))" -ForegroundColor $ColorError
            $failed++
        }
    } catch {
        Write-Host " ⚠️  WARNING (CSS not found)" -ForegroundColor $ColorWarning
        Write-Host "      This may be normal if using different path" -ForegroundColor Gray
    }

    # Test 3: Login page
    Write-Host "   🔹 Testing login page..." -NoNewline
    try {
        $response = Invoke-WebRequest -Uri "$WebBaseUrl/Auth/Login" -Method Get -TimeoutSec 30 -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-Host " ✅ PASSED" -ForegroundColor $ColorSuccess
            $passed++
        } else {
            Write-Host " ⚠️  WARNING (HTTP $($response.StatusCode))" -ForegroundColor $ColorWarning
            Write-Host "      Login page may be at different path" -ForegroundColor Gray
        }
    } catch {
        Write-Host " ⚠️  WARNING" -ForegroundColor $ColorWarning
        Write-Host "      Login page may be at different path" -ForegroundColor Gray
    }

    Write-Host ""
}

# ========================================
# Summary
# ========================================

Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host "  📊 Verification Summary" -ForegroundColor $ColorInfo
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host ""
Write-Host "✅ Passed: $passed tests" -ForegroundColor $ColorSuccess
Write-Host "❌ Failed: $failed tests" -ForegroundColor $(if ($failed -gt 0) { $ColorError } else { $ColorSuccess })
Write-Host ""

if ($failed -eq 0) {
    Write-Host "🎉 All critical tests passed!" -ForegroundColor $ColorSuccess
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor White
    Write-Host "• Test user registration and login" -ForegroundColor Gray
    Write-Host "• Upload a contractor profile image" -ForegroundColor Gray
    Write-Host "• Create test data (empleador, contratista, empleado)" -ForegroundColor Gray
    Write-Host "• Monitor logs for any errors" -ForegroundColor Gray
    Write-Host ""
    exit 0
} else {
    Write-Host "⚠️  Some tests failed. Please check:" -ForegroundColor $ColorWarning
    Write-Host ""
    Write-Host "Common issues:" -ForegroundColor White
    Write-Host "• IIS application not configured correctly" -ForegroundColor Gray
    Write-Host "• Application pool stopped or crashed" -ForegroundColor Gray
    Write-Host "• web.config errors (check stdout logs)" -ForegroundColor Gray
    Write-Host "• Database connection issues" -ForegroundColor Gray
    Write-Host "• .NET 8.0 Runtime not installed on server" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Check logs via FTP:" -ForegroundColor White
    Write-Host "• /migenteenlinea2/api/logs/stdout_*.log" -ForegroundColor Gray
    Write-Host "• /migenteenlinea2/web/logs/stdout_*.log" -ForegroundColor Gray
    Write-Host ""
    exit 1
}
