# ========================================
# MiGente En Lnea - FTP Deployment Script
# Target: myASP.NET (site4now.net)
# ========================================

param(
    [string]$Configuration = "Release",
    [string]$OutputPath = ".\artifacts\publish",
    [string]$DbConnectionString = "",
    [switch]$SkipBuild,
    [switch]$SkipUpload,
    [switch]$RunDbInit,
    [switch]$WebOutOfProcessFallback,
    [switch]$ApiOnly,
    [switch]$WebOnly
)

$ErrorActionPreference = "Stop"
$ScriptRoot = $PSScriptRoot

# Normalize output path to absolute path so WinSCP `lcd` never depends on caller CWD.
$OutputPath = [System.IO.Path]::GetFullPath($OutputPath)

# FTP Configuration - myASP.NET
$FtpHost = "win8146.site4now.net"
$FtpUsername = "rainiery"
$FtpPassword = "Pevertiman00!"
$RemoteRoot = "/MigenteApi"

# Colors
$ColorInfo = "Cyan"
$ColorSuccess = "Green"
$ColorWarning = "Yellow"
$ColorError = "Red"

# Script start
Clear-Host
Write-Host ""
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host "   MiGente En Lnea Deployment" -ForegroundColor $ColorInfo
Write-Host "========================================" -ForegroundColor $ColorInfo
Write-Host ""
Write-Host "Target: myASP.NET (win8146.site4now.net)" -ForegroundColor Gray
Write-Host "Remote: $RemoteRoot" -ForegroundColor Gray

$scriptInfo = Get-Item -LiteralPath $PSCommandPath
$scriptHash = Get-FileHash -LiteralPath $PSCommandPath -Algorithm SHA256
Write-Host "Script: $($scriptInfo.FullName)" -ForegroundColor Gray
Write-Host "Modified: $($scriptInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"))" -ForegroundColor Gray
Write-Host "SHA256: $($scriptHash.Hash)" -ForegroundColor Gray
Write-Host "Web OutOfProcess Fallback: $WebOutOfProcessFallback" -ForegroundColor Gray
Write-Host ""

# ========================================
# STEP 0: STARTUP COMPOSITION CONTRACT
# ========================================

Write-Host " STEP 0: Validating startup composition contract..." -ForegroundColor $ColorInfo
Write-Host ""
$startupValidationScript = Join-Path $ScriptRoot "scripts\validate-startup-composition.ps1"
& $startupValidationScript
if ($LASTEXITCODE -ne 0) {
    Write-Host " Startup composition validation failed." -ForegroundColor $ColorError
    exit 1
}
Write-Host " Startup composition contract passed." -ForegroundColor $ColorSuccess
Write-Host ""

# ========================================
# STEP 1: BUILD ARTIFACTS
# ========================================

