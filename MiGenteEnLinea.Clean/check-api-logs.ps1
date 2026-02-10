# ========================================
# Check API Logs via FTP
# ========================================

$FtpHost = "win8146.site4now.net"
$FtpUsername = "migente-001"
$FtpPassword = "Migente@1"
$RemotePath = "/migenteenlinea2/api/logs"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  🔍 Checking API Logs" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check WinSCP
$WinScpPath = "C:\Program Files (x86)\WinSCP\WinSCP.com"
if (-not (Test-Path $WinScpPath)) {
    $WinScpPath = "C:\Program Files\WinSCP\WinSCP.com"
}

if (-not (Test-Path $WinScpPath)) {
    Write-Host "❌ WinSCP not found. Cannot download logs automatically." -ForegroundColor Red
    Write-Host ""
    Write-Host "Please check logs manually via FTP:" -ForegroundColor Yellow
    Write-Host "  Path: /migenteenlinea2/api/logs/" -ForegroundColor Gray
    Write-Host "  Look for: stdout_*.log files" -ForegroundColor Gray
    exit 1
}

# Create temp directory
$TempDir = "$env:TEMP\MiGenteApiLogs"
New-Item -ItemType Directory -Path $TempDir -Force | Out-Null

# Create WinSCP script to list log files
$ListScript = @"
option batch abort
option confirm off
open ftp://${FtpUsername}:${FtpPassword}@${FtpHost}/
cd $RemotePath
ls
close
exit
"@

$ListScriptPath = "$TempDir\list-logs.txt"
$ListScript | Out-File -FilePath $ListScriptPath -Encoding ASCII

Write-Host "📂 Checking for log files..." -ForegroundColor Yellow

# Execute WinSCP to list files
$listResult = & $WinScpPath /script="$ListScriptPath" /xmllog="$TempDir\list-output.xml"

# Check if logs folder exists
if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "⚠️  Logs folder not found or empty" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "This means one of:" -ForegroundColor White
    Write-Host "  1️⃣  The application hasn't started yet (no errors logged)" -ForegroundColor Gray
    Write-Host "  2️⃣  Logs folder doesn't have write permissions" -ForegroundColor Gray
    Write-Host "  3️⃣  Application crashed before writing logs" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Try accessing the API endpoint again:" -ForegroundColor Cyan
    Write-Host "  http://migente-001-site1.jtempurl.com/api/health" -ForegroundColor White
    Write-Host ""
    exit 1
}

Write-Host "✅ Logs folder found" -ForegroundColor Green
Write-Host ""

# Download the most recent log file
$DownloadScript = @"
option batch abort
option confirm off
option transfer binary
open ftp://${FtpUsername}:${FtpPassword}@${FtpHost}/
cd $RemotePath
lcd "$TempDir"
get stdout_*.log
close
exit
"@

$DownloadScriptPath = "$TempDir\download-logs.txt"
$DownloadScript | Out-File -FilePath $DownloadScriptPath -Encoding ASCII

Write-Host "⬇️  Downloading log files..." -ForegroundColor Yellow

& $WinScpPath /script="$DownloadScriptPath" | Out-Null

# Find and display the most recent log
$logFiles = Get-ChildItem -Path $TempDir -Filter "stdout_*.log" -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending

if ($logFiles.Count -eq 0) {
    Write-Host ""
    Write-Host "⚠️  No stdout log files found" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "The application might not have started yet." -ForegroundColor Gray
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "  1️⃣  Verify Application Pool is started" -ForegroundColor Gray
    Write-Host "  2️⃣  Check web.config exists in /migenteenlinea2/api/" -ForegroundColor Gray
    Write-Host "  3️⃣  Try accessing /api/health endpoint again" -ForegroundColor Gray
    exit 1
}

$latestLog = $logFiles[0]

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  📄 Latest Log: $($latestLog.Name)" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

$logContent = Get-Content $latestLog.FullName -Raw

if ([string]::IsNullOrWhiteSpace($logContent)) {
    Write-Host "⚠️  Log file is empty" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "This usually means:" -ForegroundColor White
    Write-Host "  • Application started successfully but hasn't logged anything yet" -ForegroundColor Gray
    Write-Host "  • OR: Application is still starting up" -ForegroundColor Gray
} else {
    Write-Host $logContent
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    
    # Analyze common errors
    if ($logContent -like "*The specified framework*was not found*") {
        Write-Host ""
        Write-Host "🔴 PROBLEM IDENTIFIED: ASP.NET Core Runtime Missing" -ForegroundColor Red
        Write-Host ""
        Write-Host "The server doesn't have ASP.NET Core 8.0 installed." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "SOLUTION:" -ForegroundColor Cyan
        Write-Host "  Contact myASP.NET support and request:" -ForegroundColor White
        Write-Host "  'Please install ASP.NET Core 8.0 Hosting Bundle on win8146.site4now.net'" -ForegroundColor Gray
        Write-Host ""
    }
    elseif ($logContent -like "*System.IO.FileNotFoundException*") {
        Write-Host ""
        Write-Host "🔴 PROBLEM IDENTIFIED: Missing DLL Files" -ForegroundColor Red
        Write-Host ""
        Write-Host "Some required files weren't uploaded correctly." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "SOLUTION:" -ForegroundColor Cyan
        Write-Host "  Re-run: .\publish-and-deploy-ftp.ps1 -ApiOnly" -ForegroundColor White
        Write-Host ""
    }
    elseif ($logContent -like "*Connection*") {
        Write-Host ""
        Write-Host "🔴 PROBLEM IDENTIFIED: Database Connection" -ForegroundColor Red
        Write-Host ""
        Write-Host "Cannot connect to SQL Server." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "SOLUTION:" -ForegroundColor Cyan
        Write-Host "  Verify connection string in appsettings.Production.json" -ForegroundColor White
        Write-Host "  Server: SQL5106.site4now.net" -ForegroundColor Gray
        Write-Host ""
    }
    else {
        Write-Host ""
        Write-Host "📋 Review the log above for specific errors" -ForegroundColor Cyan
        Write-Host ""
    }
}

Write-Host ""
Write-Host "📂 Log files saved to: $TempDir" -ForegroundColor Gray
Write-Host ""

# Cleanup scripts
Remove-Item $ListScriptPath -Force -ErrorAction SilentlyContinue
Remove-Item $DownloadScriptPath -Force -ErrorAction SilentlyContinue
