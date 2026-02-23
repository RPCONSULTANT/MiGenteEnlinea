# Frontend API Alignment Matrix

Date: 2026-02-23

## Scope
- `src/Presentation/MiGenteEnLinea.Web`
- `src/Presentation/MiGenteEnLinea.API/Controllers`

## Decision Rules
- All frontend endpoints must come from `wwwroot/js/api-endpoints.js`.
- All parsing must use `window.readApiResponse(...)` (no direct `response.json()` assignments).
- All URL construction must use `window.buildApiUrl(...)` indirectly through `authenticatedFetch` / `requestApi`.

## Module Coverage

| Module | Frontend Area | API Contract | Status |
|---|---|---|---|
| Auth | `Views/Auth/*` | `/api/auth/login|register|activate|forgot-password` | OK |
| Planes públicos | `Views/Home/Planes.cshtml` | `/api/suscripciones/planes/*` | OK |
| Pagos | `Views/*/AdquirirPlan.cshtml`, `Views/*/Checkout.cshtml` | `POST /api/pagos/procesar` | OK |
| Suscripciones | `Views/Contratista/Suscripciones.cshtml`, `Views/Empleador/MiSuscripcion.cshtml` | `/api/suscripciones/activa/{userId}`, `/api/suscripciones/ventas/{userId}` | OK |
| Empleados | `Views/Empleador/Empleados.cshtml`, `Views/Empleador/FichaEmpleado.cshtml`, `Views/Empleador/Nomina.cshtml` | `/api/empleados/*`, `/api/nominas/*` | OK (query `soloActivos` aplicado) |
| Contratista Perfil | `Views/Contratista/Index.cshtml` | `/api/contratistas/by-user/{userId}`, `/api/contratistas/{id}/servicios`, `/api/contratistas/{userId}/foto` | OK |
| Contrataciones | `Views/Empleador/Contrataciones.cshtml` | `/api/contrataciones`, `/api/contrataciones/{id}/start|complete|cancel` | OK |
| Calificaciones | `Views/Empleador/Calificaciones.cshtml`, `Views/Contratista/MisCalificaciones.cshtml` | `/api/calificaciones/*` | OK |
| Dashboard | `Views/Empleador/Index.cshtml`, `Views/Contratista/Index.cshtml` | `/api/dashboard/*` + métricas auxiliares | OK |
| Catálogos/Utilitarios | `wwwroot/js/Custom.js` + views dependientes | `/api/catalogos/*`, `/api/utilitarios/numero-a-letras` | OK |

## High-Risk Mismatches Corrected
- Legacy routes removed:
  - `/suscripciones/usuario/{userId}`
  - `/suscripciones/ventas/usuario/{userId}`
  - `/empleados/consultar-padron/{cedula}`
  - `/empleadores/perfil/{userId}`
- Contratista photo contract split:
  - GET photo => `contratistaId` path
  - POST upload => `userId` path
- Empleados query normalized:
  - `activos=true` -> `soloActivos=true`

## Validation Gate
- `scripts/validate-frontend-api-contract.ps1`
  - Fails on legacy paths/hardcodes.
  - Fails on direct `response.json()` assignments.
  - Verifies endpoint catalog contains all required domains.
- `verify-deployment.ps1`
  - Verifies health, swagger, CORS preflight, and critical endpoints.
