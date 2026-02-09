# 🧪 IMAGE UPLOAD - TESTING & USAGE GUIDE

**Last Updated:** February 9, 2026  
**Status:** Ready for Browser Testing

---

## 🚀 QUICK START - HOW TO TEST

### Step 1: Start the Backend API

```powershell
cd "c:\Users\Ray\Documents\MiGenteEnlinea\MiGenteEnLinea.Clean"
dotnet run --project src/Presentation/MiGenteEnLinea.API
```

Expected output:
```
Listening on http://localhost:5015 and https://localhost:5016
```

### Step 2: Start the Frontend Web App

```powershell
cd "c:\Users\Ray\Documents\MiGenteEnlinea\MiGenteEnLinea.Clean"
dotnet run --project src/Presentation/MiGenteEnLinea.Web
```

Expected output:
```
Listening on http://localhost:5000 and https://localhost:5001
```

### Step 3: Login to Application

1. Open browser: `https://localhost:5001`
2. Login as **Empleador** (Employer)
3. Navigate: **Dashboard → Contrataciones Temporales**

### Step 4: Upload Contractor Photo

1. Click **"Cargar Foto"** button (top-right corner)
2. Modal opens: "Cargar Foto de Perfil"
3. Select an image file:
   - Format: JPG, PNG, or GIF
   - Size: Less than 5MB
   - Example: `contractor-photo.jpg`
4. Image preview displays
5. Click **"Cargar foto"** button
6. Wait for upload to complete
7. Success message: "¡Éxito! Foto cargada y actualizada correctamente"
8. Modal closes automatically

### Step 5: Verify Upload

**Option A: Check in Database**
```sql
-- Connect to db_a9f8ff_migente database
SELECT 
    c.contratistaID,
    c.nombre,
    c.apellido,
    c.imagenURL,
    c.foto
FROM Contratistas c
WHERE c.imagenURL IS NOT NULL
ORDER BY c.contratistaID DESC
LIMIT 5;
```

Expected result: `imagenURL` column populated with `/uploads/contratistas-fotos/20260209_xxxxx.jpg`

**Option B: Check File System**
```powershell
Get-ChildItem -Path "c:\Users\Ray\Documents\MiGenteEnlinea\MiGenteEnLinea.Clean\src\Presentation\MiGenteEnLinea.Web\wwwroot\uploads\contratistas-fotos\" -File
```

Expected result: Files with pattern `{timestamp}_{guid}.{ext}`

**Option C: Check in Browser**
1. After upload, refresh the page
2. Look at Contrataciones table avatars
3. Contractor photos should now display (no longer generic placeholders)

---

## 📋 TEST SCENARIOS

### ✅ SUCCESS SCENARIO

**Input:**
- File: `photo.jpg` (2MB, valid JPG)
- Format: JPG
- Size: Within 5MB

**Expected Result:**
- ✅ Preview displays
- ✅ "Cargando..." button animation
- ✅ Success notification
- ✅ Modal closes
- ✅ Form resets
- ✅ Page reloads
- ✅ Image persists in database
- ✅ Image visible in Contrataciones table

---

### ⚠️ VALIDATION ERROR SCENARIOS

**Test 1: Invalid File Format**
- Input: Select `.txt` or `.pdf` file
- Expected: Error alert "Formato de archivo no soportado. Use JPG, PNG o GIF"

**Test 2: File Too Large**
- Input: Select file > 5MB
- Expected: Error alert "El archivo excede el tamaño máximo permitido de 5MB"

**Test 3: No File Selected**
- Action: Click upload without selecting file
- Expected: Error alert "Selecciona una foto primero"

**Test 4: Empty File**
- Input: Create empty file and select it
- Expected: Error alert "No se proporcionó ningún archivo"

---

### 🔐 SECURITY TESTS

**Test 1: Without Authentication**
- Action: Try to upload without logging in
- Expected: Redirect to login page

**Test 2: Without JWT Token**
- Action: Disable JavaScript, manually call API without token
- Expected: 401 Unauthorized error

**Test 3: Expired Token**
- Setup: Wait for token to expire (8 hours)
- Action: Try to upload
- Expected: 401 Unauthorized error

---

### 🔄 INTEGRATION TESTS

**Test 1: Image Persistence**
1. Upload image
2. Close browser
3. Reopen application
4. Navigate to Contrataciones
5. Expected: Image still displays (persisted in DB)

**Test 2: Image Display in Tables**
1. Upload image for contractor
2. Check all three tabs (Pendientes, Activas, Completadas)
3. Expected: Image displays in avatar column (40x40 circular)

**Test 3: Rating with Updated Image**
1. Upload image
2. Click star icon on contract
3. Go to Calificaciones page
4. Expected: Updated image displays in rating section

**Test 4: Multiple Users**
1. Create multiple contractors
2. Each uploads different images
3. Expected: Each image stored separately with unique names
4. Check database: Multiple entries in Contratistas.imagenURL

---

## 🗂️ FILE LOCATIONS

### Uploaded Files Location:
```
wwwroot/uploads/contratistas-fotos/
├─ 20260209_123456_abc123def456.jpg
├─ 20260209_234567_xyz789uvw012.png
└─ 20260209_345678_hij345klm678.gif
```

