# Auth Runbook

## Production defaults
- Flow: legacy activation (`register` without password, `activate` creates password).
- Public web base: `https://plattaformv2.migenteenlinea.do`.
- API/Web deployed on separate sites.

## Health checklist
1. API responds at `/health`.
2. Auth endpoints return expected codes:
   - `POST /api/auth/register` -> 201
   - `POST /api/auth/activate` -> 200
   - `POST /api/auth/login` -> 200/401
   - `POST /api/auth/forgot-password` -> 200
   - `POST /api/auth/reset-password` -> 200/400
3. Web views available:
   - `/Auth/Login`
   - `/Auth/Registrar`
   - `/Auth/Activar`
   - `/Auth/ResetPassword`

## Troubleshooting
- 400 on register:
  - validate required fields (`email`, `nombre`, `apellido`, `tipo`, `host`).
- 400 on activate:
  - verify `password` and `confirmPassword` match and strength rules.
- 401 on login:
  - invalid credentials or account not activated.
- 500 in forgot/reset:
  - validate SMTP settings, DB connectivity, and `AuthLinks:PublicWebBaseUrl`.

## Logs and traceability
- Use API logs plus `traceId` returned by global exception middleware.
- During stabilization keep stdout logging enabled in IIS web.config.
