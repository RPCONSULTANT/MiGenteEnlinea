param(
    [Parameter(Mandatory = $true)]
    [string]$ConnectionString,
    [string]$OutputFolder = ".\artifacts\migration\empleadores-fotos",
    [switch]$WhatIf
)

$ErrorActionPreference = "Stop"

Add-Type -AssemblyName System.Data

$absoluteOutput = [System.IO.Path]::GetFullPath($OutputFolder)
New-Item -ItemType Directory -Path $absoluteOutput -Force | Out-Null

$csvPath = Join-Path $absoluteOutput "empleadores-fotos-mapping.csv"
if (-not (Test-Path -LiteralPath $csvPath)) {
    "ofertanteID,userID,fileName,relativeUrl,status" | Out-File -FilePath $csvPath -Encoding utf8
}

$connection = New-Object System.Data.SqlClient.SqlConnection $ConnectionString
$command = $connection.CreateCommand()
$command.CommandText = @"
SELECT ofertanteID, userID, foto
FROM Ofertantes
WHERE foto IS NOT NULL AND DATALENGTH(foto) > 0
ORDER BY ofertanteID;
"@

try {
    $connection.Open()
    $reader = $command.ExecuteReader()

    while ($reader.Read()) {
        $ofertanteId = $reader.GetInt32(0)
        $userId = $reader.GetString(1)
        $fotoBytes = [byte[]]$reader["foto"]

        $safeUserId = ($userId -replace "[^a-zA-Z0-9_-]", "_").ToLowerInvariant()
        $fileName = "empleador_${ofertanteId}_${safeUserId}.jpg"
        $targetPath = Join-Path $absoluteOutput $fileName
        $relativeUrl = "/uploads/empleadores-fotos/$fileName"

        if ($WhatIf) {
            Add-Content -Path $csvPath -Value "$ofertanteId,$userId,$fileName,$relativeUrl,whatif"
            continue
        }

        if (Test-Path -LiteralPath $targetPath) {
            Add-Content -Path $csvPath -Value "$ofertanteId,$userId,$fileName,$relativeUrl,skipped_exists"
            continue
        }

        [System.IO.File]::WriteAllBytes($targetPath, $fotoBytes)
        Add-Content -Path $csvPath -Value "$ofertanteId,$userId,$fileName,$relativeUrl,migrated"
    }

    Write-Host "Legacy foto export completed: $absoluteOutput" -ForegroundColor Green
    Write-Host "Mapping file: $csvPath" -ForegroundColor Gray
}
finally {
    if ($reader) { $reader.Dispose() }
    $command.Dispose()
    $connection.Dispose()
}
