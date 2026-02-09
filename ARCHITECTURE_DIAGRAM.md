# 🏗️ Architecture: Image + Ratings Integration

## 📊 Data Flow Architecture

```
┌────────────────────────────────────────────────────────────────────────────┐
│                     EMPLOYER DASHBOARD                                     │
├────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│   CONTRATACIONES VIEW                          CALIFICACIONES VIEW         │
│   ┌──────────────────────────────────┐         ┌──────────────────────┐   │
│   │ [Tab] Completadas                │         │ Dropdown:           │   │
│   │                                  │         │ [Select Contractor] │   │
│   │ ┌────────────────────────────────┐         │                     │   │
│   │ │ [Avatar] | Job | Rating         │    ╔═══╫═> [Photo Display]  │   │
│   │ │ Photo url│ Desc| Calificado     │    ║   │ [Star Rating Form] │   │
│   │ │ Name     │ Amt | [Buttons]      │    ║   │ [Submit Button]    │   │
│   │ │          │     │                │    ║   └──────────────────────┘   │
│   │ └────────────────────────────────┘    ║                              │
│   │          │                            ║                              │
│   │ onClick [Calificar★]                  ║                              │
│   └──────────┼────────────────────────────┘                              │
│              │                                                             │
│              └─→ Navigate with ID                                         │
│                                                                             │
└────────────────────────────────────────────────────────────────────────────┘
                                    ↓
```

## 🔌 API Integration Layer

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         REST API  (Port 5015)                              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  GET /api/contrataciones?soloNoCalificadas=true                            │
│  ├─ Purpose: Load completed contracts without ratings                     │
│  ├─ Query Filter: Estatus=4 (COMPLETADA) AND Calificado=false            │
│  ├─ Handler: GetContratacionesQueryHandler                               │
│  ├─ Returns: List<ContratacionDto> with NEW fields:                      │
│  │   ├─ contratistaIdentificacion (added ✨)                            │
│  │   ├─ contratistaCompleteName (added ✨)                              │
│  │   └─ contratistaFotoUrl (added ✨)                                   │
│  └─ Response:                                                              │
│     {                                                                       │
│       detalleId: 45,                                                       │
│       descripcionCorta: \"Reparación plomería\",                          │
│       contratistaIdentificacion: \"00123456789\",  ✨                     │
│       contratistaCompleteName: \"Juan García García\",  ✨                │
│       contratistaFotoUrl: \"https://cdn.../photo.jpg\"  ✨               │
│     }                                                                       │
│                                                                             │
│  POST /api/calificaciones/calificar-perfil                                │
│  ├─ Purpose: Submit rating for completed contractor work                 │
│  ├─ Handler: CalificarPerfilCommandHandler                              │
│  ├─ Request Body:                                                          │
│  │   {                                                                      │
│  │     empleadorUserId: \"user-123\",                                      │
│  │     contratistaIdentificacion: \"00123456789\",                        │
│  │     puntualidad: 5,                                                     │
│  │     cumplimiento: 4,                                                    │
│  │     conocimientos: 5,                                                   │
│  │     recomendacion: 4                                                    │
│  │   }                                                                      │
│  └─ Response: { id: 789, success: true }                                  │
│                                                                             │
│  GET /api/calificaciones/por-empleador/{userId}                           │
│  ├─ Purpose: Load historical ratings for employer                        │
│  ├─ Returns: List<CalificacionDto>                                        │
│  └─ Used in: History table population                                      │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
                                    ↓
