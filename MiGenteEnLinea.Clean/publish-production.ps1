# ========================================
# Script de Publicación a Producción
# MiGente En Línea - VPS myaspnet
# ========================================

param(
    [string]$Configuration = "Release",
    [string]$OutputPath = "C:\Publish\MiGenteEnlinea"
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  MiGente En Línea - Publicación" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configuración
$ApiProject = "src\Presentation\MiGenteEnLinea.API\MiGenteEnLinea.API.csproj"
$WebProject = "src\Presentation\MiGenteEnLinea.Web\MiGenteEnLinea.Web.csproj"
$ApiOutput = "$OutputPath\API"
$WebOutput = "$OutputPath\Web"

# Verificar que estamos en la carpeta correcta
if (-not (Test-Path "MiGenteEnLinea.Clean.sln")) {
    Write-Host "❌ Error: Ejecuta este script desde la carpeta MiGenteEnLinea.Clean" -ForegroundColor Red
    exit 1
}

# Limpiar outputs anteriores
Write-Host "🧹 Limpiando carpetas de publicación anteriores..." -ForegroundColor Yellow
if (Test-Path $ApiOutput) { Remove-Item -Path $ApiOutput -Recurse -Force }
if (Test-Path $WebOutput) { Remove-Item -Path $WebOutput -Recurse -Force }
New-Item -ItemType Directory -Path $ApiOutput -Force | Out-Null
New-Item -ItemType Directory -Path $WebOutput -Force | Out-Null

Write-Host "✅ Carpetas preparadas" -ForegroundColor Green
Write-Host ""

# Publicar API
Write-Host "📦 Publicando MiGenteEnLinea.API..." -ForegroundColor Yellow
dotnet publish $ApiProject `
    --configuration $Configuration `
    --output $ApiOutput `
    --runtime win-x64 `
    --self-contained false `
    --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error al publicar API" -ForegroundColor Red
    exit 1
}

Write-Host "✅ API publicado exitosamente" -ForegroundColor Green
Write-Host ""

# Publicar Web
Write-Host "📦 Publicando MiGenteEnLinea.Web..." -ForegroundColor Yellow
dotnet publish $WebProject `
    --configuration $Configuration `
    --output $WebOutput `
    --runtime win-x64 `
    --self-contained false `
    --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Error al publicar Web" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Web publicado exitosamente" -ForegroundColor Green
Write-Host ""

# Crear carpetas necesarias
Write-Host "📁 Creando carpetas requeridas..." -ForegroundColor Yellow
New-Item -ItemType Directory -Path "$ApiOutput\logs" -Force | Out-Null
New-Item -ItemType Directory -Path "$WebOutput\logs" -Force | Out-Null
New-Item -ItemType Directory -Path "$ApiOutput\wwwroot\uploads\contratistas-fotos" -Force | Out-Null

# Crear archivo .gitkeep en uploads
New-Item -ItemType File -Path "$ApiOutput\wwwroot\uploads\contratistas-fotos\.gitkeep" -Force | Out-Null

Write-Host "✅ Estructura de carpetas creada" -ForegroundColor Green
Write-Host ""

# Generar checksums
Write-Host "🔐 Generando checksums..." -ForegroundColor Yellow
$ApiDllPath = "$ApiOutput\MiGenteEnLinea.API.dll"
$WebDllPath = "$WebOutput\MiGenteEnLinea.Web.dll"

if (Test-Path $ApiDllPath) {
    $ApiHash = (Get-FileHash -Path $ApiDllPath -Algorithm SHA256).Hash
    $ApiHash | Out-File -FilePath "$ApiOutput\API.checksum.txt"
    Write-Host "   API Hash: $ApiHash" -ForegroundColor Gray
}

if (Test-Path $WebDllPath) {
    $WebHash = (Get-FileHash -Path $WebDllPath -Algorithm SHA256).Hash
    $WebHash | Out-File -FilePath "$WebOutput\Web.checksum.txt"
    Write-Host "   Web Hash: $WebHash" -ForegroundColor Gray
}

Write-Host "✅ Checksums generados" -ForegroundColor Green
Write-Host ""

# Crear archivo ZIP para API
Write-Host "📦 Comprimiendo archivos..." -ForegroundColor Yellow
$ApiZipPath = "$OutputPath\MiGenteEnLinea-API.zip"
$WebZipPath = "$OutputPath\MiGenteEnLinea-Web.zip"

if (Test-Path $ApiZipPath) { Remove-Item $ApiZipPath -Force }
if (Test-Path $WebZipPath) { Remove-Item $WebZipPath -Force }

Compress-Archive -Path "$ApiOutput\*" -DestinationPath $ApiZipPath -Force
Compress-Archive -Path "$WebOutput\*" -DestinationPath $WebZipPath -Force

Write-Host "✅ Archivos ZIP creados" -ForegroundColor Green
Write-Host ""

# Generar información de la build
$BuildInfo = @"
========================================
MiGente En Línea - Build Information
========================================

Build Date: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
Configuration: $Configuration
.NET Version: $(dotnet --version)

API Output: $ApiOutput
Web Output: $WebOutput

API ZIP: $ApiZipPath ($('{0:N2} MB' -f ((Get-Item $ApiZipPath).Length / 1MB)))
Web ZIP: $WebZipPath ($('{0:N2} MB' -f ((Get-Item $WebZipPath).Length / 1MB)))

========================================
"@

$BuildInfo | Out-File -FilePath "$OutputPath\BUILD_INFO.txt"

# Resumen
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ✅ PUBLICACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📂 Ubicaciones:" -ForegroundColor White
Write-Host "   API: $ApiOutput" -ForegroundColor Gray
Write-Host "   Web: $WebOutput" -ForegroundColor Gray
Write-Host "   ZIP API: $ApiZipPath" -ForegroundColor Gray
Write-Host "   ZIP Web: $WebZipPath" -ForegroundColor Gray
Write-Host ""
Write-Host "📋 Próximos pasos:" -ForegroundColor White
Write-Host "   1️⃣  Revisar appsettings.Production.json en ambas carpetas" -ForegroundColor Gray
Write-Host "   2️⃣  Actualizar ConnectionStrings con credenciales del VPS" -ForegroundColor Gray
Write-Host "   3️⃣  Actualizar JWT SecretKey con valor aleatorio seguro" -ForegroundColor Gray
Write-Host "   4️⃣  Subir archivos ZIP al VPS myaspnet" -ForegroundColor Gray
Write-Host "   5️⃣  Descomprimir en carpetas del IIS" -ForegroundColor Gray
Write-Host "   6️⃣  Configurar sitios en IIS (API y Web)" -ForegroundColor Gray
Write-Host "   7️⃣  Verificar permisos de escritura en wwwroot/uploads" -ForegroundColor Gray
Write-Host ""
Write-Host "⚠️  IMPORTANTE: Configurar App Pools en IIS:" -ForegroundColor Yellow
Write-Host "   - .NET CLR Version: No Managed Code" -ForegroundColor Gray
Write-Host "   - Managed Pipeline Mode: Integrated" -ForegroundColor Gray
Write-Host ""
Write-Host "🌐 URLs sugeridas para IIS:" -ForegroundColor White
Write-Host "   API: https://api.migenteenlinea.com (Puerto 443)" -ForegroundColor Gray
Write-Host "   Web: https://www.migenteenlinea.com (Puerto 443)" -ForegroundColor Gray
Write-Host ""
