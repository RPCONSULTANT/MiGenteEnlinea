# myASP Website Prep Checklist (API + Web)

This checklist is the deployment baseline for myASP with separate websites.
For full release workflow, rollback, and error map, see `DEPLOYMENT_GUIDE_FTP_MYASPNET.md`.

## 1) Publish artifacts

Run from `MiGenteEnLinea.Clean`:

```powershell
powershell -File .\publish-and-deploy-ftp.ps1 -Configuration Release
```

If you hit IIS `500.35` and cannot separate app pools immediately, use fallback for Web:

```powershell
powershell -File .\publish-and-deploy-ftp.ps1 -Configuration Release -WebOutOfProcessFallback
```

If you only want local artifacts (no upload):

```powershell
powershell -File .\publish-and-deploy-ftp.ps1 -Configuration Release -SkipUpload
```

## 2) Remote folder layout

The script uploads to:

- API: `/MigenteApi/api`
- Web: `/MigenteApi/web`

## 3) myASP AppManager websites

Create or validate two websites:

1. API website
- Host header/domain: `api.yourdomain.com`
- Add all aliases actually used (including temporary `*.atempurl.com` if testing there)
- Physical path: `/MigenteApi/api`
- App Pool: dedicated `No Managed Code` pool (not shared with Web)

2. Web website
- Host header/domain: `www.yourdomain.com` (and apex if used)
- Add all aliases actually used
- Physical path: `/MigenteApi/web`
- App Pool: different dedicated `No Managed Code` pool (not shared with API)

Important:
- `500.35` means API and Web are running in the same app pool with in-process hosting.
- Preferred fix: separate app pools.
- Temporary fallback: keep API in-process and publish Web with `-WebOutOfProcessFallback`.

## 4) DNS

- `api` subdomain (A or CNAME) must point to the myASP target.
- `www` (and apex if used) must point to the correct myASP target.
- Wait for propagation, then recycle both websites.

## 5) Production app settings

1. API: `src/Presentation/MiGenteEnLinea.API/appsettings.Production.json`
- `AllowedHosts` should be `"*"`
- `DatabaseInitialization` stays disabled in production:
  - `ApplyMigrationsOnStartup = false`
  - `RunCatalogSeedOnStartup = false`
  - `RunDemoSeedOnStartup = false`

2. Web: `src/Presentation/MiGenteEnLinea.Web/appsettings.Production.json`
- `AllowedHosts` should be `"*"`
- `ApiConfiguration.BaseUrl` must be the real API domain (not localhost), for example:
  - `https://apimigente.migenteenlinea.do/api`

## 5.1) HTTPS redirect note

- Do not add IIS `<rewrite>` rules inside app `web.config` unless hosting confirms URL Rewrite module support.
- Keep app `web.config` minimal.
- Enforce HTTP->HTTPS redirect from myASP domain/bindings configuration.

## 6) Database release-first flow

Run DB deployment as an explicit release step:

```powershell
powershell -File .\scripts\deploy-db.ps1 -Configuration Release
```

This generates and applies idempotent EF migrations and then runs `scripts/seed-catalogs.sql`.

## 7) Smoke tests

1. API health:
- `https://api.yourdomain.com/health` returns `200`

2. API runtime:
- `stdout` logs are created under API `logs/`
- no startup crash loop

3. Web runtime:
- `https://www.yourdomain.com/` returns `200`
- Web calls API at the configured production `BaseUrl`

## 8) Common failures

1. `400 Bad Request - Invalid Hostname`
- Host header/binding mismatch in myASP website config
- Domain alias not added to the right website
- DNS not propagated yet

2. `500` with blank page
- check `logs/stdout_*.log` in API/Web folders
- verify website physical path points to the correct published folder
- verify DLL in root matches `web.config` arguments

3. `500.35 ASP.NET Core does not support multiple apps in the same app pool`
- assign different app pools to API and Web websites
- recycle both websites/pools
- if hosting panel does not allow pool split, redeploy Web with `-WebOutOfProcessFallback`