```

## 💾 Database & Domain Layer

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                    MiGenteDbContext (SQL Server)                           │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  DetalleContrataciones Table                                               │
│  ├─ PK: detalleID                                                          │
│  ├─ FK: contratacionID → EmpleadosTemporale                              │
│  ├─ Data:                                                                   │
│  │   ├─ descripcionCorta                                                   │
│  │   ├─ fechaInicio, fechaFinal                                            │
│  │   ├─ montoAcordado                                                      │
│  │   ├─ estatus (1=Pendiente...4=Completada)                             │
│  │   ├─ calificado (bool)                                                  │
│  │   └─ calificacionID (FK)                                                │
│  └─ [GetContratacionesQueryHandler ENRICHes this with: ↓]                │
│                                                                             │
│  EmpleadosTemporale Table (Legacy Bridge)                                  │
│  ├─ PK: contratacionID (= DetalleContrataciones.contratacionID)          │
│  ├─ Data:                                                                   │
│  │   ├─ identificacion (Cedula/RNC)                                       │
│  │   ├─ nombre, apellido                                                   │
│  │   └─ foto (URL string) ✨ SOURCE OF IMAGE                             │
│  └─ [Mapped to Domain EmpleadoTemporal entity]                           │
│                                                                             │
│  Calificaciones Table                                                       │
│  ├─ PK: calificacionID                                                     │
│  ├─ FK: empleadorID, contratistaIdentificacion                            │
│  ├─ Data:                                                                   │
│  │   ├─ puntualidad (1-5)                                                  │
│  │   ├─ cumplimiento (1-5)                                                 │
│  │   ├─ conocimientos (1-5)                                                │
│  │   ├─ recomendacion (1-5)                                                │
│  │   └─ fechaCalificacion                                                  │
│  └─ [Persisted by CreateCalificacionCommandHandler]                      │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
                                    ↑
```

## 🔄 Handler & Query Execution

```
┌─────────────────────────────────────────────────────────────────────────────┐
│         GetContratacionesQueryHandler (Application Layer)                  │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  1. FILTER Phase                                                            │
│     var query = _context.DetalleContrataciones.AsQueryable()              │
│     Apply filters: SoloNoCalificadas=true → Estatus=4 & Calificado=false │
│     Result: IQueryable[DetalleContratacion]                              │
│                                                                             │
│  2. EXECUTE Phase                                                           │
│     var contrataciones = await query.ToListAsync()                       │
│     Result: List[DetalleContratacion] (domain entities)                  │
│                                                                             │
│  3. MAP Phase  (Standard AutoMapper)                                        │
│     var dtos = _mapper.Map<List<ContratacionDto>>(contrataciones)       │
│     Result: List[ContratacionDto] (basic fields only)                    │
│                                                                             │
│  4. ENRICH Phase  ✨ NEW - Fills in contractor data                       │
│     if (dtos.Any())                                                        │
│     {                                                                       │
│       var empleadoTemporalIds = dtos                                      │
│           .Where(d => d.ContratacionId.HasValue)                         │
│           .Select(d => d.ContratacionId.Value)                           │
│           .Distinct()                                                      │
│           .ToList();                                                       │
│                                                                             │
│       var empleadosTemporales = await                                     │
│           _context.Set<EmpleadoTemporal>()                              │
│           .Where(et => empleadoTemporalIds.Contains(et.ContratacionId)) │
│           .ToListAsync();                                                 │
│                                                                             │
│       foreach (var dto in dtos.Where(d => d.ContratacionId.HasValue))  │
│       {                                                                    │
│           if (empleadoTemporalDict.TryGetValue(...))                    │
│           {                                                                │
│               dto.ContratistaIdentificacion = et.Identificacion;        │
│               dto.ContratistaCompleteName = et.ObtenerNombreCompleto(); │
│               dto.ContratistaFotoUrl = et.Foto;  ← PHOTO!             │
│           }                                                                │
│       }                                                                    │
│     }                                                                       │
│                                                                             │
│  5. RETURN Phase                                                            │
│     return dtos;  ← Now fully enriched with contractor data              │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## 🎨 Frontend Rendering

```
┌─────────────────────────────────────────────────────────────────────────────┐
│              Contrataciones.cshtml (renderCompletadas)                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  Data Received from API:                                                   │
│  [                                                                          │
│    {                                                                        │
│      detalleId: 45,                                                        │
│      descripcionCorta: \"Reparación plomería\",                           │
│      contratistaIdentificacion: \"00123456789\",   ✨                    │
│      contratistaCompleteName: \"Juan García López\", ✨                   │
│      contratistaFotoUrl: \"https://cdn.../photo.jpg\"  ✨               │
│    }                                                                        │
│  ]                                                                          │
│                                                                             │
│  Rendering Logic (inside table.map()):                                    │
│  └─ <tr>                                                                   │
│     ├─ <td> #45 </td>                                                      │
│     │                                                                       │
│     ├─ <td>                         ✨ NEW COLUMN                          │
│     │     <div class=\"d-flex\">                                          │
│     │         <img src=\"${c.contratistaFotoUrl}\"                       │
│     │              onerror=\"this.src='/images/circular1.png'\"/>        │
│     │         <div>${c.contratistaCompleteName}</div>                   │
│     │     </div>                                                          │
│     │ </td>                                                                │
│     │                                                                       │
│     ├─ <td> 2026-02-01 - 2026-02-15 </td>                                │
│     ├─ <td> RD$ 5,000 </td>                                               │
│     ├─ <td> Pending [Calificar★] </td>                                   │
│     └─ </tr>                                                               │
│                                                                             │
│  Fallback Strategy:                                                        │
│  - Image URL valid → Display photo (40x40, rounded)                      │
│  - Image URL broken → onerror event → /images/circular1.png             │
│  - No URL → Use fallback directly                                         │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## 🎭 Modal & Rating Flow

