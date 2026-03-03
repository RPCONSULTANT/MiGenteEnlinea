# Form Standards (UI/UX)

## Objetivo
Homologar formularios del sistema para reducir fricción y errores de captura.

## Reglas base
- Usar contenedor `mge-form` en formularios o bloques de captura.
- Usar `form-label` con texto claro y consistente.
- Aplicar validación inline y resumen superior cuando exista error de servidor.
- Botones de acción con estados: normal, loading, disabled.
- Evitar acciones redundantes (ej: validación manual de cédula en botón separado).

## Formato de datos
- Teléfono: máscara automática con `formatPhoneInput`.
- Cédula/identificación: máscara automática con `formatCedulaInput`.
- Montos: sanitización numérica y formato local al perder foco.

## Imagen de perfil
- Resolver imagen por prioridad: `fotoUrl` -> `fotoBase64` -> `foto` -> `imagenUrl` -> placeholder.
- Placeholder por defecto: `/images/circular1.png`.
- Siempre usar `onerror` para fallback visual.

## Responsive
- Desktop: formularios en columnas.
- Tablet/móvil: formularios en bloque vertical.
- Acciones principales visibles y accesibles en viewport pequeño.
