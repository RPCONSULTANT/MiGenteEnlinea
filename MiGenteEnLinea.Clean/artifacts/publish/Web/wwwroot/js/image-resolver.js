(function () {
  const DEFAULT_PLACEHOLDER = "/images/circular1.png";

  function resolveStaticBase() {
    const explicit = String(window.STATIC_FILES_BASE || "").trim();
    if (explicit) return explicit.replace(/\/+$/, "");

    const apiBase = String(window.API_BASE || "").trim();
    if (!apiBase) return "";

    // API_BASE suele venir como https://host/api, para archivos estáticos necesitamos origen base.
    const withoutApi = apiBase.replace(/\/api\/?$/i, "");
    return withoutApi.replace(/\/+$/, "");
  }

  function normalizeImageSource(rawValue) {
    const raw = String(rawValue || "").trim();
    if (!raw) return null;

    if (raw.startsWith("data:image")) return raw;
    if (raw.startsWith("http://") || raw.startsWith("https://")) return raw;
    if (raw.startsWith("/uploads/")) {
      const staticBase = resolveStaticBase();
      return staticBase ? `${staticBase}${raw}` : raw;
    }
    if (raw.startsWith("/")) return raw;

    if (/^[A-Za-z0-9+/=]+$/.test(raw) && raw.length > 80) {
      return `data:image/jpeg;base64,${raw}`;
    }

    return raw;
  }

  function getImageSource(entity, options) {
    const keys = Array.isArray(options?.keys) && options.keys.length
      ? options.keys
      : ["fotoUrl", "foto", "imagenUrl", "imagen", "fotoBase64"];

    for (const key of keys) {
      const normalized = normalizeImageSource(entity?.[key]);
      if (normalized) return normalized;
    }

    const fallbackBuilder = options?.fallbackBuilder;
    if (typeof fallbackBuilder === "function") {
      const fallback = normalizeImageSource(fallbackBuilder(entity));
      if (fallback) return fallback;
    }

    return options?.placeholder || DEFAULT_PLACEHOLDER;
  }

  function appendImageVersion(url, version) {
    const normalized = normalizeImageSource(url);
    if (!normalized || normalized.startsWith("data:image")) return normalized;
    const token = encodeURIComponent(String(version ?? Date.now()));
    return normalized.includes("?") ? `${normalized}&v=${token}` : `${normalized}?v=${token}`;
  }

  function resolveContratistaImageSource(entity, options) {
    const keys = Array.isArray(options?.keys) && options.keys.length
      ? options.keys
      : ["fotoUrl", "FotoUrl", "imagenUrl", "ImagenUrl", "fotoBase64", "foto", "imagen"];

    const fallbackBuilder = options?.fallbackBuilder || ((item) => {
      const id = Number(
        item?.contratistaId ||
        item?.ContratistaId ||
        item?.contratistaID ||
        item?.id ||
        0
      );
      if (id <= 0) return null;

      const fotoPath = window.API_ENDPOINTS?.CONTRATISTAS?.FOTO
        ? window.API_ENDPOINTS.CONTRATISTAS.FOTO(id)
        : `/contratistas/${id}/foto`;
      const apiBase = String(window.API_BASE || "").trim().replace(/\/+$/, "");
      return apiBase ? `${apiBase}${fotoPath}` : fotoPath;
    });

    return getImageSource(entity, {
      keys,
      fallbackBuilder,
      placeholder: options?.placeholder || DEFAULT_PLACEHOLDER
    });
  }

  window.DEFAULT_IMAGE_PLACEHOLDER = DEFAULT_PLACEHOLDER;
  window.normalizeImageSource = normalizeImageSource;
  window.resolveEntityImageSource = getImageSource;
  window.appendImageVersion = appendImageVersion;
  window.resolveContratistaImageSource = resolveContratistaImageSource;
})();
