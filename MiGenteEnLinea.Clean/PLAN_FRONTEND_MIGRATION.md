# 🎨 PLAN DE MIGRACIÓN FRONTEND - MiGenteEnLinea

> **Fecha:** Enero 31, 2026  
> **Objetivo:** Migrar 100% del frontend Legacy a ASP.NET Core MVC  
> **Estrategia:** Primero TODO lo visual, luego conectar con API

---

## 📋 RESUMEN EJECUTIVO

### Fuentes de Verdad
| Carpeta | Propósito |
|---------|-----------|
| `FRONT_Publicado/` | Frontend PRODUCCIÓN actual - **TODO lo visual viene de aquí** |
| `Codigo Fuente Mi Gente/` | Backend Legacy + Code-behind - **Lógica de referencia** |
| `MiGenteEnLinea.API/` | Backend nuevo - **123 endpoints REST listos** |

### Estrategia de Migración
1. **FASE 0:** Eliminar proyecto Web actual y recrear desde cero
2. **FASE 1-4:** Migrar assets y layouts (CSS, JS, fonts, imágenes)
3. **FASE 5-8:** Migrar páginas por módulo (Landing, Auth, Empleador, Contratista)
4. **FASE 9:** Conectar con API Backend

---

## 📊 INVENTARIO COMPLETO DE PÁGINAS A MIGRAR

### 🏠 Landing/Public Pages (6 páginas)
| Página Legacy | Ruta Legacy | Nueva Ruta MVC | Layout |
|---------------|-------------|----------------|--------|
| `Index.aspx` | `/` | `/` | `_LayoutLanding` |
| `Landing/Login.aspx` | `/Landing/Login.aspx` | `/Auth/Login` | `_LayoutAuth` |
| `Landing/Registrar.aspx` | `/Landing/Registrar.aspx` | `/Auth/Register` | `_LayoutAuth` |
| `Landing/activarperfil.aspx` | `/Landing/activarperfil.aspx` | `/Auth/Activate` | `_LayoutAuth` |
| `Landing/Planes.aspx` | `/Landing/Planes.aspx` | `/Planes` | `_LayoutLanding` |
| `paypalGateway.aspx` | `/paypalGateway.aspx` | `/Payment/Gateway` | Ninguno |

### 👔 Empleador Module (15 páginas)
| Página Legacy | Ruta Legacy | Nueva Ruta MVC | Layout |
|---------------|-------------|----------------|--------|
| `Empleador/index_empleador.aspx` | `/Empleador/index_empleador.aspx` | `/Empleador` | `_LayoutEmpleador` |
| `Empleador/empleados.aspx` | `/Empleador/empleados.aspx` | `/Empleador/Empleados` | `_LayoutEmpleador` |
| `Empleador/fichaEmpleado.aspx` | `/Empleador/fichaEmpleado.aspx` | `/Empleador/Empleados/{id}` | `_LayoutEmpleador` |
| `Empleador/Nomina.aspx` | `/Empleador/Nomina.aspx` | `/Empleador/Nomina` | `_LayoutEmpleador` |
| `Empleador/ContratacionesTemporales.aspx` | `/Empleador/ContratacionesTemporales.aspx` | `/Empleador/Contrataciones` | `_LayoutEmpleador` |
| `Empleador/fichaColaboradorTemporal.aspx` | `/Empleador/fichaColaboradorTemporal.aspx` | `/Empleador/Contrataciones/{id}` | `_LayoutEmpleador` |
| `Empleador/detalleContratacion.aspx` | `/Empleador/detalleContratacion.aspx` | `/Empleador/Contrataciones/Detalle/{id}` | `_LayoutEmpleador` |
| `Empleador/CalificacionDePerfiles.aspx` | `/Empleador/CalificacionDePerfiles.aspx` | `/Empleador/Calificaciones` | `_LayoutEmpleador` |
| `Empleador/perfilProfesional.aspx` | `/Empleador/perfilProfesional.aspx` | `/Empleador/Contratistas/{id}` | `_LayoutEmpleador` |
| `Empleador/miPerfilEmpleador.aspx` | `/Empleador/miPerfilEmpleador.aspx` | `/Empleador/Perfil` | `_LayoutEmpleador` |
| `Empleador/AdquirirPlanEmpleador.aspx` | `/Empleador/AdquirirPlanEmpleador.aspx` | `/Empleador/Plan` | `_LayoutEmpleador` |
| `Empleador/Checkout.aspx` | `/Empleador/Checkout.aspx` | `/Empleador/Checkout` | `_LayoutEmpleador` |
| `Empleador/Suscripciones_Empleador.aspx` | `/Empleador/Suscripciones_Empleador.aspx` | `/Empleador/Suscripcion` | `_LayoutEmpleador` |
| `Empleador/FAQ.aspx` | `/Empleador/FAQ.aspx` | `/Empleador/FAQ` | `_LayoutEmpleador` |
| `Empleador/dashboardPage.aspx` | `/Empleador/dashboardPage.aspx` | `/Empleador/Dashboard` | `_LayoutDashboard` |