if (-not $SkipBuild) {
    Write-Host " STEP 1: Building artifacts..." -ForegroundColor $ColorInfo
    Write-Host ""

    # Clean output directory
    if (Test-Path $OutputPath) {
        Write-Host "   Cleaning output directory..." -ForegroundColor Gray
        Remove-Item -Path $OutputPath -Recurse -Force -ErrorAction SilentlyContinue
    }

    # Create output directories
    $ApiOutput = Join-Path $OutputPath "API"
    $WebOutput = Join-Path $OutputPath "Web"
    New-Item -ItemType Directory -Path $ApiOutput -Force | Out-Null
    New-Item -ItemType Directory -Path $WebOutput -Force | Out-Null

    # Build API
    if (-not $WebOnly) {
        Write-Host "    Publishing API..." -ForegroundColor Yellow
        $apiProject = Join-Path $ScriptRoot "src\Presentation\MiGenteEnLinea.API\MiGenteEnLinea.API.csproj"
        
        dotnet publish $apiProject `
            --configuration $Configuration `
            --output $ApiOutput `
            --self-contained false `
            --verbosity minimal

        if ($LASTEXITCODE -ne 0) {
            Write-Host ""
            Write-Host " API build failed!" -ForegroundColor $ColorError
            exit 1
        }

        # Create required folders
        New-Item -ItemType Directory -Path "$ApiOutput\logs" -Force | Out-Null
        New-Item -ItemType Directory -Path "$ApiOutput\wwwroot\uploads\contratistas-fotos" -Force | Out-Null
        
        Write-Host "    API build complete" -ForegroundColor $ColorSuccess
    }

    # Build Web
    if (-not $ApiOnly) {
        Write-Host "    Publishing Web..." -ForegroundColor Yellow
        $webProject = Join-Path $ScriptRoot "src\Presentation\MiGenteEnLinea.Web\MiGenteEnLinea.Web.csproj"
        
        dotnet publish $webProject `
            --configuration $Configuration `
            --output $WebOutput `
            --self-contained false `
            --verbosity minimal

        if ($LASTEXITCODE -ne 0) {
            Write-Host ""
            Write-Host " Web build failed!" -ForegroundColor $ColorError
            exit 1
        }

        # Create required folders
        New-Item -ItemType Directory -Path "$WebOutput\logs" -Force | Out-Null
        
        Write-Host "    Web build complete" -ForegroundColor $ColorSuccess
    }

    Write-Host ""
    Write-Host " Build artifacts created successfully!" -ForegroundColor $ColorSuccess
    Write-Host ""
} else {
    Write-Host "  Skipping build (using existing artifacts)" -ForegroundColor $ColorWarning
    Write-Host ""
}

# ========================================
# STEP 1.5: FIX WEB.CONFIG FILES
# ========================================

Write-Host " STEP 1.5: Fixing web.config files..." -ForegroundColor $ColorInfo
Write-Host ""

# Force API web.config to framework-dependent startup (dotnet + dll arguments)
if (-not $WebOnly) {
    $apiWebConfig = Join-Path $OutputPath "API\web.config"
    if (Test-Path $apiWebConfig) {
        Write-Host "   Fixing API web.config..." -ForegroundColor Yellow
        $content = Get-Content $apiWebConfig -Raw
        $content = $content -replace 'processPath="\.\\MiGenteEnLinea\.API\.exe"', 'processPath="dotnet"'
        $content = $content -replace "processPath='\.\\MiGenteEnLinea\.API\.exe'", "processPath='dotnet'"
        $content = $content -replace 'arguments="\s*"', 'arguments=".\MiGenteEnLinea.API.dll"'
        $content = $content -replace "arguments='\s*'", "arguments='.\MiGenteEnLinea.API.dll'"
        Set-Content -Path $apiWebConfig -Value $content -NoNewline -Force -ErrorAction Stop
        Write-Host "    API web.config fixed (processPath=`"dotnet`", arguments=`".\\MiGenteEnLinea.API.dll`")" -ForegroundColor $ColorSuccess
    } else {
        Write-Host "     API web.config not found, skipping..." -ForegroundColor $ColorWarning
    }
}

# Force Web web.config to framework-dependent startup (dotnet + dll arguments)
if (-not $ApiOnly) {
    $webWebConfig = Join-Path $OutputPath "Web\web.config"
    if (Test-Path $webWebConfig) {
        Write-Host "   Fixing Web web.config..." -ForegroundColor Yellow
        $content = Get-Content $webWebConfig -Raw
        $content = $content -replace 'processPath="\.\\MiGenteEnLinea\.Web\.exe"', 'processPath="dotnet"'
        $content = $content -replace "processPath='\.\\MiGenteEnLinea\.Web\.exe'", "processPath='dotnet'"
        $content = $content -replace 'arguments="\s*"', 'arguments=".\MiGenteEnLinea.Web.dll"'
        $content = $content -replace "arguments='\s*'", "arguments='.\MiGenteEnLinea.Web.dll'"
        if ($WebOutOfProcessFallback) {
            $content = $content -replace 'hostingModel="inprocess"', 'hostingModel="outofprocess"'
            $content = $content -replace "hostingModel='inprocess'", "hostingModel='outofprocess'"
        } else {
            $content = $content -replace 'hostingModel="outofprocess"', 'hostingModel="inprocess"'
            $content = $content -replace "hostingModel='outofprocess'", "hostingModel='inprocess'"
        }
        Set-Content -Path $webWebConfig -Value $content -NoNewline -Force -ErrorAction Stop
        if ($WebOutOfProcessFallback) {
            Write-Host "    Web web.config fixed (processPath=`"dotnet`", arguments=`".\\MiGenteEnLinea.Web.dll`", hostingModel=`"outofprocess`")" -ForegroundColor $ColorSuccess
        } else {
            Write-Host "    Web web.config fixed (processPath=`"dotnet`", arguments=`".\\MiGenteEnLinea.Web.dll`", hostingModel=`"inprocess`")" -ForegroundColor $ColorSuccess
        }
    } else {
        Write-Host "     Web web.config not found, skipping..." -ForegroundColor $ColorWarning
    }
}

