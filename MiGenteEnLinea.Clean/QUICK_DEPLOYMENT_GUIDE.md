# 🚀 Guía Rápida de Despliegue - MiGente En Línea VPS

**Última actualización:** Febrero 9, 2026  
**VPS:** myaspnet (Windows Server + IIS)

---

## ⚡ Pasos Rápidos para Deployar

### 1️⃣ Compilar y Publicar Binarios (LOCAL - tu máquina)

```powershell
cd "C:\Users\Ray\Documents\MiGenteEnlinea\MiGenteEnLinea.Clean"

# Ejecutar script de publicación
.\publish-production.ps1

# Esto generará:
# C:\Publish\MiGenteEnlinea\API\           - Binarios del API
# C:\Publish\MiGenteEnlinea\Web\           - Binarios del Web
# C:\Publish\MiGenteEnlinea\MiGenteEnLinea-API.zip  - ZIP para subir
# C:\Publish\MiGenteEnlinea\MiGenteEnLinea-Web.zip  - ZIP para subir
```

---

### 2️⃣ Configurar Credenciales de Producción (LOCAL - antes de subir)

**⚠️ CRÍTICO:** Edita estos archivos ANTES de subir al VPS:

#### **API - ConnectionString**
Archivo: `C:\Publish\MiGenteEnlinea\API\appsettings.Production.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SQL_SERVER;Database=db_a9f8ff_migente;User Id=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true;Encrypt=True"
  },
  "Jwt": {
    "SecretKey": "GENERAR_KEY_ALEATORIA_64_CHARS_AQUI"
  }
}
```

**Generar JWT Secret Key:**
```powershell
# Ejecuta esto para generar un secret key aleatorio
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | ForEach-Object {[char]$_})
```

#### **Web - API URL**
Archivo: `C:\Publish\MiGenteEnlinea\Web\appsettings.Production.json`

```json
{
  "ApiConfiguration": {
    "BaseUrl": "https://api.migenteenlinea.com/api",
    "StaticFilesBaseUrl": "https://api.migenteenlinea.com"
  }
}
```

---

### 3️⃣ Subir al VPS (RDP o FTP)

```
Conéctate a myaspnet vía Remote Desktop o FTP

Crea carpetas:
C:\inetpub\MiGenteEnLinea\API\
C:\inetpub\MiGenteEnLinea\Web\

Sube y descomprime:
MiGenteEnLinea-API.zip → C:\inetpub\MiGenteEnLinea\API\
MiGenteEnLinea-Web.zip → C:\inetpub\MiGenteEnLinea\Web\
```

---

### 4️⃣ Configurar IIS en el VPS

#### **Crear Application Pool**

1. Abre **IIS Manager**
2. Click derecho en **Application Pools** → **Add Application Pool**
3. **Configuración API:**
   - Name: `MiGenteEnLinea.API`
   - .NET CLR version: **No Managed Code**
   - Managed pipeline mode: **Integrated**
   - Start immediately: ✅

4. **Configuración Web:**
   - Name: `MiGenteEnLinea.Web`
   - .NET CLR version: **No Managed Code**
   - Managed pipeline mode: **Integrated**
   - Start immediately: ✅

#### **Crear Sitios Web**

**Sitio API:**
1. Click derecho en **Sites** → **Add Website**
2. Site name: `MiGente API`
3. Application pool: `MiGenteEnLinea.API`
4. Physical path: `C:\inetpub\MiGenteEnLinea\API`
5. Binding:
   - Type: **https**
   - IP address: **All Unassigned**
   - Port: **443**
   - Host name: `api.migenteenlinea.com`
   - SSL certificate: **[Tu certificado SSL]**

**Sitio Web:**
1. Click derecho en **Sites** → **Add Website**
2. Site name: `MiGente Web`
3. Application pool: `MiGenteEnLinea.Web`
4. Physical path: `C:\inetpub\MiGenteEnLinea\Web`
5. Binding:
   - Type: **https**
   - IP address: **All Unassigned**
   - Port: **443**
   - Host name: `www.migenteenlinea.com`
   - SSL certificate: **[Tu certificado SSL]**

