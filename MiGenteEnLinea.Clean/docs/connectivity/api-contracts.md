# API Contracts for Migrated Connectivity

Contratos funcionales para los endpoints migrados y consumidos por el front-end actual.

## Convención de error (objetivo)
```json
{
  "code": "validation_error|business_rule_error|unauthorized|not_found|internal_error",
  "message": "Mensaje legible",
  "details": [],
  "correlationId": "trace-id"
}
```

## Endpoints críticos

### 1) Crear temporal base
- `POST /api/empleados/temporales`
- Request:
```json
{
  "tipo": 1,
  "nombre": "Juan",
  "apellido": "Perez",
  "identificacion": "00112345678",
  "servicio": "Electricidad",
  "fechaInicio": "2026-03-02",
  "fechaFinal": "2026-03-12",
  "pago": 1000
}
```
- Success: `200` body numérico (`contratacionId`) o wrapper equivalente.
- Error: contrato de error estándar.

### 2) Crear detalle de contratación
- `POST /api/contrataciones`
- Request:
```json
{
  "contratacionId": 123,
  "contratistaId": 1013,
  "servicioId": 10,
  "descripcionCorta": "Instalación eléctrica",
  "fechaInicio": "2026-03-02",
  "fechaFinal": "2026-03-12",
  "montoAcordado": 1000
}
```
- Success: `200` con `detalleId` (número o wrapper).
- Error: contrato de error estándar.

### 3) Ficha temporal
- `GET /api/empleados/temporales/ficha?contratacionId={id}&userId={userId}`
- Seguridad:
  - No admin: `userId` de query debe coincidir con JWT (o se usa JWT por defecto).
  - Admin: puede consultar con `userId` explícito.
- Success: `200` `EmpleadoTemporalDto`.
- NotFound: `404`.

### 4) Vista temporal
- `GET /api/empleados/temporales/vista?contratacionId={id}&userId={userId}`
- Seguridad igual que ficha temporal.
- Success: `200` `VistaContratacionTemporalDto`.

### 5) Contrato PDF temporal
- `GET /api/contrataciones/{detalleId}/contrato-pdf`
- Success: `200` `application/pdf`.
- Errores comunes:
  - `404` no existe detalle/temporal
  - `409` estado inválido para contrato
  - `403` ownership inválido

### 6) Solicitud de contacto
- `POST /api/contactos/solicitudes`
- Request:
```json
{
  "empleadorId": 10,
  "mensaje": "Me interesa colaborar.",
  "canalPreferido": "whatsapp"
}
```
- Success: `201`
- Error:
  - `400` validación
  - `409` solicitud pendiente duplicada

### 7) Historial nómina unificado
- `GET /api/nominas/historial-unificado?pageIndex=1&pageSize=50`
- Success: `200` lista/estructura paginada con registros fijos y temporales.

## Consumo frontend obligatorio
- Usar `window.API_ENDPOINTS` para estos endpoints:
  - `EMPLEADOS.TEMPORALES_CREATE`
  - `EMPLEADOS.TEMPORALES_FICHA`
  - `EMPLEADOS.TEMPORALES_VISTA`
  - `EMPLEADOS.PAGOS_CONTRATACIONES`
  - `CONTACTOS.SOLICITUDES`
  - `CONTRATACIONES.CONTRATO_PDF`
