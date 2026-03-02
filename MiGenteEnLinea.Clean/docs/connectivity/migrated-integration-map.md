# Migrated Integration Map

Matriz de conectividad real para lo migrado a Clean/DDD (API -> Application -> Frontend).

## Estados
- `Connected`: endpoint, handler y UI conectados.
- `Partial`: endpoint/handler existe, UI depende de hardcode/fallback o contrato incompleto.
- `Broken`: hay desalineación funcional.

## Flujos críticos
| Use case | Controller endpoint | Handler/query | Frontend consumer | Status | Notes |
|---|---|---|---|---|---|
| Crear temporal (paso 1) | `POST /api/empleados/temporales` | `CreateEmpleadoTemporalCommandHandler` | `Views/Empleador/Contrataciones.cshtml` | Connected | Usa `EMPLEADOS.TEMPORALES_CREATE`; `UserId` viene de JWT en backend. |
| Crear contratación (paso 2) | `POST /api/contrataciones` | `CreateContratacionCommandHandler` | `Views/Empleador/Contrataciones.cshtml` | Connected | Payload normalizado vía mapper en JS. |
| Ver detalle de contratación | `GET /api/contrataciones/{id}` | `GetContratacionByIdQuery` | `Views/Empleador/Contrataciones.cshtml`, `FichaColaboradorTemporal.cshtml` | Connected | Endpoint catalogado. |
| Aceptar/Iniciar/Completar/Cancelar | `PUT /api/contrataciones/{id}/accept|start|complete|cancel` | handlers de estado en `Features/Contrataciones` | `Views/Empleador/Contrataciones.cshtml` | Connected | Opera con user autenticado. |
| Contrato temporal PDF | `GET /api/contrataciones/{detalleId}/contrato-pdf` | `ContratacionesController.GetContratoTemporalPdf` | `Contrataciones.cshtml`, `FichaColaboradorTemporal.cshtml` | Connected | Endpoint centralizado `CONTRATACIONES.CONTRATO_PDF`. |
| Ficha temporal | `GET /api/empleados/temporales/ficha` | `GetFichaTemporalesQueryHandler` | `Views/Empleador/FichaColaboradorTemporal.cshtml` | Connected | `userId` query ya validado/normalizado por JWT en controller. |
| Vista temporal completa | `GET /api/empleados/temporales/vista` | `GetVistaContratacionTemporalQueryHandler` | APIs internas / futura UI | Connected | `userId` query validado contra JWT. |
| Pagos contratación | `GET /api/empleados/pagos-contrataciones` | `GetPagosContratacionesQueryHandler` | `Views/Empleador/FichaColaboradorTemporal.cshtml` | Connected | Endpoint catalogado `EMPLEADOS.PAGOS_CONTRATACIONES`. |
| Nómina historial unificado | `GET /api/nominas/historial-unificado` | `GetHistorialNominaUnificadoQueryHandler` | `Views/Empleador/Nomina.cshtml` | Connected | Query migrada a EF/LINQ. |
| Solicitud de contacto | `POST /api/contactos/solicitudes` | `ContactosController` (EF typed) | `Views/Contratista/Directorio.cshtml` | Connected | Endpoint catalogado `CONTACTOS.SOLICITUDES`. |
| Baja empleado fijo | `PUT /api/empleados/{id}/dar-de-baja` | `DarDeBajaEmpleadoCommandHandler` | `Views/Empleador/Empleados.cshtml`, `FichaEmpleado.cshtml` | Connected | Handler ya no usa legacy service. |
| Eliminar recibos | `DELETE /api/empleados/recibos-empleado/{id}/eliminar`, `...recibos-contratacion...` | handlers `EliminarRecibo*` | `Views/Empleador/Nomina.cshtml` (vía APIs) | Partial | Falta contrato de error unificado en todos los llamados UI. |

## Checklist de cierre de conectividad
- No usar rutas hardcodeadas para endpoints migrados que ya están en `wwwroot/js/api-endpoints.js`.
- Todos los errores de endpoints migrados deben incluir `code`, `message`, `correlationId`.
- Todos los flujos críticos tienen al menos 1 prueba API + 1 E2E.
