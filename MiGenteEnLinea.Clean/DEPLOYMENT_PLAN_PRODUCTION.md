# 🚀 Plan de Despliegue a Producción - MiGente En Línea

**Objetivo:** Compilar y publicar binarios para el VPS "myaspnet" (IIS + Windows Server)

**Fecha:** Febrero 9, 2026  
**Versión:** 1.0.0  
**Arquitectura:** ASP.NET Core 8.0 - Framework-Dependent Deployment

---

## 📋 **FASE 1: Configuración de Production**

### **1.1 Crear appsettings.Production.json - API**

**Ubicación:** `src/Presentation/MiGenteEnLinea.API/appsettings.Production.json`

**Configuraciones críticas a actualizar:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_VPS_SQL_SERVER;Database=db_a9f8ff_migente;User Id=YOUR_SQL_USER;Password=YOUR_SQL_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true;Encrypt=True"
  },
  "Jwt": {
    "SecretKey": "PRODUCTION_SECRET_KEY_CHANGE_THIS_TO_RANDOM_64_CHARS_MINIMUM",
    "Issuer": "MiGenteEnLinea.API",
    "Audience": "MiGenteEnLinea.Client",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "Cardnet": {
    "ProductionUrl": "https://ecommerce.cardnet.com.do/api/payment/transactions/sales",
    "ProductionIdempotencyUrl": "https://ecommerce.cardnet.com.do/api/payment/idenpotency-keys",
    "MerchantId": "YOUR_CARDNET_MERCHANT_ID",
    "TerminalId": "YOUR_CARDNET_TERMINAL_ID",
    "ApiKey": "YOUR_CARDNET_API_KEY",
    "IsTest": false
  },
  "EmailSettings": {
    "SmtpServer": "YOUR_SMTP_SERVER",
    "SmtpPort": 587,
    "Username": "YOUR_SMTP_USERNAME",
    "Password": "YOUR_SMTP_PASSWORD",
    "FromEmail": "noreply@migenteenlinea.com",
    "FromName": "MiGente En Línea",
    "EnableSsl": true
  },
  "CorsConfiguration": {
    "AllowedOrigins": [
      "https://www.migenteenlinea.com",
      "https://migenteenlinea.com"
    ],
    "AllowCredentials": true
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning",
      "Override": {
        "Microsoft": "Error",
        "Microsoft.Hosting.Lifetime": "Information",
        "Microsoft.EntityFrameworkCore": "Error"
      }
    }
  },
  "AllowedHosts": "*.migenteenlinea.com;myaspnet"
}
```

---

### **1.2 Crear appsettings.Production.json - Web**

**Ubicación:** `src/Presentation/MiGenteEnLinea.Web/appsettings.Production.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Error"
    }
  },
  "AllowedHosts": "*.migenteenlinea.com;myaspnet",
  "ApiConfiguration": {
    "BaseUrl": "https://api.migenteenlinea.com/api",
    "StaticFilesBaseUrl": "https://api.migenteenlinea.com",
    "TimeoutSeconds": 30,
    "RetryAttempts": 3,
    "HealthCheckEndpoint": "/health",
    "EnableRequestLogging": false,
    "HealthCheckTimeoutMs": 5000,
    "UserAgent": "MiGenteEnLinea.Web/1.0"
  }
}
```

**NOTA IMPORTANTE:** Si API y Web están en el mismo servidor, puedes usar URLs internas:
- API: `http://localhost:5015/api` (interno, más rápido)
- Static Files: `https://www.migenteenlinea.com` (público, para imágenes)

---

## 📋 **FASE 2: Configuración de IIS**

### **2.1 Crear web.config - API**