### 🔧 Contratista Module (4 páginas)
| Página Legacy | Ruta Legacy | Nueva Ruta MVC | Layout |
|---------------|-------------|----------------|--------|
| `Contratista/index_contratista.aspx` | `/Contratista/index_contratista.aspx` | `/Contratista` | `_LayoutContratista` |
| `Contratista/Suscripciones_Contratistas.aspx` | `/Contratista/Suscripciones_Contratistas.aspx` | `/Contratista/Suscripcion` | `_LayoutContratista` |
| `Contratista/Checkout_Contratista.aspx` | `/Contratista/Checkout_Contratista.aspx` | `/Contratista/Checkout` | `_LayoutContratista` |
| (MisCalificaciones - en código fuente) | `/Contratista/MisCalificaciones.aspx` | `/Contratista/Calificaciones` | `_LayoutContratista` |

### 🖨️ Impresión/PDF Templates (7 archivos HTML)
| Archivo | Propósito |
|---------|-----------|
| `Impresion/ContratoPersonaFisica.html` | Contrato para persona física |
| `Impresion/ContratoEmpresa.html` | Contrato para empresa |
| `Impresion/ReciboPagoPersonaFisica_Empleador1.html` | Recibo de pago persona física |
| `Impresion/ReciboPagoEmpresa_Empleador1.html` | Recibo de pago empresa |
| `Impresion/ReciboDescargoPersonaFisica_Empleador1.html` | Recibo de descargo persona física |
| `Impresion/ReciboDescargoEmpresa_Empleador1.html` | Recibo de descargo empresa |
| `Impresion/PrintViewer.aspx` | Visor de impresión |

### 📧 Email Templates (3 archivos HTML)
| Archivo | Propósito |
|---------|-----------|
| `MailTemplates/checkout.html` | Confirmación de pago |
| `MailTemplates/confirmacionRegistro.html` | Confirmación de registro |
| `MailTemplates/recuperarPass.html` | Recuperar contraseña |

### 📜 Legal Templates (3 archivos HTML)
| Archivo | Propósito |
|---------|-----------|
| `Template/AutorizacionEmpleadores.html` | Autorización empleadores |
| `Template/AutorizacionProveedores.html` | Autorización proveedores |
| `Template/TerminosMiGente.html` | Términos y condiciones |

---

## 📁 INVENTARIO DE ASSETS

### CSS (Copiar a wwwroot/css/)
```
FRONT_Publicado/Styles/
├── Custom.css              → wwwroot/css/custom.css
└── animated.css            → wwwroot/css/animated.css
```

### JavaScript (Copiar a wwwroot/js/)
```
FRONT_Publicado/Scripts/
├── Custom.js               → wwwroot/js/custom.js
└── paypal.js               → wwwroot/js/paypal.js
```

### Fonts (Copiar a wwwroot/fonts/)
```
FRONT_Publicado/Fonts/
├── Gurajada-Regular.ttf    → wwwroot/fonts/Gurajada-Regular.ttf
└── Barlow-Black.ttf        → wwwroot/fonts/Barlow-Black.ttf
```

### Imágenes (Copiar a wwwroot/images/)
```
FRONT_Publicado/Images/
├── logoMiGene.png          ⭐ Logo principal
├── logoMiGenteBlanco.png   ⭐ Logo blanco
├── back1.jpg               ⭐ Fondo login/landing
├── banner_Foto1.jpg        ⭐ Banner empleador
├── bannerADM1.jpg          Banner admin
├── banner2.png             Banner 2
├── banner3.png             Banner 3
├── MainBanner.jpg          Banner principal
├── Cardnet-Web.png         Logo Cardnet
├── calculatorIcon.png      Icono calculadora
├── legalDocIcon.png        Icono documentos legales
├── moneyIcon.png           Icono dinero
├── whatsapp.png            Icono WhatsApp
├── workers.png             Ilustración trabajadores
├── circular1.png           Elemento circular
├── image47.png             Imagen paso 1
├── angryimg.png            Imagen error
├── x2q8uahp.bmp            Imagen misc
└── Contratista/            Carpeta contratistas
```

