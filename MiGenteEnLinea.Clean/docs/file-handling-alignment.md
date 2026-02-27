# File Handling Alignment Matrix

## Scope
- API: `ContratistasController`, `EmpleadoresController`, `IFileStorageService`, `LocalFileStorageService`
- Deploy: `publish-and-deploy-ftp.ps1`
- Config: `appsettings.json`, `appsettings.Production.json`

## Current contract (post-hardening)

| Area | Endpoint / Flow | Storage | Status | Notes |
| --- | --- | --- | --- | --- |
| Contratista upload | `POST /api/contratistas/{userId}/foto` | Filesystem + URL (`imagenURL`) | OK | Validación de extensión/MIME/firma y tamaño centralizada en `IFileStorageService`. |
| Contratista read | `GET /api/contratistas/{contratistaId}/foto` | Filesystem (primary) + DB bytes (legacy fallback) | OK | Lee `ImagenUrl` primero; fallback temporal a bytes legacy si existen. |
| Empleador upload | `PUT /api/empleadores/{userId}/foto` | Filesystem + DB bytes (compat) | COMPAT | Guarda archivo en filesystem y mantiene update legacy en `byte[]` para no romper esquema actual. |
| Empleador read | DTO `tieneFoto` | DB bytes (legacy) | LEGACY | Pendiente migración de esquema para URL persistente en `Ofertantes`. |
| Runtime static files | `/uploads/...` | `wwwroot/uploads` | OK | Servido por `UseStaticFiles()`. |
| Deploy uploads | FTP publish | Persistente | OK | Script excluye `wwwroot/uploads/*` de sobrescritura. |

## Security controls implemented
- Validación de carpeta permitida (`AllowedFolders`).
- Normalización canónica y bloqueo de path traversal.
- Validación de extensión + MIME + firma binaria.
- Límite único de tamaño (`FileStorage.MaxFileSizeMB`) aplicado en servicio y `FormOptions`.
- Logging estructurado por operación (`file.upload.*`, `file.read.*`, `file.delete.*`, `file.exists.*`).

## Pending migration (explicit)
- Objetivo final: `Empleadores` con URL persistente en BD (sin `byte[] foto`).
- Requiere cambio de esquema y actualización de queries/DTO para `fotoUrl`.