```
┌─────────────────────────────────────────────────────────────────────────────┐
│              Calificaciones.cshtml (Modal Flow)                            │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  1. PAGE LOAD                                                              │
│     $(document).ready()                                                    │
│     → cargarPerfilesCalificables()  ✨ NEW IMPLEMENTATION                │
│       └─ GET /api/contrataciones?soloNoCalificadas=true                 │
│          └─ Response → perfilesCalificables = [...]                     │
│          └─ llenarDropdownPerfiles()                                    │
│                                                                             │
│  2. DROPDOWN POPULATION                                                    │
│     perfilesCalificables.forEach(p => {                                   │
│       ddlPerfil.append(<option>                                          │
│         value=\"${p.id}\"                                                 │
│         data-identificacion=\"${p.identificacion}\"                      │
│         data-nombre=\"${p.nombre}\"                                       │
│         data-foto=\"${p.fotoUrl}\"                                        │
│       >                                                                     │
│     })                                                                      │
│                                                                             │
│  3. CONTRACTOR SELECTION                                                   │
│     User clicks dropdown option                                            │
│     → onPerfilSeleccionado()  ✨ PHOTO DISPLAY ADDED                     │
│       ├─ $('#calif_identificacion').val(identificacion)                 │
│       ├─ $('#calif_nombre').val(nombre)                                 │
│       ├─ $('#fotoContratista').attr('src', fotoUrl)  ✨ NEW            │
│       │   .on('error', () => this.src='/images/circular1.png')          │
│       └─ verificarFormulario()                                           │
│                                                                             │
│  4. RATING ENTRY                                                           │
│     User clicks 5 stars under each category                              │
│     → jQuery star rating system (already implemented)                    │
│     ├─ $('#valorPuntualidad').val(5)                                    │
│     ├─ $('#valorCumplimiento').val(4)                                   │
│     ├─ $('#valorConocimientos').val(5)                                  │
│     └─ $('#valorRecomendacion').val(4)                                  │
│                                                                             │
│  5. SUBMISSION                                                             │
│     User clicks [Enviar Calificación]                                    │
│     → calificarPerfil()  ✨ FULL API INTEGRATION                        │
│       └─ $.ajax POST /api/calificaciones/calificar-perfil               │
│          ├─ Request: {                                                    │
│          │   empleadorUserId: \"123\",  ← from JWT                      │
│          │   contratistaIdentificacion: \"00123456789\",                │
│          │   puntualidad: 5,                                            │
│          │   cumplimiento: 4,                                           │
│          │   conocimientos: 5,                                          │
│          │   recomendacion: 4                                           │
│          │ }                                                              │
│          └─ Response: { success: true, id: 789 }                        │
│                                                                             │
│  6. SUCCESS HANDLING                                                       │
│     onSuccess()                                                            │
│     ├─ $('#modalCalificar').modal('hide')                               │
│     ├─ Swal.fire('¡Éxito!', '...', 'success')                          │
│     ├─ limpiarFormularioCalificacion()                                  │
│     ├─ cargarPerfilesCalificables()  ← Refresh dropdown                │
│     └─ cargarMisCalificaciones()  ✨ NEW - Load history                │
│                                                                             │
│  7. HISTORY DISPLAY  ✨ NEW                                              │
│     cargarMisCalificaciones()                                            │
│     └─ GET /api/calificaciones/por-empleador/123                       │
│        └─ Response → renderizarTablaCalificaciones(data)               │
│           └─ Display table with 5-star ratings & dates                 │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## 🔐 Security & Validation

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                   Security Layer                                           │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  JWT Token Extraction (CLIENT-SIDE)                                        │
│  ├─ localStorage.getItem('token') OR sessionStorage.getItem('token')     │
│  └─ getUserIdFromToken() ✨ NEW HELPER                                   │
│     ├─ Decode JWT: base64(split[1])                                      │
│     ├─ Extract: payload['nameid'] || payload['sub']                      │
│     └─ Try-catch for safe error handling                                  │
│                                                                             │
│  AJAX Authorization Headers (ALL CALLS)                                    │
│  └─ headers: { 'Authorization': 'Bearer ' + token }                      │
│     └─ Validates user identity on server-side                            │
│                                                                             │
│  Server-Side Validation (CalificarPerfilCommandHandler)                   │
│  ├─ Verify empleadorUserId matches authenticated user                   │
│  ├─ Check no duplicate rating (empleadorId + contratistaId)             │
│  ├─ Validate rating values (1-5 range)                                   │
│  └─ Enforce DetalleContratacion belongs to employer                     │
│                                                                             │
│  Null Safety (FRONTEND)                                                    │
│  ├─ Image URL fallback: ${url || '/images/circular1.png'}              │
│  ├─ Name fallback: ${name || 'Contratista'}                            │
│  └─ onerror handler on <img> tags                                        │
│                                                                             │
│  SQL Injection Prevention (ORM-ONLY)                                       │
│  └─ All queries use Entity Framework LINQ - NO string concatenation    │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## 📈 Performance Optimization

```
┌─────────────────────────────────────────────────────────────────────────────┐
│            Performance Considerations                                      │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  Database Queries                                                          │
│  ├─ GetContratacionesQueryHandler filters BEFORE ToListAsync()          │
│  │   └─ Estatus=4 + Calificado=false pruned in SQL                     │
│  │                                                                        │
│  ├─ EmpleadoTemporal JOIN only for non-null ContratacionIds             │
│  │   └─ Dictionary lookup O(1) for enrichment                            │
│  │                                                                        │
│  └─ Indexes on database:                                                │
│     ├─ IX_DetalleContrataciones_Estatus_Calificado ✓                   │
│     └─ IX_DetalleContrataciones_ContratacionId ✓                       │
│                                                                             │
│  Frontend Rendering                                                        │
│  ├─ Images lazy-loaded (HTML <img> default behavior)                    │
│  └─ Star rating system uses event delegation (no repeated handlers)     │
│                                                                             │
│  API Calls                                                                 │
│  ├─ GET /api/contrataciones filtered: ~100 items max (pageSize)        │
│  ├─ GET /api/calificaciones/por-empleador filtered: paginated         │
│  └─ POST /api/calificaciones/calificar-perfil: single record           │
│                                                                             │
│  Browser Caching                                                           │
│  └─ Images cached by browser (CDN + HTTP headers)                       │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## ✨ Summary

**Before:**
```
Contrataciones × No contractor info
Calificaciones × Empty TODO blocks
Ratings × Not functional
```

**After:**
```
Contrataciones ✓ Shows contractor photos + names
Calificaciones ✓ Fully functional API integration
Ratings ✓ End-to-end working system
```

**Key Achievement:** 
Unified data from 2 separate tables (DetalleContratacion + EmpleadoTemporal) into single enriched DTO for seamless UI rendering of contractor information + photos.
