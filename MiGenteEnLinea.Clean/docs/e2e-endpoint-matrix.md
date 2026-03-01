# E2E Endpoint Matrix

## Web routes
- `/`
- `/Auth/Login`
- `/Auth/Registrar`
- `/Auth/Activar`
- `/Empleador/Index`
- `/Empleador/Empleados`
- `/Empleador/Contrataciones`
- `/Empleador/Nomina`
- `/Empleador/AdquirirPlan`
- `/Empleador/Checkout`
- `/Contratista/Index`
- `/Contratista/Perfil`
- `/Contratista/Directorio`
- `/Contratista/Suscripciones`
- `/Contratista/AdquirirPlan`
- `/Contratista/Checkout`

## API contracts (covered in E2E)
- `GET /health`
- `GET /swagger/v1/swagger.json`
- `POST /api/auth/login`
- `POST /api/auth/register`
- `POST /api/auth/activate`
- `POST /api/auth/forgot-password`
- `POST /api/auth/refresh`
- `POST /api/auth/revoke`
- `POST /api/auth/delete-user`
- `GET /api/catalogos/provincias`
- `GET /api/catalogos/sectores`
- `GET /api/catalogos/servicios`
- `GET /api/utilitarios/numero-a-letras`
- `GET /api/suscripciones/planes/empleadores`
- `GET /api/suscripciones/planes/contratistas`
- `GET /api/empleadores/by-user/{userId}`
- `GET /api/contratistas/by-user/{userId}`
- `GET /api/contratistas/{contratistaId}/servicios`
- `OPTIONS /api/pagos/procesar`
- `POST /api/pagos/procesar`
- `GET /api/nominas/historial/{userId}`
- `POST /api/admin/database/repair-plans` (admin + seed key)

## Expected status policy
- Public read routes: `200`
- Auth negative scenarios: `400|401|403|404|409` controlled
- Write operations in full tests must register rollback actions.
