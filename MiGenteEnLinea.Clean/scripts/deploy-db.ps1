param(
    [string]$Configuration = "Release",
    [string]$ConnectionString = "",
    [string]$MigrationScriptOutput = "artifacts/sql/migration-idempotent.sql",
    [switch]$SkipMigrationScriptGeneration,
    [switch]$SkipMigrationApply,
    [switch]$SkipCatalogSeed
)

$ErrorActionPreference = "Stop"

function Get-ConnectionString {
    param([string]$ProvidedConnectionString)

    if (-not [string]::IsNullOrWhiteSpace($ProvidedConnectionString)) {
        return $ProvidedConnectionString
    }

    $prodConfigPath = "src/Presentation/MiGenteEnLinea.API/appsettings.Production.json"
    if (-not (Test-Path $prodConfigPath)) {
        throw "No se encontro $prodConfigPath y no se recibio -ConnectionString."
    }

    $config = Get-Content $prodConfigPath -Raw | ConvertFrom-Json
    $fromFile = $config.ConnectionStrings.DefaultConnection

    if ([string]::IsNullOrWhiteSpace($fromFile)) {
        throw "ConnectionStrings:DefaultConnection esta vacia en $prodConfigPath."
    }

    return $fromFile
}

function Get-SqlCmdArgsFromConnectionString {
    param([string]$Cs)

    $builder = New-Object System.Data.SqlClient.SqlConnectionStringBuilder($Cs)

    if ([string]::IsNullOrWhiteSpace($builder.DataSource)) {
        throw "Connection string sin Data Source/Server."
    }

    if ([string]::IsNullOrWhiteSpace($builder.InitialCatalog)) {
        throw "Connection string sin Initial Catalog/Database."
    }

    if ([string]::IsNullOrWhiteSpace($builder.UserID)) {
        throw "Connection string sin User Id."
    }

    return @{
        Server = $builder.DataSource
        Database = $builder.InitialCatalog
        User = $builder.UserID
        Password = $builder.Password
    }
}

if (-not (Test-Path "MiGenteEnLinea.Clean.sln")) {
    throw "Ejecuta este script desde la carpeta MiGenteEnLinea.Clean."
}

$resolvedConnectionString = Get-ConnectionString -ProvidedConnectionString $ConnectionString
$sql = Get-SqlCmdArgsFromConnectionString -Cs $resolvedConnectionString

$migrationScriptFullPath = Join-Path (Get-Location) $MigrationScriptOutput
$migrationScriptDir = Split-Path $migrationScriptFullPath -Parent
New-Item -Path $migrationScriptDir -ItemType Directory -Force | Out-Null

Write-Host "=== Database Deploy ===" -ForegroundColor Cyan
Write-Host "Server: $($sql.Server)" -ForegroundColor Gray
Write-Host "Database: $($sql.Database)" -ForegroundColor Gray
Write-Host "Migration script: $migrationScriptFullPath" -ForegroundColor Gray
Write-Host ""

if (-not $SkipMigrationScriptGeneration) {
    Write-Host "Generating idempotent migration script..." -ForegroundColor Yellow
    dotnet ef migrations script --idempotent `
        --project src/Infrastructure/MiGenteEnLinea.Infrastructure `
        --startup-project src/Presentation/MiGenteEnLinea.API `
        --output $migrationScriptFullPath `
        --configuration $Configuration

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet ef migrations script fallo."
    }
}

if (Get-Command sqlcmd -ErrorAction SilentlyContinue) {
    if (-not $SkipMigrationApply) {
        Write-Host "Applying migrations with sqlcmd..." -ForegroundColor Yellow
        sqlcmd -S $sql.Server -d $sql.Database -U $sql.User -P $sql.Password -b -i $migrationScriptFullPath
        if ($LASTEXITCODE -ne 0) {
            throw "Fallo aplicando migraciones con sqlcmd."
        }
    }

    if (-not $SkipCatalogSeed) {
        Write-Host "Applying catalog seed script..." -ForegroundColor Yellow
        sqlcmd -S $sql.Server -d $sql.Database -U $sql.User -P $sql.Password -b -i "scripts/seed-catalogs.sql"
        if ($LASTEXITCODE -ne 0) {
            throw "Fallo aplicando seed de catalogos."
        }
    }
}
else {
    throw "sqlcmd no esta instalado. Instala SQLCMD o ejecuta scripts manualmente."
}

Write-Host ""
Write-Host "Database deploy completed successfully." -ForegroundColor Green
