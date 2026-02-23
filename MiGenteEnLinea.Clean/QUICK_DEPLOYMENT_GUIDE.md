# Quick Deployment Guide (myASP)

## Standard release

Run from `MiGenteEnLinea.Clean`:

```powershell
powershell -ExecutionPolicy Bypass -File .\publish-and-deploy-ftp.ps1 -Configuration Release
```

## If you only want to build artifacts first

```powershell
powershell -ExecutionPolicy Bypass -File .\publish-and-deploy-ftp.ps1 -Configuration Release -SkipUpload
```

## If build is done and you only need upload

```powershell
powershell -ExecutionPolicy Bypass -File .\publish-and-deploy-ftp.ps1 -Configuration Release -SkipBuild
```

## If myASP cannot split app pools yet (`500.35`)

```powershell
powershell -ExecutionPolicy Bypass -File .\publish-and-deploy-ftp.ps1 -Configuration Release -WebOutOfProcessFallback
```

## Database release step (explicit)

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\deploy-db.ps1 -Configuration Release
```

## Mandatory checks after deploy

1. API health:
- `https://api.yourdomain.com/health`

2. Web home:
- `https://www.yourdomain.com/`

3. Logs:
- `/MigenteApi/api/logs/stdout_*.log`
- `/MigenteApi/web/logs/stdout_*.log`

## Mandatory IIS/myASP setup

1. API website -> `/MigenteApi/api` -> dedicated app pool (`No Managed Code`)
2. Web website -> `/MigenteApi/web` -> different dedicated app pool (`No Managed Code`)
3. Correct host headers/bindings for both domains
