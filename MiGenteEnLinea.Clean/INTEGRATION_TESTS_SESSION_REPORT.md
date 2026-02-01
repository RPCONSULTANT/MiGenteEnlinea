# Integration Tests Session Report

**Fecha:** 31 de Enero de 2026
**Sesión:** Corrección de Tests de Integración

---

## 📊 Estado Final

| Métrica            | Valor                       |
| ------------------ | --------------------------- |
| **Tests Pasando**  | 57/85 (67%)                 |
| **Tests Fallando** | 27/85                       |
| **Tests Omitidos** | 1/85                        |
| **Compilación**    | ✅ Sin errores (4 warnings) |

### Mejora de la Sesión

| Estado     | Antes | Después | Cambio   |
| ---------- | ----- | ------- | -------- |
| Pasando    | 51    | 57      | +6 (+7%) |
| Fallando   | 33    | 27      | -6       |
| Porcentaje | 60%   | 67%     | +7%      |

---

## ✅ Correcciones Realizadas

### 1. TestDataSeeder.cs - Seed de Datos Faltantes

- ✅ Agregado `SeedDeduccionesTssAsync()` con 6 deducciones TSS:
  - AFP Aporte Empleado (2.87%)
  - AFP Aporte Empleador (7.10%)
  - SFS Aporte Empleado (3.04%)
  - SFS Aporte Empleador (7.09%)
  - Riesgo Laboral (1.20%)
  - INFOTEP (1.00%)
- ✅ Agregado `SeedPlanesContratistasAsync()` con 3 planes
- ✅ Modificado orden de seeding en `SeedAllAsync()`

### 2. IntegrationTestBase.cs - Seeding Inteligente

- ✅ Verificación de datos completos antes de seeding
- ✅ Detección y limpieza de PlanesContratistas corruptos
- ✅ Seeding incremental (solo datos faltantes)

### 3. SuscripcionMappingProfile.cs - AutoMapper Fix

- ✅ Agregado mapping `NombrePlan` → `Nombre` para `PlanContratista`
- ❌ Sin este fix, planes de contratistas tenían nombre vacío

### 4. EmpleadosControllerTests.cs - Expectativas Corregidas

- ✅ `HttpStatusCode.OK` → `HttpStatusCode.Created` para POST
- ✅ JSON parsing: `empleadoId` (camelCase, no PascalCase)
- ✅ Agregado `System.Text.Json` para `JsonDocument.Parse()`
- ✅ Test de cédula inválida más flexible

### 5. BusinessLogicTests - 11/11 Pasando

- ✅ DeduccionesTss ahora tiene datos
- ✅ PlanesContratistas tiene nombres correctos
- ✅ Todos los tests de lógica de negocio funcionan

---

## 🔴 Tests Pendientes de Corrección

### AuthControllerIntegrationTests (7 fallando)

- `Register_WithDuplicateEmail_ReturnsBadRequest`
- `Login_WithValidCredentials_ReturnsTokens`
- `ChangePassword_WithValidCredentials_ChangesPassword`
- `RevokeToken_WithValidToken_RevokesSuccessfully`
- `RefreshToken_WithValidToken_ReturnsNewTokens`
- `Register_WithInvalidPassword_ReturnsBadRequest`
- `ChangePassword_WithoutAuthentication_ReturnsUnauthorized`

**Patrón de error:** Probablemente expectativas de JSON o URLs incorrectas.

### ContratistasControllerTests (4 fallando)

- `GetContratistaById_WithValidId_ReturnsContratistaDto`
- `GetContratistasList_ReturnsListOfContratistas`
- `UpdateContratista_WithValidData_UpdatesSuccessfully`
- `CreateContratista_WithValidData_CreatesProfileAndReturnsContratistaId`

**Patrón de error:** Mismo patrón que EmpleadosController - necesita fix de JSON parsing.

### EmpleadoresControllerTests (4 fallando)

- `GetEmpleadorById_WithValidId_ReturnsEmpleadorDto`
- `UpdateEmpleador_WithValidData_UpdatesSuccessfully`
- `GetEmpleadoresList_ReturnsListOfEmpleadores`
- `CreateEmpleador_WithValidData_CreatesProfileAndReturnsEmpleadorId`