### Argon Dashboard Theme (Copiar a wwwroot/lib/argon/)
```
FRONT_Publicado/Template/assets/
├── css/
│   ├── argon-dashboard.css      ⭐ Theme principal
│   ├── argon-dashboard.min.css
│   ├── bootstrap.css
│   ├── bootstrap.min.css
│   ├── nucleo-icons.css         ⭐ Iconos
│   └── nucleo-svg.css
├── fonts/                       ⭐ Fuentes del theme
├── img/                         ⭐ Imágenes del theme
├── js/                          ⭐ JavaScript del theme
└── scss/                        (opcional, para desarrollo)
```

---

## 🏗️ LAYOUTS (MASTER PAGES → RAZOR LAYOUTS)

### Layout Mapping
| Master Page Legacy | Razor Layout | Uso |
|-------------------|--------------|-----|
| `Landing/landing.Master` | `_LayoutLanding.cshtml` | Páginas públicas |
| `Empleador/comunity.Master` | `_LayoutEmpleador.cshtml` | Dashboard empleador |
| `Empleador/dashboard.Master` | `_LayoutDashboard.cshtml` | Dashboard alternativo |
| `Empleador/FAQ_Master.Master` | `_LayoutFAQ.cshtml` | Páginas FAQ |
| `Contratista/ContratistasM.Master` | `_LayoutContratista.cshtml` | Dashboard contratista |
| `Platform/platform.Master` | `_LayoutPlatform.cshtml` | Páginas plataforma |
| (nuevo) | `_LayoutAuth.cshtml` | Login/Register/Activate |

---

## 🚀 FASES DE IMPLEMENTACIÓN

### FASE 0: Reset del Proyecto Web (30 min)
**Objetivo:** Eliminar contenido actual y recrear estructura limpia

```powershell
# Acciones:
1. Eliminar contenido de Views/, Controllers/, wwwroot/, Services/, Models/
2. Mantener archivos de configuración (Program.cs, appsettings.json, .csproj)
3. Crear estructura de carpetas nueva
4. Commit: "Reset: Clean slate for frontend migration"
```

**Estructura Nueva:**
```
MiGenteEnLinea.Web/
├── Controllers/
│   ├── HomeController.cs
│   ├── AuthController.cs
│   ├── EmpleadorController.cs
│   └── ContratistaController.cs
├── Models/
│   └── ViewModels/
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml
│   │   ├── _LayoutLanding.cshtml
│   │   ├── _LayoutAuth.cshtml
│   │   ├── _LayoutEmpleador.cshtml
│   │   ├── _LayoutContratista.cshtml
│   │   ├── _ViewImports.cshtml
│   │   └── _ViewStart.cshtml
│   ├── Home/
│   ├── Auth/
│   ├── Empleador/
│   └── Contratista/
├── wwwroot/
│   ├── css/
│   ├── js/
│   ├── fonts/
│   ├── images/
│   ├── lib/
│   └── templates/
├── Services/
│   └── ApiService.cs
├── Program.cs
├── appsettings.json
└── MiGenteEnLinea.Web.csproj
```

---

### FASE 1: Assets Estáticos (1 hora)
**Objetivo:** Copiar todos los assets del Legacy

**Bloque 1.1: CSS**
```powershell
# Copiar CSS personalizados
Copy-Item "FRONT_Publicado/Styles/*" → "wwwroot/css/"
# Copiar Argon Dashboard CSS
Copy-Item "FRONT_Publicado/Template/assets/css/*" → "wwwroot/lib/argon/css/"
```

**Bloque 1.2: JavaScript**
```powershell
# Copiar JS personalizados
Copy-Item "FRONT_Publicado/Scripts/*" → "wwwroot/js/"
# Copiar Argon Dashboard JS
Copy-Item "FRONT_Publicado/Template/assets/js/*" → "wwwroot/lib/argon/js/"
```

