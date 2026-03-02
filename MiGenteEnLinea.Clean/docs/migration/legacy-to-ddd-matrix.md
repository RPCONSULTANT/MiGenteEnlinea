# Legacy to DDD/EF Matrix

Estado inicial de migracion para erradicar `ILegacyDataService`, entidades `Generated` y SQL raw en runtime.

## Convenciones
- `Legacy Source`: metodo legacy actual o SQL raw.
- `Consumer`: handler/controlador que lo usa.
- `Target`: repositorio/servicio DDD + EF tipado.
- `Status`: `Pending` | `InProgress` | `Migrated` | `Validated`.

## Matriz
| Legacy Source | Consumer | Target | Status | Notes |
|---|---|---|---|---|
| `GetRemuneracionesAsync` | `GetRemuneracionesQueryHandler` | EF tipado (`DbSet<Remuneracion>`) | Migrated | Sin `ILegacyDataService` |
| `CreateRemuneracionAsync` | `CreateRemuneracionCommandHandler` | `IRemuneracionRepository.Add` | Pending | Mantener validaciones actuales |
| `UpdateRemuneracionAsync` | `UpdateRemuneracionCommandHandler` | `IRemuneracionRepository.Update` | Pending | Concurrency check |
| `DeleteRemuneracionAsync` | `DeleteRemuneracionCommandHandler` | EF tipado (`DbSet<Remuneracion>`) | Migrated | Conserva paridad legacy (si no existe, no falla) |
| `GetDeduccionesTssAsync` | `GetDeduccionesTssQueryHandler` | EF tipado (`DbSet<DeduccionTss>`) | Migrated | Catalogo solo lectura |
| `DarDeBajaEmpleadoAsync` | `DarDeBajaEmpleadoCommandHandler` | EF tipado sobre `DbSet<Empleado>` | Migrated | Mantiene validaciones de existencia/activo |
| `CancelarTrabajoAsync` | `CancelarTrabajoCommandHandler` | EF tipado sobre `DbSet<DetalleContratacion>` | Migrated | Preserva semántica legacy (`estatus = 3`) |
| `EliminarReciboContratacion*Async` | `EliminarRecibo*` handlers | EF tipado (`ExecuteDelete` + transacción) | Migrated | Header + detalle en transacción |
| `GetReciboContratacionAsync` | recibo query handlers | Query EF sobre `EmpleadorRecibosHeaderContrataciones` | Pending | Include tipado |
| `EliminarEmpleadoTemporalAsync` | `EliminarEmpleadoTemporalCommandHandler` | EF tipado (`ExecuteDelete` + transacción) | Migrated | Elimina recibos, detalle y temporal |
| `GetPagosContratacionesAsync` | pagos/nomina queries | ReadModel EF (`VistaPagoContratacion`) | Pending | Sin SQL raw |
| `CreateEmpleadoTemporalAsync` | `CreateEmpleadoTemporalCommandHandler` | EF tipado (`DbSet<EmpleadoTemporal>` + `DbSet<DetalleContratacion>`) | Migrated | Sin SQL raw |
| `CreateDetalleContratacionAsync` | `CreateDetalleContratacionCommandHandler` | EF tipado (`DbSet<DetalleContratacion>`) | Migrated | |
| `UpdateDetalleContratacionAsync` | `UpdateDetalleContratacionCommandHandler` | EF tipado + `ExecuteUpdate` | Migrated | |
| `Calificar*Async` | `CalificarContratacionCommandHandler` | EF tipado (`DbSet<DetalleContratacion>`) | Migrated | marca `calificado` + `calificacionId` |
| `GetFichaTemporalesAsync` | `GetFichaTemporalesQueryHandler` | Query EF tipada | Migrated | |
| `GetTodosLosTemporalesAsync` | `GetTodosLosTemporalesQueryHandler` | Query EF tipada | Migrated | |
| `GetVistaContratacionTemporalAsync` | `GetVistaContratacionTemporalQueryHandler` | `VistaContratacionTemporal` read model | Migrated | |
| `GetReciboHeaderByPagoIdAsync` | `GetReciboHeaderByPagoIdQueryHandler` | Query EF tipada sobre `ReciboHeader/Detalle/Empleado` | Migrated | |
| SQL raw en `ContactosController` | `POST /api/contactos/solicitudes` | EF entity `ContactoSolicitud` | Migrated | Migrado a entidad/configuracion EF |
| SQL raw en `GetHistorialNominaByUserId` | nomina query | LINQ + proyeccion DTO | Pending | Prioridad alta |
| SQL raw en `GetHistorialNominaUnificado` | nomina unificada query | LINQ + union tipado | Pending | Prioridad alta |

## Siguientes pasos inmediatos
1. Crear repositorios/servicios DDD para temporales y recibos.
2. Reescribir queries de nomina sin SQL raw.
3. Eliminar inyecciones de `ILegacyDataService` en handlers migrados.
4. Activar guardas CI para bloquear nuevas referencias `Generated` y SQL raw en runtime.
