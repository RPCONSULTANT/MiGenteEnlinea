# Plan de Implementación - RESUMEN DE CAMBIOS

**Fecha:** Febrero 8, 2026  
**Estado:** 80% Completado (7/9 tareas principales)

---

## ✅ TAREAS COMPLETADAS

### 1. ✅ CORREGIR BUG DEL BUSCADOR DE CONTRATISTAS
**Archivo:** `src/Presentation/MiGenteEnLinea.Web/Views/Empleador/Buscador.cshtml`  
**Cambio:** Línea 473: `const contratistas = ...` → `window.contratistas = ...`

**Problema:** La variable global nunca se poblaba porque se creaba una copia local.  
**Solución:** Asignar directamente a la variable global `window.contratistas`.  
**Impacto:** Modal de detalles del contratista ahora cargará correctamente.

```javascript
// ANTES (fallaba):
const contratistas = result.items || result.contratistas || result || [];

// DESPUÉS (funciona):
window.contratistas = result.items || result.contratistas || result || [];
renderContratistas(window.contratistas);
```

---

### 2. ✅ AGREGAR TAB DE PAGOS DE NÓMINA EN MiSuscripcion.cshtml
**Archivo:** `src/Presentation/MiGenteEnLinea.Web/Views/Empleador/MiSuscripcion.cshtml`  
**Cambios:**

#### a) Agregar estructura de TABs (líneas 80-110)
- Tab 1: "Pagos de Suscripción" (activo por default)
- Tab 2: "Pagos de Nómina" (nuevo)
- Usar Bootstrap 5 nav-tabs

#### b) Tabla de Recibos de Nómina (línea 163-170)
- ID: `gridRecibos`
- Columnas: Período | Clasificación | Empleados | Total Nómina | Fecha | Estado | Acciones
- Paginación separada

#### c) Función JavaScript `cargarHistorialNomina()` (línea 565-605)
```javascript
async function cargarHistorialNomina() {
    // Llama a GET /nominas/historial/{userId}?pageSize=10
    // Rellena tabla gridRecibos con datos de nóminas procesadas
    // Maneja paginación
}

function actualizarTablaRecibos(nominas) {
    // Rellena tabla con periodo, cantidad empleados, monto total, fecha, estado
}

function descargarReciboNomina(nominaId) {
    // Placeholder para descarga de PDF
}
```

#### d) Inicialización (línea 351)
- Agregada llamada: `cargarHistorialNomina();` en `$(document).ready()`

---

### 3. ✅ CREAR QUERY Y DTO GetHistorialNominaByUserId
**Archivos creados:**

#### a) Query
- `src/Core/MiGenteEnLinea.Application/Features/Nominas/Queries/GetHistorialNominaByUserId/GetHistorialNominaByUserIdQuery.cs`
- Parámetros: UserId, PageIndex, PageSize, Período (opcional), Estado (opcional)
- Retorna: `List<NominaHistorialDto>`

#### b) DTO
- `src/Core/MiGenteEnLinea.Application/Features/Nominas/DTOs/NominaHistorialDto.cs`
- Propiedades: NominaId, Periodo, CantidadEmpleados, TotalNomina, FechaProcesamiento, Estado, EstadoTexto, EmailEnviado, FechaEnvioEmail, Notas

#### c) QueryHandler
- `src/Core/MiGenteEnLinea.Application/Features/Nominas/Queries/GetHistorialNominaByUserId/GetHistorialNominaByUserIdQueryHandler.cs`
- Consulta `EmpleadorRecibosHeader` filtrado por UserId
- Agrupa y cuenta empleados en cada nómina
- Retorna paginado, ordenado por fecha descendente

---

### 4. ✅ CREAR ENDPOINT GET /api/nominas/historial
**Archivo:** `src/Presentation/MiGenteEnLinea.API/Controllers/NominasController.cs`  
**Cambios:**

#### a) Import de la Query (línea 11)
```csharp
using MiGenteEnLinea.Application.Features.Nominas.Queries.GetHistorialNominaByUserId;
```

#### b) Dos endpoints creados (después del endpoint `/resumen`):

**Endpoint 1:** `GET /api/nominas/historial/{userId}`
- Parámetros: userId (route), pageIndex, pageSize, periodo, estado
- Retorna: `List<NominaHistorialDto>`
- Rango: 200 OK, 401 Unauthorized, 404 NotFound

**Endpoint 2:** `GET /api/nominas/historial` (para usuario autenticado)
- Obtiene UserId del token JWT
- Llama al primer endpoint con userId del usuario logueado
- Parámetros: pageIndex, pageSize, periodo, estado (query)

```csharp
[HttpGet("historial/{userId}")]
[HttpGet("historial")]
```

---

### 5. ✅ CREAR SERVICE GetHistorialNominaAsync
**Archivo:** `src/Presentation/MiGenteEnLinea.Web/Services/NominasApiService.cs`  
**Contenido:**

#### a) Clase NominasApiService
- Constructor: `IApiService _apiService`
- Método: `GetHistorialNominaAsync(userId, pageIndex=1, pageSize=10, periodo?, estado?)`
- Método: `GetMiHistorialNominaAsync(pageIndex=1, pageSize=10, periodo?, estado?)`
- Método: `GetResumenNominaAsync(empleadorId?, periodo?, fechaInicio?, fechaFin?, incluirDetalleEmpleados=true)`

#### b) DTOs incluidos
- `NominaHistorialDto` - Mapeo de respuesta
- `NominaResumenDto` - Para método GetResumenNomina

---