**Bloque 1.3: Fonts**
```powershell
# Copiar fuentes personalizadas
Copy-Item "FRONT_Publicado/Fonts/*" → "wwwroot/fonts/"
# Copiar fuentes Argon
Copy-Item "FRONT_Publicado/Template/assets/fonts/*" → "wwwroot/lib/argon/fonts/"
```

**Bloque 1.4: Imágenes**
```powershell
# Copiar imágenes
Copy-Item "FRONT_Publicado/Images/*" → "wwwroot/images/" -Recurse
# Copiar imágenes Argon
Copy-Item "FRONT_Publicado/Template/assets/img/*" → "wwwroot/lib/argon/img/" -Recurse
```

**Commit:** `feat(frontend): Add all static assets from legacy`

---

### FASE 2: Templates HTML (30 min)
**Objetivo:** Copiar templates de email, impresión y legales

**Bloque 2.1: Email Templates**
```powershell
Copy-Item "FRONT_Publicado/MailTemplates/*" → "wwwroot/templates/email/"
```

**Bloque 2.2: Print Templates**
```powershell
Copy-Item "FRONT_Publicado/Empleador/Impresion/*.html" → "wwwroot/templates/print/"
```

**Bloque 2.3: Legal Templates**
```powershell
Copy-Item "FRONT_Publicado/Template/*.html" → "wwwroot/templates/legal/"
```

**Commit:** `feat(frontend): Add email, print, and legal templates`

---

### FASE 3: Layouts Base (2 horas)
**Objetivo:** Convertir Master Pages a Razor Layouts

**Bloque 3.1: _Layout.cshtml (Base)**
- Layout base que todos heredan
- Referencias CDN comunes (Bootstrap 4, FontAwesome, SweetAlert2)
- Scripts globales

**Bloque 3.2: _LayoutLanding.cshtml**
- Convertir `Landing/landing.Master`
- Navbar público con logo
- Footer público
- Sin sidebar

**Bloque 3.3: _LayoutAuth.cshtml**
- Layout para Login/Register/Activate
- Fondo con gradiente + imagen (back1.jpg)
- Sin navbar, solo logo centrado

**Bloque 3.4: _LayoutEmpleador.cshtml**
- Convertir `Empleador/comunity.Master`
- Navbar con menú de empleador
- Sidebar con opciones de empleador
- Footer

**Bloque 3.5: _LayoutContratista.cshtml**
- Convertir `Contratista/ContratistasM.Master`
- Navbar con menú de contratista
- Sidebar con opciones de contratista
- Footer

**Commit:** `feat(frontend): Add Razor layouts converted from Master Pages`

---

### FASE 4: Páginas Landing/Public (2 horas)
**Objetivo:** Migrar páginas públicas

**Bloque 4.1: Home/Index**
- Página principal (Index.aspx)
- Banner principal
- Secciones informativas
- Call-to-action

**Bloque 4.2: Planes**
- Página de planes (Landing/Planes.aspx)
- Cards de precios
- Comparación de planes

**Commit:** `feat(frontend): Add public landing pages`

---

### FASE 5: Páginas de Autenticación (2 horas)
**Objetivo:** Migrar Login, Register, Activate

**Bloque 5.1: Auth/Login**
- Convertir `Landing/Login.aspx`
- Formulario de login
- Link "Olvidaste tu contraseña"
- Animaciones (animate.css)

**Bloque 5.2: Auth/Register**
- Convertir `Landing/Registrar.aspx`
- Formulario de registro
- Selección tipo usuario (Empleador/Contratista)
- Validaciones client-side

**Bloque 5.3: Auth/Activate**
- Convertir `Landing/activarperfil.aspx`
- Mensaje de activación
- Redirección automática

**Bloque 5.4: Auth/ForgotPassword + ResetPassword**
- Formulario de recuperación
- Formulario de reset

**Commit:** `feat(frontend): Add authentication pages`

---

### FASE 6: Dashboard Empleador (4 horas)
**Objetivo:** Migrar todas las páginas de empleador

**Bloque 6.1: Empleador/Index (Dashboard)**
- Convertir `Empleador/index_empleador.aspx`
- Cards de resumen
- Gráficas (si aplica)
- Quick actions

**Bloque 6.2: Empleador/Empleados**
- Lista de empleados
- Tabla con DevExpress → DataTable Bootstrap
- Filtros y búsqueda

**Bloque 6.3: Empleador/FichaEmpleado**
- Detalle de empleado
- Tabs (datos personales, remuneraciones, deducciones)
- Formulario de edición

