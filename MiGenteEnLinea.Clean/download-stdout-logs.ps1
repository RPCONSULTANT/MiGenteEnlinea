# ========================================
# Download stdout logs from FTP
# ========================================

param(
    [string]$LogPath = "$env:TEMP\MiGenteStdoutLogs"
)

$ColorSuccess = "Green"
$ColorError = "Red"
$ColorWarning = "Yellow"
$ColorInfo = "Cyan"

# FTP Settings
$FtpHost = "win8146.site4now.net"
$FtpUsername = "rainiery"
$FtpPassword = "Pevertiman00!"
$RemoteLogPath = "/MigenteApi/api/logs"

Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host "  📥 Downloading stdout Logs from FTP" -ForegroundColor $ColorInfo
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host ""

# Create temp directory
if (-not (Test-Path $LogPath)) {
    New-Item -ItemType Directory -Path $LogPath -Force | Out-Null
}

Write-Host "📂 Log download path: $LogPath" -ForegroundColor Gray
Write-Host ""

# Check WinSCP
$WinScpPath = "C:\Program Files (x86)\WinSCP\WinSCP.com"
if (-not (Test-Path $WinScpPath)) {
    $WinScpPath = "C:\Program Files\WinSCP\WinSCP.com"
}

if (-not (Test-Path $WinScpPath)) {
    Write-Host "❌ WinSCP not found!" -ForegroundColor $ColorError
    Write-Host ""
    Write-Host "Alternative: Use FileZilla to download manually from:" -ForegroundColor $ColorWarning
    Write-Host "  $RemoteLogPath/stdout*.log" -ForegroundColor Gray
    exit 1
}

# Create WinSCP script
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$ScriptPath = "$LogPath\winscp-download-$timestamp.txt"

$WinScpScript = @"
option batch on
option confirm off
option transfer binary

# Connect to FTP
open ftp://${FtpUsername}:${FtpPassword}@${FtpHost}/ -passive=on

# Change to logs directory
cd $RemoteLogPath

# List log files
ls

# Download all stdout logs
get stdout*.log $LogPath\

# Close connection
close
exit
"@

Set-Content -Path $ScriptPath -Value $WinScpScript -Encoding ASCII

Write-Host "🔄 Downloading logs from server..." -ForegroundColor $ColorInfo
Write-Host ""

# Execute WinSCP
$output = & $WinScpPath /script=$ScriptPath 2>&1

# Check if successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Logs downloaded successfully!" -ForegroundColor $ColorSuccess
    Write-Host ""
    
    # List downloaded files
    $logFiles = Get-ChildItem -Path $LogPath -Filter "stdout*.log" -ErrorAction SilentlyContinue
    
    if ($logFiles.Count -gt 0) {
        Write-Host "📄 Downloaded log files:" -ForegroundColor $ColorSuccess
        foreach ($file in $logFiles) {
            Write-Host "   - $($file.Name) ($($file.Length) bytes)" -ForegroundColor Gray
        }
        Write-Host ""
        
        # Show most recent log content
        $latestLog = $logFiles | Sort-Object LastWriteTime -Descending | Select-Object -First 1
        Write-Host "📋 Content of latest log ($($latestLog.Name)):" -ForegroundColor $ColorInfo
        Write-Host "----------------------------------------" -ForegroundColor Gray
        Get-Content $latestLog.FullName -ErrorAction SilentlyContinue
        Write-Host "----------------------------------------" -ForegroundColor Gray
        Write-Host ""
        Write-Host "📁 All logs saved to: $LogPath" -ForegroundColor $ColorInfo
        
    } else {
        Write-Host "⚠️  No stdout log files found on server" -ForegroundColor $ColorWarning
        Write-Host ""
        Write-Host "This means:" -ForegroundColor $ColorWarning
        Write-Host "  1️⃣  The application hasn't written any logs yet" -ForegroundColor Gray
        Write-Host "  2️⃣  The logs folder doesn't have write permissions" -ForegroundColor Gray
        Write-Host "  3️⃣  The application is crashing before stdout can write" -ForegroundColor Gray
        Write-Host ""
        Write-Host "FTP listing output:" -ForegroundColor $ColorInfo
        Write-Host $output
    }
} else {
    Write-Host "❌ Failed to download logs" -ForegroundColor $ColorError
    Write-Host ""
    Write-Host "Error output:" -ForegroundColor $ColorWarning
    Write-Host $output
    Write-Host ""
    Write-Host "Try manually with FileZilla:" -ForegroundColor $ColorInfo
    Write-Host "  Host: $FtpHost" -ForegroundColor Gray
    Write-Host "  Path: $RemoteLogPath" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor $ColorInfo