Write-Host ""
Write-Host " web.config files corrected!" -ForegroundColor $ColorSuccess
Write-Host ""

# ========================================
# STEP 1.6: DATABASE INIT (OPTIONAL)
# ========================================
if ($RunDbInit) {
    Write-Host " STEP 1.6: Running database initialization..." -ForegroundColor $ColorInfo
    Write-Host ""

    $dbInitArgs = @(
        "-Configuration", $Configuration
    )

    if (-not [string]::IsNullOrWhiteSpace($DbConnectionString)) {
        $dbInitArgs += @("-ConnectionString", $DbConnectionString)
    }

    $deployDbScript = Join-Path $ScriptRoot "scripts\deploy-db.ps1"
    & $deployDbScript @dbInitArgs

    if ($LASTEXITCODE -ne 0) {
        Write-Host " Database initialization failed." -ForegroundColor $ColorError
        exit 1
    }

    Write-Host ""
    Write-Host " Database initialization completed." -ForegroundColor $ColorSuccess
    Write-Host ""
}

# ========================================
# STEP 1.7: ARTIFACT INTEGRITY CHECK
# ========================================
Write-Host " STEP 1.7: Validating artifact integrity..." -ForegroundColor $ColorInfo
Write-Host ""
$integrityScript = Join-Path $ScriptRoot "scripts\validate-artifacts-integrity.ps1"
& $integrityScript
if ($LASTEXITCODE -ne 0) {
    Write-Host " Artifact integrity validation failed." -ForegroundColor $ColorError
    exit 1
}
Write-Host ""
Write-Host " Artifact integrity check passed." -ForegroundColor $ColorSuccess
Write-Host ""

# ========================================
# STEP 2: FTP UPLOAD
# ========================================

if ($SkipUpload) {
    Write-Host "  Skipping FTP upload" -ForegroundColor $ColorWarning
    Write-Host ""
    Write-Host " Artifacts location: $OutputPath" -ForegroundColor $ColorInfo
    exit 0
}

Write-Host " STEP 2: Uploading to FTP server..." -ForegroundColor $ColorInfo
Write-Host ""

$apiArtifactPath = Join-Path $OutputPath "API"
$webArtifactPath = Join-Path $OutputPath "Web"

$deployApi = -not $WebOnly
$deployWeb = -not $ApiOnly

if ($deployApi -and -not (Test-Path $apiArtifactPath)) {
    Write-Host "  API artifacts not found at: $apiArtifactPath" -ForegroundColor $ColorWarning
    Write-Host "  API upload will be skipped." -ForegroundColor $ColorWarning
    $deployApi = $false
}

if ($deployWeb -and -not (Test-Path $webArtifactPath)) {
    Write-Host "  Web artifacts not found at: $webArtifactPath" -ForegroundColor $ColorWarning
    Write-Host "  Web upload will be skipped." -ForegroundColor $ColorWarning
    $deployWeb = $false
}

if (-not $deployApi -and -not $deployWeb) {
    Write-Host " No artifacts available to upload. Build first or set a valid -OutputPath." -ForegroundColor $ColorError
    exit 1
}