**Bloque 6.4: Empleador/Nomina**
- Procesamiento de nómina
- Tabla de empleados con cálculos
- Acciones de pago

**Bloque 6.5: Empleador/Contrataciones**
- Lista de contrataciones temporales
- Fichas de colaboradores temporales

**Bloque 6.6: Empleador/Calificaciones**
- Búsqueda de contratistas
- Cards de perfiles
- Sistema de estrellas

**Bloque 6.7: Empleador/Perfil**
- Perfil del empleador
- Edición de datos
- Cambio de contraseña

**Bloque 6.8: Empleador/Suscripcion + Checkout**
- Estado de suscripción
- Compra de plan
- Integración Cardnet

**Bloque 6.9: Empleador/FAQ**
- Preguntas frecuentes
- Acordeones expandibles

**Commit:** `feat(frontend): Add employer dashboard pages`

---

### FASE 7: Dashboard Contratista (2 horas)
**Objetivo:** Migrar páginas de contratista

**Bloque 7.1: Contratista/Index (Dashboard)**
- Convertir `Contratista/index_contratista.aspx`
- Resumen de perfil
- Calificaciones recibidas

**Bloque 7.2: Contratista/Calificaciones**
- Mis calificaciones
- Historial

**Bloque 7.3: Contratista/Suscripcion + Checkout**
- Estado de suscripción
- Compra de plan

**Commit:** `feat(frontend): Add contractor dashboard pages`

---

### FASE 8: Componentes Parciales (2 horas)
**Objetivo:** Crear componentes reutilizables

**Bloque 8.1: Partial Views**
- `_Navbar.cshtml` (navbar compartido)
- `_Sidebar.cshtml` (sidebar empleador)
- `_SidebarContratista.cshtml` (sidebar contratista)
- `_Footer.cshtml` (footer compartido)
- `_StarRating.cshtml` (estrellas de calificación)
- `_PlanCard.cshtml` (card de plan)
- `_EmployeeCard.cshtml` (card de empleado)
- `_ContractorCard.cshtml` (card de contratista)

**Bloque 8.2: View Components**
- `NotificationComponent` (notificaciones)
- `UserProfileComponent` (mini perfil en navbar)

**Commit:** `feat(frontend): Add partial views and components`

---

### FASE 9: Conectar con API (4 horas)
**Objetivo:** Integrar con MiGenteEnLinea.API

**Bloque 9.1: ApiService**
- HttpClient configurado
- Métodos para cada endpoint
- Manejo de tokens JWT
- Refresh automático

**Bloque 9.2: Autenticación**
- Login → POST /api/auth/login
- Register → POST /api/auth/register
- Activate → POST /api/auth/activate
- Logout → POST /api/auth/logout

**Bloque 9.3: Empleador Endpoints**
- GET/POST/PUT empleados
- GET/POST recibos
- GET/POST contrataciones
- GET planes, suscripciones

**Bloque 9.4: Contratista Endpoints**
- GET perfil
- GET calificaciones
- GET suscripción

**Commit:** `feat(frontend): Integrate with REST API`

---

## 📝 PROMPT PARA AGENTE - FASE 0: RESET

```markdown
# TAREA: Reset del Proyecto MiGenteEnLinea.Web

## CONTEXTO
Estamos migrando el frontend Legacy de MiGenteEnLinea a ASP.NET Core MVC.
El proyecto `MiGenteEnLinea.Web` ya existe pero tiene contenido que debe eliminarse.

## OBJETIVO
Eliminar TODO el contenido actual y crear una estructura limpia para la migración.

## ACCIONES REQUERIDAS

### 1. Eliminar Contenido Actual
Eliminar TODO el contenido de estas carpetas (mantener las carpetas vacías):
- `src/Presentation/MiGenteEnLinea.Web/Controllers/` (eliminar todos los .cs)
- `src/Presentation/MiGenteEnLinea.Web/Models/` (eliminar todo)
- `src/Presentation/MiGenteEnLinea.Web/Views/` (eliminar todo)
- `src/Presentation/MiGenteEnLinea.Web/wwwroot/` (eliminar todo)
- `src/Presentation/MiGenteEnLinea.Web/Services/` (eliminar todo)

### 2. NO Eliminar
- `Program.cs` (mantener)
- `appsettings.json` y `appsettings.Development.json` (mantener)
- `MiGenteEnLinea.Web.csproj` (mantener)
- `Properties/` (mantener)

### 3. Crear Estructura Nueva
Crear las siguientes carpetas vacías:
```
Controllers/
Models/
  ViewModels/