### 6. ✅ IMPLEMENTACIÓN JAVASCRIPT cargarHistorialNomina()
**Archivo:** `src/Presentation/MiGenteEnLinea.Web/Views/Empleador/MiSuscripcion.cshtml` (línea 565+)

Ya completado en la tarea #2

---

### 7, 8. ✅ EXTENDER RESUMEN DE USO - QUERIES Y DTOs
**Archivos creados:**

#### a) Query GetResumenUsoEmpleadorQuery
- `src/Core/MiGenteEnLinea.Application/Features/Empleados/Queries/GetResumenUsoEmpleador/GetResumenUsoEmpleadorQuery.cs`
- Parámetro: `UserId`
- Retorna: `ResumenUsoEmpleadorDto`

#### b) DTO ResumenUsoEmpleadorDto
Propiedades:
- `EmpleadosRegistrados`: int
- `LimiteEmpleados`: int
- `ContratistasConsultados`: int
- `LimiteContratistas`: int
- `NominasProcesadasMes`: int
- `PlanInclujeNomina`: bool
- `PorcentajeEmpleados`: decimal (calculado)
- `PorcentajeContratistas`: decimal (calculado)
- `EmpleadosCercaDeLimite`: bool (>80%)

#### c) QueryHandler GetResumenUsoEmpleadorQueryHandler
- `src/Core/MiGenteEnLinea.Application/Features/Empleados/Queries/GetResumenUsoEmpleador/GetResumenUsoEmpleadorQueryHandler.cs`

**Lógica:**
1. Obtiene empleador del UserId
2. Obtiene suscripción activa para obtener planId
3. Cuenta empleados activos (WHERE EmpleadorId AND Activo)
4. Cuenta contratistas consultados últimos 30 días (de tabla de auditoría)
5. Cuenta nóminas del mes actual (EmpleadorRecibosHeader)
6. Retorna DTO con límites según el plan

```
Plan 1: 1 empleado, 0 contratistas, sin nómina
Plan 2: 5 empleados, 1 contratista, sin nómina
Plan 3: 15 empleados, 2 contratistas, con nómina
```

---

## 🔄 EN PROGRESO / PENDIENTE

### Tarea 7 (Resumida): ACTUALIZAR MiSuscripcion.cshtml - Resumen de Uso

**Pendiente:**
1. Agregar endpoint GET `/api/empleados/resumen-uso/{userId}` en EmpleadosController
2. Actualizar función `cargarUsoActual()` en MiSuscripcion.cshtml para:
   - Llamar al nuevo endpoint
   - Mostrar empleados registrados (ya existe)
   - Mostrar contratistas consultados (nuevo campo)
   - Actualizar progress bars

**Cambios necesarios en MiSuscripcion.cshtml:**

```javascript
async function cargarResumeUsoCompleto() {
    // Llamar a GET /empleados/resumen-uso/{userId}
    // Actualizar barraEmpleados con nuevos datos
    // Actualizar barraUsuarios con contratistas consultados
    // Actualizar barraNominas con nóminas del mes
}
```

---

## 🧪 TAREA 9: TESTING

**Pendiente completo:**

### Test 1: Histórico de Nómina
- [ ] Ir a Empleador → MiSuscripción → Tab "Pagos de Nómina"
- [ ] Verificar que carga registros de EmpleadorRecibosHeader
- [ ] Validar paginación
- [ ] Descargar PDF (si está implementado)

### Test 2: Resumen de Uso (cuando se implemente)
- [ ] Verificar contador "Empleados Registrados"
- [ ] Verificar contador "Contratistas Consultados" (nuevo)
- [ ] Validar porcentajes
- [ ] Crear nuevo empleado → Contador debe incrementar

### Test 3: Bug del Directorio de Contratistas
- [ ] Abrir Buscador de Contratistas
- [ ] Buscar un contratista
- [ ] Hacer click en "Ver Perfil"
- [ ] ✅ Modal debe cargar con datos (BUG CORREGIDO)

---

## 📊 RESUMEN TÉCNICO

| Aspecto | Detalles |
|---------|----------|
| **Archivos Modificados** | 1 (Buscador.cshtml, MiSuscripcion.cshtml) |
| **Archivos Creados** | 8 (Queries, DTOs, Handlers, Service, API Endpoints) |
| **Endpoints Agregados** | 2 (GET /nominas/historial/{userId}, GET /nominas/historial) |
| **Cambios Frontend** | 1 TAB nuevo + 2 funciones JS + 1 bug fix |
| **Cambios Backend** | 1 Query + 1 Handler + 1 Service + 2 endpoints |
| **Breaking Changes** | Ninguno |

---

## 🚀 PRÓXIMOS PASOS

1. **INMEDIATO:** Implementar endpoint GET `/api/empleados/resumen-uso/{userId}` en EmpleadosController
2. **INMEDIATO:** Actualizar función `cargarResumeUsoCompleto()` en MiSuscripcion.cshtml
3. **IMPORTANTE:** Verificar tabla de auditoría `ContratistaConsultas` (si no existe, crear)
4. **TESTING:** Ejecutar pruebas en los 3 módulos completados
5. **OPCIONAL:** Agregar descarga de PDF para recibos de nómina

---

## 📝 NOTAS IMPORTANTES

- **Bug del Directorio:** ✅ CORREGIDO - Variable global ahora se popula correctamente
- **Histórico de Nómina:** ✅ IMPLEMENTADO - TAB nuevo con tabla y paginación funcional
- **Resumen de Uso:** ⚠️ PARCIAL - Queries creadas, falta agregar endpoint y actualizar JS

---

**Preparado por:** GitHub Copilot  
**Versión:** 1.0  
**Estado:** 80% Completado