# Check for WinSCP
$WinScpPath = "C:\Program Files (x86)\WinSCP\WinSCP.com"
if (-not (Test-Path $WinScpPath)) {
    $WinScpPath = "C:\Program Files\WinSCP\WinSCP.com"
}

if (-not (Test-Path $WinScpPath)) {
    Write-Host " WinSCP not found!" -ForegroundColor $ColorError
    Write-Host ""
    Write-Host "Please install WinSCP from: https://winscp.net/eng/download.php" -ForegroundColor Yellow
    Write-Host "Or install via Chocolatey: choco install winscp -y" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Artifacts are ready at: $OutputPath" -ForegroundColor $ColorInfo
    Write-Host "You can upload manually via FileZilla or WinSCP GUI" -ForegroundColor $ColorInfo
    exit 1
}

Write-Host "    WinSCP found: $WinScpPath" -ForegroundColor Gray
Write-Host ""

# Create WinSCP script
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$ScriptPath = Join-Path $OutputPath "winscp-deploy-$timestamp.txt"
$LogPath = Join-Path $OutputPath "winscp-upload-$timestamp.log"

$WinScpScript = @"
option batch abort
option confirm off
option transfer binary
option reconnecttime 120

# Connect to FTP server
open ftp://${FtpUsername}:${FtpPassword}@${FtpHost}/ -passive=on -timeout=120

"@

# Add API upload commands
if ($deployApi) {
    $WinScpScript += @"

# ========================================
# Upload API
# ========================================
lcd "$OutputPath\API"

# Create remote directories first (option batch continue = don't fail if exists)
option batch continue
mkdir $RemoteRoot
mkdir $RemoteRoot/api
mkdir $RemoteRoot/api/logs
mkdir $RemoteRoot/api/wwwroot
mkdir $RemoteRoot/api/wwwroot/uploads
mkdir $RemoteRoot/api/wwwroot/uploads/contratistas-fotos

# Now change to API directory
cd $RemoteRoot/api
option batch abort

# Upload all files (excluding runtime folders logs/uploads content)
put -filemask="|logs/;logs/*;wwwroot/uploads/;wwwroot/uploads/*" *

# Ensure logs folder exists by uploading a placeholder
option batch continue
cd logs
cd ../wwwroot/uploads
pwd
option batch abort

"@
    Write-Host "    API  $RemoteRoot/api/" -ForegroundColor Yellow
}

# Add Web upload commands
if ($deployWeb) {
    $WinScpScript += @"

# ========================================
# Upload Web
# ========================================
lcd "$OutputPath\Web"

# Create remote directories first (option batch continue = don't fail if exists)
option batch continue
mkdir $RemoteRoot
mkdir $RemoteRoot/web
mkdir $RemoteRoot/web/logs

# Now change to Web directory
cd $RemoteRoot/web
option batch abort

# Upload all files (excluding logs folder content)
put -filemask="|logs/;logs/*" *

# Ensure logs folder exists by uploading a placeholder
option batch continue
cd logs
option batch abort

"@
    Write-Host "    Web  $RemoteRoot/web/" -ForegroundColor Yellow
}

$WinScpScript += @"

# Close connection
close
exit
"@

# Save script
$WinScpScript | Out-File -FilePath $ScriptPath -Encoding ASCII

Write-Host ""
Write-Host "    Starting FTP upload..." -ForegroundColor Yellow
Write-Host "      Host: $FtpHost" -ForegroundColor Gray
Write-Host "      User: $FtpUsername" -ForegroundColor Gray
Write-Host ""