---

### 5️⃣ Configurar Permisos (VPS)

```powershell
# Dar permisos de escritura a la carpeta de uploads
icacls "C:\inetpub\MiGenteEnLinea\API\wwwroot\uploads" /grant "IIS AppPool\MiGenteEnLinea.API:(OI)(CI)M" /T

# Dar permisos de escritura a logs
icacls "C:\inetpub\MiGenteEnLinea\API\logs" /grant "IIS AppPool\MiGenteEnLinea.API:(OI)(CI)M" /T
icacls "C:\inetpub\MiGenteEnLinea\Web\logs" /grant "IIS AppPool\MiGenteEnLinea.Web:(OI)(CI)M" /T
```

---

### 6️⃣ Instalar ASP.NET Core Runtime (si no está instalado)

**En el VPS:**

1. Descarga desde: https://dotnet.microsoft.com/download/dotnet/8.0
2. Instala: **ASP.NET Core Runtime 8.0 - Windows Hosting Bundle**
3. **Reinicia IIS:**
   ```cmd
   net stop was /y
   net start w3svc
   ```

---

### 7️⃣ Verificar Instalación

**Healthcheck API:**
```
https://api.migenteenlinea.com/health
```
**Debería devolver:**
```json
{
  "status": "Healthy",
  "timestamp": "2026-02-09T12:00:00Z"
}
```

**Swagger UI:**
```
https://api.migenteenlinea.com/
```

**Sitio Web:**
```
https://www.migenteenlinea.com/
```

---

## 🔧 Troubleshooting Común

### ❌ Error 500.19 - web.config inválido
**Solución:** Instala **URL Rewrite Module** para IIS
```
https://www.iis.net/downloads/microsoft/url-rewrite
```

### ❌ Error 500.30 - App no inicia
**Solución:**
1. Verifica que ASP.NET Core Runtime 8.0 esté instalado
2. Revisa logs en: `C:\inetpub\MiGenteEnLinea\API\logs\stdout_*.log`

### ❌ Error de conexión a SQL Server
**Solución:**
1. Verifica que SQL Server esté accesible desde el VPS
2. Prueba el connection string con **SQL Server Management Studio**
3. Verifica firewall permite puerto 1433

### ❌ Imágenes no cargan (404)
**Solución:**
1. Verifica permisos en `wwwroot\uploads`
2. Verifica que `StaticFilesBaseUrl` apunte correcto en Web
3. Prueba subir imagen de prueba en Contratista/Perfil

---

## 📋 Checklist de Post-Deployment

- [ ] API Health Check responde OK
- [ ] Swagger UI funciona
- [ ] Website carga correctamente
- [ ] Login funciona (crea usuario de prueba)
- [ ] Upload de imagen funciona
- [ ] Email de activación se envía
- [ ] Pago con Cardnet funciona (modo test primero)
- [ ] Logs se están escribiendo
- [ ] Certificado SSL válido y activo
- [ ] DNS apunta correctamente al VPS

---

## 🆘 Soporte

**Logs del API:** `C:\inetpub\MiGenteEnLinea\API\logs\`  
**Logs del Web:** `C:\inetpub\MiGenteEnLinea\Web\logs\`  
**Logs de IIS:** Event Viewer → Windows Logs → Application

**Comandos útiles de IIS:**
```powershell
# Reiniciar IIS
iisreset

# Reiniciar solo un Application Pool
Restart-WebAppPool -Name "MiGenteEnLinea.API"

# Ver estado de sitios
Get-Website | Format-Table Name, State, Bindings

# Ver logs de aplicación
Get-EventLog -LogName Application -Source "IIS*" -Newest 50
```

---

## ✅ ¡Listo!

Tu aplicación ahora debería estar corriendo en producción. Recuerda:

1. **Haz backup de la base de datos** antes de hacer cambios
2. **Monitorea los logs** las primeras 24 horas
3. **Configura Cardnet en modo producción** solo después de testear
4. **Habilita HTTPS** en el redirect de web.config después de configurar SSL

**Próximas actualizaciones:** Solo sube los archivos .dll modificados, no necesitas recompilar todo.
