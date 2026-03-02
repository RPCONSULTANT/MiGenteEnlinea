# MiGenteEnLinea E2E (Playwright)

## Required env vars
- `E2E_WEB_BASE_URL` (default: `http://plattaformv2.migenteenlinea.do`)
- `E2E_API_BASE_URL` (default: `http://api2.migenteenlinea.do`)
- `E2E_USER_EMPLEADOR_EMAIL`
- `E2E_USER_EMPLEADOR_PASSWORD`
- `E2E_USER_CONTRATISTA_EMAIL`
- `E2E_USER_CONTRATISTA_PASSWORD`
- `E2E_USER_ADMIN_EMAIL`
- `E2E_USER_ADMIN_PASSWORD`
- Alias supported:
  - `E2E_EMAIL_EMPLEADOR` / `E2E_PASSWORD_EMPLEADOR`
  - `E2E_EMAIL_CONTRATISTA` / `E2E_PASSWORD_CONTRATISTA`
  - `E2E_EMAIL_ADMIN` / `E2E_PASSWORD_ADMIN`
- `E2E_SEED_KEY` (optional, required for admin seed tests)
- `E2E_ALLOW_WRITE` (`false` for smoke, `true` for full)
- `E2E_RUN_ID` (optional)

## Commands
- `npm run test:e2e:smoke`
- `npm run test:e2e:full`
- `npm run test:e2e:all`
- `npm run report:summary`
- `npm run report:open`

## Tags
- `@smoke`, `@full`
- `@auth`, `@empleador`, `@contratista`, `@pagos`, `@suscripciones`, `@nomina`, `@catalogos`, `@dashboard`, `@admin`