# Execute WinSCP
$process = Start-Process -FilePath $WinScpPath `
    -ArgumentList "/script=`"$ScriptPath`" /log=`"$LogPath`"" `
    -Wait -PassThru -NoNewWindow

# Check result
if ($process.ExitCode -eq 0) {
    Write-Host ""
    Write-Host " FTP Upload completed successfully!" -ForegroundColor $ColorSuccess
    Write-Host ""
    Write-Host "========================================" -ForegroundColor $ColorSuccess
    Write-Host "   DEPLOYMENT COMPLETE!" -ForegroundColor $ColorSuccess
    Write-Host "========================================" -ForegroundColor $ColorSuccess
    Write-Host ""
    Write-Host " Post-Deployment Checklist:" -ForegroundColor White
    Write-Host ""
    
    if ($deployApi) {
        Write-Host "   API:" -ForegroundColor $ColorInfo
        Write-Host "   1  Prepare API Website in myASP.NET AppManager (dedicated app pool)" -ForegroundColor Gray
        Write-Host "       Website domain/host header: api.yourdomain.com (and any alias used)" -ForegroundColor Gray
        Write-Host "       Physical Path: $RemoteRoot/api" -ForegroundColor Gray
        Write-Host "       Application Pool: No Managed Code (Integrated)" -ForegroundColor Gray
        Write-Host "   2  Test health endpoint: https://api.migenteenlinea.com/health" -ForegroundColor Gray
        Write-Host "   3  Ensure domain DNS points to myASP target before testing" -ForegroundColor Gray
        Write-Host ""
    }
    
    if ($deployWeb) {
        Write-Host "   Web:" -ForegroundColor $ColorInfo
        Write-Host "   4  Prepare Web Website in myASP.NET AppManager (different app pool than API)" -ForegroundColor Gray
        Write-Host "       Website domain/host header: www.yourdomain.com (and apex if used)" -ForegroundColor Gray
        Write-Host "       Physical Path: $RemoteRoot/web" -ForegroundColor Gray
        Write-Host "       Application Pool: No Managed Code (Integrated)" -ForegroundColor Gray
        if ($WebOutOfProcessFallback) {
            Write-Host "       Fallback mode active: Web hostingModel=outofprocess" -ForegroundColor Gray
        }
        Write-Host "   5  Test website: https://www.migenteenlinea.com/" -ForegroundColor Gray
        Write-Host ""
    }
    
    Write-Host "   Logs:" -ForegroundColor $ColorInfo
    Write-Host "   6  Check server logs via FTP:" -ForegroundColor Gray
    Write-Host "       $RemoteRoot/api/logs/stdout_*.log" -ForegroundColor Gray
    Write-Host "       $RemoteRoot/web/logs/stdout_*.log" -ForegroundColor Gray
    Write-Host ""
    Write-Host "   Database:" -ForegroundColor $ColorInfo
    Write-Host "   7  Verify connection to SQL5106.site4now.net" -ForegroundColor Gray
    Write-Host "       Test an API endpoint that hits database" -ForegroundColor Gray
    Write-Host ""
    
    Write-Host " Local files:" -ForegroundColor White
    Write-Host "    Artifacts: $OutputPath" -ForegroundColor Gray
    Write-Host "    Upload log: $LogPath" -ForegroundColor Gray
    Write-Host ""
    
    # Cleanup script file
    Remove-Item $ScriptPath -Force -ErrorAction SilentlyContinue
    
} else {
    Write-Host ""
    Write-Host " FTP Upload failed!" -ForegroundColor $ColorError
    Write-Host ""
    Write-Host "Check the log file for details:" -ForegroundColor Yellow
    Write-Host "$LogPath" -ForegroundColor White
    Write-Host ""
    
    if (Test-Path $LogPath) {
        Write-Host "Last 20 lines of log:" -ForegroundColor Yellow
        Write-Host ""
        Get-Content $LogPath | Select-Object -Last 20 | ForEach-Object {
            Write-Host $_ -ForegroundColor Gray
        }
    }
    
    Write-Host ""
    Write-Host "Common issues:" -ForegroundColor Yellow
    Write-Host " Check FTP credentials are correct" -ForegroundColor Gray
    Write-Host " Verify remote directory exists: $RemoteRoot" -ForegroundColor Gray
    Write-Host " Ensure passive mode is supported by your network/firewall" -ForegroundColor Gray
    Write-Host " Check if server is accessible: ping win8146.site4now.net" -ForegroundColor Gray
    
    exit 1
}

Write-Host ""
Write-Host " Ready to test your deployment!" -ForegroundColor $ColorSuccess
Write-Host ""

