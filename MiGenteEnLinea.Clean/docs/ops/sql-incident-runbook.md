# SQL Incident Runbook (18456 + EF Execution Strategy)

## Scope
Runbook para incidentes donde la API presenta `500` por:
- autenticación SQL fallida (`SqlException 18456`)
- transacciones manuales incompatibles con `SqlServerRetryingExecutionStrategy`.

## Symptom Pattern
- `GET /api/contratistas` o `GET /api/contrataciones` retorna `500`.
- Logs con `Error Number:18456` y `State:<n>`.
- Logs con `The configured execution strategy 'SqlServerRetryingExecutionStrategy' does not support user-initiated transactions`.

## Immediate Triage
1. Confirmar build activo en servidor (`BUILD_COMMIT`/`GITHUB_SHA`).
2. Revisar variables efectivas:
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `ConnectionStrings__DefaultConnection`
3. Probar salud:
   - `GET /health`
   - `GET /health/db`
4. Si `/health/db` falla, ir a diagnóstico SQL.

## SQL 18456 Diagnostic
1. Consultar SQL Error Log y capturar `State` exacto.
2. Aplicar corrección según state:
   - login/password inválido: recrear/activar login.
   - DB por defecto inválida: `ALTER LOGIN ... WITH DEFAULT_DATABASE = <db>`.
   - permisos: mapear usuario en DB y asignar roles mínimos requeridos.
   - connection string a host/DB incorrectos: corregir `DefaultConnection`.
3. Reiniciar solo el App Pool/API después de corregir.

## EF Retry + Transaction Guardrail
1. Toda transacción manual en runtime debe ir dentro de:
   - `db.Database.CreateExecutionStrategy().ExecuteAsync(async () => { ... BeginTransactionAsync ... })`
2. Verificación automática:
   - `.\scripts\validate-ef-transaction-retry.ps1`
3. Si falla, corregir handlers antes de despliegue.

## Pre-deploy Checklist
1. `.\scripts\env\Validate-RequiredEnv.ps1 -Component API`
2. `.\scripts\validate-ef-transaction-retry.ps1`
3. `.\scripts\env\Invoke-PreDeployChecks.ps1 -EnvironmentName Production -ApiHealthUrl <url> -ApiCorsProbeUrl <url> -WebUrl <url>`

## Post-fix Validation
1. `GET /health` -> `200`.
2. `GET /health/db` -> `200`.
3. Endpoints críticos sin `500`:
   - `GET /api/contratistas`
   - `GET /api/contrataciones`
   - `POST /api/empleados/temporales`

## Microsoft Learn MCP (Documentation Lookup)
Usar el servidor MCP para consulta rápida de documentación oficial durante el incidente:
- Learn MCP overview: https://learn.microsoft.com/en-us/training/support/mcp
- Learn MCP developer reference: https://learn.microsoft.com/en-us/training/support/mcp-developer-reference
- EF Core connection resiliency: https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
- SQL Server error 18456: https://learn.microsoft.com/en-us/sql/relational-databases/errors-events/mssqlserver-18456-database-engine-error?view=sql-server-ver16

Nota: `https://learn.microsoft.com/api/mcp` no se prueba desde navegador como endpoint de negocio de la aplicación.
