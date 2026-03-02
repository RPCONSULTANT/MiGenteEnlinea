# Environment Variables Matrix (Local, Staging, Production)

This document is the operational source of truth for environment variable governance.

## Scope
- API (`MiGenteEnLinea.API`)
- Web (`MiGenteEnLinea.Web`)
- E2E (`tests/MiGenteEnLinea.E2E`)

## Criticality
- `CRITICA-ARRANQUE`: app must fail at startup if missing or placeholder.
- `CRITICA-RUN`: app can start but runtime features or pipeline checks fail.
- `OPCIONAL`: metadata/traceability.

## Canonical Inventory
- Full machine-readable inventory:
- [expected-env-vars.json](/c:/Users/Ray/Documents/MiGenteEnlinea/MiGenteEnLinea.Clean/scripts/env/expected-env-vars.json)
- Editable matrix template (no secrets):
- [environment-matrix.template.csv](/c:/Users/Ray/Documents/MiGenteEnlinea/MiGenteEnLinea.Clean/docs/environment/environment-matrix.template.csv)

## Server-Only Secret Policy
- Do not store production secrets in repo files.
- Configure values directly in server environment variables (IIS/app pool level).
- Keep `appsettings*.json` for non-sensitive defaults only.

## Naming and Format Rules
- Use .NET hierarchical naming with `__` (double underscore).
- Use array prefixes with indexes (`Name__0`, `Name__1`, ...).
- Use lowercase booleans: `true`, `false`.
- Use integer numbers without quotes.
- Use absolute URLs with scheme.

## Validation Workflow
1. Validate required vars:
```powershell
./scripts/env/Validate-RequiredEnv.ps1 -Component API
./scripts/env/Validate-RequiredEnv.ps1 -Component Web
./scripts/env/Validate-RequiredEnv.ps1 -Component E2E
```
2. Generate env snapshot (names only):
```powershell
./scripts/env/Get-EnvSnapshot.ps1 -Component All
```
3. Run pre-deploy checks:
```powershell
./scripts/env/Invoke-PreDeployChecks.ps1 -EnvironmentName Production -ApiHealthUrl "https://api2.migenteenlinea.do/health" -WebUrl "https://plattaformv2.migenteenlinea.do"
```

## Promotion Gate
- Block deploy if any `CRITICA-ARRANQUE` variable is missing.
- Block deploy if E2E smoke fails with missing environment variables.
- Keep last successful snapshot and summary in deployment evidence.
