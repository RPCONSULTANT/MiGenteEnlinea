# MiGenteEnLinea - Master Deployment Guide (myASP)

This is the canonical deployment guide for myASP.

## 1) Fixed architecture

- API website and Web website are deployed separately.
- API remote path: `/MigenteApi/api`
- Web remote path: `/MigenteApi/web`
- Keep app `web.config` minimal (no IIS `<rewrite>` block in app config).
- HTTP->HTTPS redirect is handled at myASP hosting/bindings level.
- Preferred hosting model:
  - API: `inprocess`
  - Web: `inprocess`
- Required app pools:
  - API in dedicated pool
  - Web in different dedicated pool
  - both with `No Managed Code`

## 2) Canonical commands

Run all commands from `MiGenteEnLinea.Clean`.

1. Build + publish artifacts only:

```powershell
powershell -ExecutionPolicy Bypass -File .\publish-and-deploy-ftp.ps1 -Configuration Release -SkipUpload
```

2. Full deploy (build + FTP upload):

```powershell
powershell -ExecutionPolicy Bypass -File .\publish-and-deploy-ftp.ps1 -Configuration Release
```

3. Upload only (reuse existing artifacts):

```powershell
powershell -ExecutionPolicy Bypass -File .\publish-and-deploy-ftp.ps1 -Configuration Release -SkipBuild
```

4. Partial deploy:

```powershell
# API only
powershell -ExecutionPolicy Bypass -File .\publish-and-deploy-ftp.ps1 -Configuration Release -ApiOnly

# Web only
powershell -ExecutionPolicy Bypass -File .\publish-and-deploy-ftp.ps1 -Configuration Release -WebOnly
```

5. Fallback for IIS `500.35` when pool split is not possible:

```powershell
powershell -ExecutionPolicy Bypass -File .\publish-and-deploy-ftp.ps1 -Configuration Release -WebOutOfProcessFallback
```

6. Explicit DB release step:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\deploy-db.ps1 -Configuration Release
```

## 3) myASP website preparation

1. API website:
- host header: `api.yourdomain.com` (+ aliases you use)
- physical path: `/MigenteApi/api`
- app pool: dedicated, `No Managed Code`

2. Web website:
- host header: `www.yourdomain.com` (+ apex/aliases if used)
- physical path: `/MigenteApi/web`
- app pool: dedicated and different from API, `No Managed Code`

3. DNS:
- `api` and `www` must point to myASP target
- wait for propagation

4. recycle both websites/app pools after changes.

## 4) Production configuration baseline

1. API `appsettings.Production.json`:
- `AllowedHosts` = `"*"`
- `DatabaseInitialization`:
  - `ApplyMigrationsOnStartup = false`
  - `RunCatalogSeedOnStartup = false`
  - `RunDemoSeedOnStartup = false`

2. Web `appsettings.Production.json`:
- `AllowedHosts` = `"*"`
- `ApiConfiguration.BaseUrl` points to real API domain, example:
  - `https://apimigente.migenteenlinea.do/api`

## 5) Runtime validation (required)

1. API health:
- `https://api.yourdomain.com/health` must return `200`

2. API root:
- `https://api.yourdomain.com/` should load Swagger if enabled in the deployed build

3. Web:
- `https://www.yourdomain.com/` must return `200`

4. Logs:
- API: `/MigenteApi/api/logs/stdout_*.log`
- Web: `/MigenteApi/web/logs/stdout_*.log`

## 6) Error map

1. `400 Bad Request - Invalid Hostname`
- wrong host header/binding
- domain alias missing on the target website

2. `500.35`
- API and Web sharing same app pool in `inprocess`
- fix by splitting pools or use `-WebOutOfProcessFallback`

3. `404`
- site/path mapping mismatch
- testing wrong URL (use `/health` for API validation)

4. `500`
- app runtime issue, check stdout logs and DB connectivity

## 7) Rollback

1. Re-upload last known-good API artifact to `/MigenteApi/api`.
2. Re-upload last known-good Web artifact to `/MigenteApi/web`.
3. If fallback was used, return Web to `inprocess` after pool split is available.
4. Recycle both sites/pools.
5. Re-check `/health` and `/`.

## 8) Release gate checklist

- build/publish completed without errors
- API and Web artifacts both present
- sites mapped to correct physical paths
- pools separated (or fallback enabled)
- API `/health` = 200
- Web `/` = 200
- stdout logs are generated and clean