### Configuration:
```
API Configuration: http://localhost:5015
- Upload Endpoint: POST /api/contratistas/{userId}/foto
- Accept: multipart/form-data
- Max Size: 5MB
- Required: JWT Bearer token

Web Application: http://localhost:5000
- Contrataciones View: /Empleador/Contrataciones
- Upload trigger: abrirModalFoto() function
```

### Database:
```
Table: Contratistas
Column: imagenURL (VARCHAR 150)
Update: Automatic on successful upload
Persistence: Permanent until deleted
```

---

## 🐛 TROUBLESHOOTING

### Issue: "Error al procesar la carga de foto"

**Cause 1: wwwroot/uploads/ directory missing**
```powershell
# Solution: Manually create directory
New-Item -Path "MiGenteEnLinea.Clean\src\Presentation\MiGenteEnLinea.Web\wwwroot\uploads\contratistas-fotos" -ItemType Directory -Force
```

**Cause 2: Insufficient permissions**
```powershell
# Solution: Grant write permissions to wwwroot folder
icacls "C:\Path\To\wwwroot" /grant:r "%USERNAME%:F"
```

**Cause 3: IFileStorageService not registered**
```csharp
// Check DependencyInjection.cs for:
services.AddScoped<IFileStorageService, LocalFileStorageService>();
```

---

### Issue: "No hay token de autenticación"

**Cause:** JWT token not stored in localStorage

**Solution:** 
1. Ensure you've logged in successfully
2. Check browser DevTools:
   - Application → Storage → Local Storage
   - Look for key: `jwtToken`
   - Should contain long JWT string

---

### Issue: Image not displaying in table after upload

**Cause 1:** Page not reloaded after upload
**Solution:** Refresh page (F5) to see updated image

**Cause 2:** Image URL not persisted to database
**Solution:** Check database query results for imageURL value

**Cause 3:** Wrong URL format in HTML
**Solution:** Verify URL starts with `/` (absolute path from webroot)

---

### Issue: "No se pueden convertir los datos del formulario"

**Cause:** Content-Type mismatch in form submission

**Solution:** Ensure FormData is used, not JSON:
```javascript
const formData = new FormData();
formData.append('file', fileInput.files[0]);
// DON'T JSON.stringify()
```

---

## 📊 MONITORING & LOGGING

### API Logs:

Check console output for messages like:
```
info: MiGenteEnLinea.Infrastructure.Services.LocalFileStorageService[0]
      Archivo guardado exitosamente
      
info: MiGenteEnLinea.API.Controllers.ContrastistasController[0]
      Foto actualizada exitosamente
```

### Database Logs:

Enable SQL logging in appsettings:
```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

This shows all SQL queries executed.

---

## 🎯 PERFORMANCE BENCHMARKS

### Expected Performance:

| Operation | Time | Notes |
|-----------|------|-------|
| File upload (1MB JPG) | 0.5-1 sec | Over LAN |
| File validation | <100ms | Client-side + Server-side |
| Database persist | <200ms | Single row UPDATE |
| Image display refresh | Instant | CSS no-cache |
| Full roundtrip | 1-2 sec | Total end-to-end |

### Resource Usage:

- **Memory:** ~5-10MB per upload (stream-based, not buffered)
- **Disk Space:** Based on file size + backup overhead
- **CPU:** Minimal (no image processing)
- **Network:** Upload size only

---

## ✅ VALIDATION CHECKLIST

Run through this before considering implementation complete:

```
BACKEND:
- [ ] API endpoint responds to POST /api/contratistas/{userId}/foto
- [ ] Files saved to wwwroot/uploads/contratistas-fotos/
- [ ] imagenURL column populated in database
- [ ] Error messages returned for invalid files
- [ ] Logging shows successful operations
- [ ] Rate limiting working (if configured)

FRONTEND:
- [ ] "Cargar Foto" button visible on Contrataciones page
- [ ] Modal opens when button clicked
- [ ] File preview displays after selection
- [ ] Upload button shows loading state
- [ ] Success notification appears after upload
- [ ] Modal closes automatically on success
- [ ] Table refreshes with new image

DATABASE:
- [ ] Records updated in Contratistas.imagenURL
- [ ] Timestamps (CreatedAt/UpdatedAt) updated
- [ ] Data persists after page reload
- [ ] Audit trail recorded (if configured)

SECURITY:
- [ ] JWT authentication required
- [ ] File type validation working
- [ ] File size validation working
- [ ] User can only upload for their own account
- [ ] Error messages don't leak sensitive info
```

---

## 📧 SUPPORT & DOCUMENTATION

For issues or questions:

1. **Check this document:** Most common issues covered
2. **Review logs:** API console and browser DevTools
3. **Inspect database:** Verify data persistence
4. **Run tests:** Use test scenarios above

Key files for reference:
- `IMAGE_UPLOAD_IMPLEMENTATION_REPORT.md` - Technical details
- `LocalFileStorageService.cs` - File I/O implementation
- `UpdateContratistaFotoCommandHandler.cs` - Domain logic
- `ContrastistasController.cs` - API endpoint
- `Contrataciones.cshtml` - Frontend UI

---

**Status:** ✅ Ready to Test  
**Expected Result:** Full image upload pipeline functional  
**Time to Verify:** ~15 minutes with above steps