**Ubicación:** `src/Presentation/MiGenteEnLinea.API/web.config`

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" 
                  arguments=".\MiGenteEnLinea.API.dll" 
                  stdoutLogEnabled="true" 
                  stdoutLogFile=".\logs\stdout" 
                  hostingModel="inprocess" 
                  forwardWindowsAuthToken="false">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
        </environmentVariables>
      </aspNetCore>
      
      <!-- Allow large file uploads (100MB max) -->
      <security>
        <requestFiltering>
          <requestLimits maxAllowedContentLength="104857600" />
        </requestFiltering>
      </security>
      
      <!-- Static files configuration -->
      <staticContent>
        <mimeMap fileExtension=".json" mimeType="application/json" />
        <mimeMap fileExtension=".woff2" mimeType="font/woff2" />
      </staticContent>
      
      <!-- Compression -->
      <urlCompression doStaticCompression="true" doDynamicCompression="true" />
      
      <!-- HTTPS Redirect -->
      <rewrite>
        <rules>
          <rule name="HTTPS Redirect" stopProcessing="true">
            <match url="(.*)" />
            <conditions>
              <add input="{HTTPS}" pattern="^OFF$" />
            </conditions>
            <action type="Redirect" url="https://{HTTP_HOST}/{R:1}" redirectType="Permanent" />
          </rule>
        </rules>
      </rewrite>
    </system.webServer>
  </location>
</configuration>
```

---

### **2.2 Crear web.config - Web**

**Ubicación:** `src/Presentation/MiGenteEnLinea.Web/web.config`

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" 
                  arguments=".\MiGenteEnLinea.Web.dll" 
                  stdoutLogEnabled="true" 
                  stdoutLogFile=".\logs\stdout" 
                  hostingModel="inprocess" 
                  forwardWindowsAuthToken="false">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
        </environmentVariables>
      </aspNetCore>
      
      <!-- Cache configuration for static files -->
      <staticContent>
        <clientCache cacheControlMode="UseMaxAge" cacheControlMaxAge="7.00:00:00" />
      </staticContent>
      
      <!-- Compression -->
      <urlCompression doStaticCompression="true" doDynamicCompression="true" />
      
      <!-- HTTPS Redirect -->
      <rewrite>
        <rules>
          <rule name="HTTPS Redirect" stopProcessing="true">
            <match url="(.*)" />
            <conditions>
              <add input="{HTTPS}" pattern="^OFF$" />
            </conditions>
            <action type="Redirect" url="https://{HTTP_HOST}/{R:1}" redirectType="Permanent" />
          </rule>
        </rules>
      </rewrite>
    </system.webServer>
  </location>
</configuration>
```

---

## 📋 **FASE 3: Scripts de Publicación**

### **3.1 Crear script de publicación - PowerShell**

**Ubicación:** `MiGenteEnLinea.Clean/publish-production.ps1`

```powershell
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
}

if (Test-Path $WebDllPath) {
    $WebHash = (Get-FileHash -Path $WebDllPath -Algorithm SHA256).Hash
    $WebHash | Out-File -FilePath "$WebOutput\Web.checksum.txt"
}

Write-Host "✅ Checksums generados" -ForegroundColor Green
Write-Host ""

# Resumen
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ✅ PUBLICACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📂 Ubicaciones:" -ForegroundColor White
Write-Host "   API: $ApiOutput" -ForegroundColor Gray
Write-Host "   Web: $WebOutput" -ForegroundColor Gray
Write-Host ""
Write-Host "📋 Próximos pasos:" -ForegroundColor White
Write-Host "   1. Revisar appsettings.Production.json en ambas carpetas" -ForegroundColor Gray
Write-Host "   2. Actualizar ConnectionStrings con credenciales reales" -ForegroundColor Gray
Write-Host "   3. Comprimir carpetas API y Web en archivos .zip" -ForegroundColor Gray
Write-Host "   4. Subir al VPS myaspnet" -ForegroundColor Gray
Write-Host "   5. Configurar sitios en IIS" -ForegroundColor Gray
Write-Host ""
Write-Host "⚠️  IMPORTANTE: Configurar permisos de escritura en wwwroot/uploads" -ForegroundColor Yellow
Write-Host ""