Views/
  Shared/
  Home/
  Auth/
  Empleador/
  Contratista/
wwwroot/
  css/
  js/
  fonts/
  images/
  lib/
    argon/
      css/
      js/
      fonts/
      img/
  templates/
    email/
    print/
    legal/
Services/
```

### 4. Crear Archivos Base
Crear estos archivos mínimos:

**Views/_ViewImports.cshtml:**
```cshtml
@using MiGenteEnLinea.Web
@using MiGenteEnLinea.Web.Models
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

**Views/_ViewStart.cshtml:**
```cshtml
@{
    Layout = "_Layout";
}
```

**Views/Shared/_Layout.cshtml:**
```cshtml
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Mi Gente en Línea</title>
    @RenderSection("Styles", required: false)
</head>
<body>
    @RenderBody()
    @RenderSection("Scripts", required: false)
</body>
</html>
```

**Controllers/HomeController.cs:**
```csharp
using Microsoft.AspNetCore.Mvc;

namespace MiGenteEnLinea.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
```

**Views/Home/Index.cshtml:**
```cshtml
@{
    ViewData["Title"] = "Inicio";
}

<h1>MiGenteEnLinea - Frontend en Construcción</h1>
<p>Fase 0 completada. Listo para migración.</p>
```

### 5. Verificar
- Ejecutar `dotnet build` para verificar que compila
- Ejecutar `dotnet run` para verificar que inicia

### 6. Commit
Mensaje: `chore(frontend): Reset Web project for migration - clean slate`

## RESULTADO ESPERADO
Un proyecto Web limpio con estructura preparada para recibir los assets y páginas del Legacy.
```

---

## 📝 PROMPT PARA AGENTE - FASE 1: ASSETS

```markdown
# TAREA: Migrar Assets Estáticos a MiGenteEnLinea.Web

## CONTEXTO
Proyecto: MiGenteEnLinea.Clean/src/Presentation/MiGenteEnLinea.Web
Fuente: FRONT_Publicado/ (frontend Legacy en producción)

## OBJETIVO
Copiar TODOS los assets estáticos del Legacy al proyecto Web nuevo.

## ACCIONES REQUERIDAS

### 1. CSS
Copiar archivos CSS:
- `FRONT_Publicado/Styles/Custom.css` → `wwwroot/css/custom.css`
- `FRONT_Publicado/Styles/animated.css` → `wwwroot/css/animated.css`
- `FRONT_Publicado/Template/assets/css/argon-dashboard.css` → `wwwroot/lib/argon/css/argon-dashboard.css`
- `FRONT_Publicado/Template/assets/css/argon-dashboard.min.css` → `wwwroot/lib/argon/css/argon-dashboard.min.css`
- `FRONT_Publicado/Template/assets/css/bootstrap.css` → `wwwroot/lib/argon/css/bootstrap.css`
- `FRONT_Publicado/Template/assets/css/bootstrap.min.css` → `wwwroot/lib/argon/css/bootstrap.min.css`
- `FRONT_Publicado/Template/assets/css/nucleo-icons.css` → `wwwroot/lib/argon/css/nucleo-icons.css`
- `FRONT_Publicado/Template/assets/css/nucleo-svg.css` → `wwwroot/lib/argon/css/nucleo-svg.css`

### 2. JavaScript
Copiar archivos JS:
- `FRONT_Publicado/Scripts/Custom.js` → `wwwroot/js/custom.js`
- `FRONT_Publicado/Scripts/paypal.js` → `wwwroot/js/paypal.js`
- Toda la carpeta `FRONT_Publicado/Template/assets/js/` → `wwwroot/lib/argon/js/`

### 3. Fonts
Copiar fuentes:
- `FRONT_Publicado/Fonts/Gurajada-Regular.ttf` → `wwwroot/fonts/Gurajada-Regular.ttf`
- `FRONT_Publicado/Fonts/Barlow-Black.ttf` → `wwwroot/fonts/Barlow-Black.ttf`
- Toda la carpeta `FRONT_Publicado/Template/assets/fonts/` → `wwwroot/lib/argon/fonts/`