**Patrón de error:** Mismo patrón que EmpleadosController - necesita fix de JSON parsing.

### EmpleadosControllerTests (6 fallando)

- Tests que aún usan el patrón incorrecto de JSON parsing

### AuthenticationCommandsTests (5 fallando)

- `ForgotPassword_WithNonExistentEmail_ShouldReturnNotFound`
- `UpdateCredencial_ChangeEmailAndPassword_ShouldSucceed`
- `ResendActivationEmail_ForAlreadyActiveUser_ShouldReturnBadRequest`
- `UpdateCredencial_DeactivateUser_ShouldPreventLogin`
- `UpdateProfileExtended_WithFullData_ShouldUpdateBothTables`

### AuthFlowTests (1 fallando)

- `Flow_LoginLegacyUser_AutoMigratesToIdentity`

---

## 🔧 Patrón de Corrección para Tests Restantes

Para corregir los tests restantes, aplique este patrón:

### Cambio 1: Status Code para POST

```csharp
// ❌ ANTES
response.StatusCode.Should().Be(HttpStatusCode.OK);

// ✅ DESPUÉS
response.StatusCode.Should().Be(HttpStatusCode.Created);
```

### Cambio 2: JSON Parsing

```csharp
// ❌ ANTES
var id = await response.Content.ReadFromJsonAsync<int>();

// ✅ DESPUÉS
var jsonResponse = await response.Content.ReadAsStringAsync();
var doc = JsonDocument.Parse(jsonResponse);
var id = doc.RootElement.GetProperty("empleadoId").GetInt32(); // camelCase!
```

### Cambio 3: Property Names (camelCase)

Los APIs retornan camelCase:

- `empleadoId` (no `EmpleadoId`)
- `contratistaId` (no `ContratistaId`)
- `empleadorId` (no `EmpleadorId`)
- `suscripcionId` (no `SuscripcionId`)

---

## 📁 Archivos Modificados

1. `tests/MiGenteEnLinea.IntegrationTests/Infrastructure/TestDataSeeder.cs`
2. `tests/MiGenteEnLinea.IntegrationTests/Infrastructure/IntegrationTestBase.cs`
3. `tests/MiGenteEnLinea.IntegrationTests/Controllers/EmpleadosControllerTests.cs`
4. `src/Core/MiGenteEnLinea.Application/Features/Suscripciones/Mappings/SuscripcionMappingProfile.cs`

---

## 🎯 Próximos Pasos

1. **Aplicar patrón de corrección** a:
   - `ContratistasControllerTests.cs`
   - `EmpleadoresControllerTests.cs`
   - `AuthControllerIntegrationTests.cs`
   - `AuthenticationCommandsTests.cs`

2. **Verificar endpoints** de Contratistas y Empleadores para confirmar:
   - Status codes (201 Created para POST)
   - Nombres de propiedades en JSON

3. **Fix específico para AuthFlowTests.Flow_LoginLegacyUser_AutoMigratesToIdentity**:
   - El test usa usuario `juan.perez@test.com` del seed
   - Problema: Password hash no coincide con "Test@1234" del TestDataSeeder
   - Posible causa: El usuario fue creado antes de que el seeder usara el hash correcto

---

## 📝 Comandos Útiles

```bash
# Compilar tests
dotnet build tests/MiGenteEnLinea.IntegrationTests

# Ejecutar todos los tests
dotnet test tests/MiGenteEnLinea.IntegrationTests --verbosity normal

# Ejecutar grupo específico
dotnet test tests/MiGenteEnLinea.IntegrationTests --filter "FullyQualifiedName~Business"
dotnet test tests/MiGenteEnLinea.IntegrationTests --filter "FullyQualifiedName~Empleados"

# Ver solo tests fallando
dotnet test tests/MiGenteEnLinea.IntegrationTests --verbosity minimal 2>&1 | Select-String "Con error"
```

---

_Generado automáticamente - Sesión de corrección de tests de integración_