### 4. Imágenes
Copiar imágenes:
- Toda la carpeta `FRONT_Publicado/Images/` → `wwwroot/images/` (incluyendo subcarpetas)
- Toda la carpeta `FRONT_Publicado/Template/assets/img/` → `wwwroot/lib/argon/img/`

### 5. Templates
Copiar templates HTML:
- `FRONT_Publicado/MailTemplates/checkout.html` → `wwwroot/templates/email/checkout.html`
- `FRONT_Publicado/MailTemplates/confirmacionRegistro.html` → `wwwroot/templates/email/confirmacionRegistro.html`
- `FRONT_Publicado/MailTemplates/recuperarPass.html` → `wwwroot/templates/email/recuperarPass.html`
- `FRONT_Publicado/Empleador/Impresion/ContratoPersonaFisica.html` → `wwwroot/templates/print/ContratoPersonaFisica.html`
- `FRONT_Publicado/Empleador/Impresion/ContratoEmpresa.html` → `wwwroot/templates/print/ContratoEmpresa.html`
- `FRONT_Publicado/Empleador/Impresion/ReciboPagoPersonaFisica_Empleador1.html` → `wwwroot/templates/print/ReciboPagoPersonaFisica.html`
- `FRONT_Publicado/Empleador/Impresion/ReciboPagoEmpresa_Empleador1.html` → `wwwroot/templates/print/ReciboPagoEmpresa.html`
- `FRONT_Publicado/Empleador/Impresion/ReciboDescargoPersonaFisica_Empleador1.html` → `wwwroot/templates/print/ReciboDescargoPersonaFisica.html`
- `FRONT_Publicado/Empleador/Impresion/ReciboDescargoEmpresa_Empleador1.html` → `wwwroot/templates/print/ReciboDescargoEmpresa.html`
- `FRONT_Publicado/Template/AutorizacionEmpleadores.html` → `wwwroot/templates/legal/AutorizacionEmpleadores.html`
- `FRONT_Publicado/Template/AutorizacionProveedores.html` → `wwwroot/templates/legal/AutorizacionProveedores.html`
- `FRONT_Publicado/Template/TerminosMiGente.html` → `wwwroot/templates/legal/TerminosMiGente.html`

### 6. Verificar
- Verificar que todos los archivos se copiaron correctamente
- Ejecutar `dotnet build` para verificar que compila

### 7. Commit
Mensaje: `feat(frontend): Add all static assets from legacy production`

## NOTAS IMPORTANTES
- Los archivos CSS custom.css y animated.css contienen estilos críticos
- Las fuentes Gurajada y Barlow son usadas en los headers
- Las imágenes logoMiGene.png y back1.jpg son críticas para el branding
```

---

## 📊 ESTIMACIÓN DE TIEMPO TOTAL

| Fase | Descripción | Tiempo Estimado |
|------|-------------|-----------------|
| 0 | Reset del proyecto | 30 min |
| 1 | Assets estáticos | 1 hora |
| 2 | Templates HTML | 30 min |
| 3 | Layouts Razor | 2 horas |
| 4 | Páginas Landing | 2 horas |
| 5 | Páginas Auth | 2 horas |
| 6 | Dashboard Empleador | 4 horas |
| 7 | Dashboard Contratista | 2 horas |
| 8 | Componentes parciales | 2 horas |
| 9 | Conectar con API | 4 horas |
| **TOTAL** | **Migración completa** | **~20 horas** |

---

## ✅ CHECKLIST FINAL

### Pre-requisitos
- [ ] Backend API funcionando (123 endpoints)
- [ ] Base de datos con datos de prueba
- [ ] Proyecto Web reseteado (Fase 0)

### Migración Visual
- [ ] Assets copiados (CSS, JS, fonts, images)
- [ ] Templates copiados (email, print, legal)
- [ ] Layouts convertidos (5 layouts)
- [ ] Páginas Landing migradas (6 páginas)
- [ ] Páginas Auth migradas (4 páginas)
- [ ] Páginas Empleador migradas (15 páginas)
- [ ] Páginas Contratista migradas (4 páginas)
- [ ] Componentes creados (partials + view components)

### Integración
- [ ] ApiService configurado
- [ ] Autenticación JWT funcionando
- [ ] Todos los endpoints conectados
- [ ] Flujos completos probados

### Verificación
- [ ] Visual 100% idéntico al Legacy
- [ ] Rutas funcionando
- [ ] Responsive funcionando
- [ ] Sin errores de consola
