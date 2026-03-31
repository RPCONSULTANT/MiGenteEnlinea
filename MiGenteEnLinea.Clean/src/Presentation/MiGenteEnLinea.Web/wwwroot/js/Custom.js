document.write(new Date().getFullYear());

// ========================================
// FUNCIONES COMPARTIDAS PARA TODA LA APP
// ========================================

/**
 * URL base del API
 * Prioridad:
 * 1. window.API_BASE (inyectado desde servidor - producción)
 * 2. Mapeo por hostname (producción)
 * 3. localhost:5015 (desarrollo)
 * 4. /api (fallback relativo)
 */
const API_BASE_BY_HOST = {
  "plattaformv2.migenteenlinea.do": {
    http: "http://api2.migenteenlinea.do/api",
    https: "https://api2.migenteenlinea.do/api",
  },
  "platformv2.migenteenlinea.do": {
    http: "http://api2.migenteenlinea.do/api",
    https: "https://api2.migenteenlinea.do/api",
  },
  "www.migenteenlinea.do": {
    http: "http://api2.migenteenlinea.do/api",
    https: "https://api2.migenteenlinea.do/api",
  },
  "migenteenlinea.do": {
    http: "http://api2.migenteenlinea.do/api",
    https: "https://api2.migenteenlinea.do/api",
  },
};

function resolveMappedApiBase(hostname, protocol) {
  const entry = API_BASE_BY_HOST[hostname];
  if (!entry) return null;
  if (typeof entry === "string") return entry;
  if (protocol === "https:" && entry.https) return entry.https;
  if (entry.http) return entry.http;
  return entry.https || null;
}

const resolvedMappedBase = resolveMappedApiBase(window.location.hostname, window.location.protocol);

const API_BASE_RAW =
  window.API_BASE ||
  resolvedMappedBase ||
  (window.location.hostname === "localhost"
    ? "http://localhost:5015/api"
    : "/api");

const API_BASE = API_BASE_RAW.replace(/\/+$/, "");

function buildApiUrl(path) {
  if (!path) return API_BASE;
  if (/^https?:\/\//i.test(path)) return path;
  return `${API_BASE}/${String(path).replace(/^\/+/, "")}`;
}

if (!window.API_BASE && window.location.hostname !== "localhost") {
  console.warn(
    `[API CONFIG] window.API_BASE no fue inyectado; host='${window.location.hostname}', usando fallback '${API_BASE}'. Verifique el layout activo.`,
  );
}

if (window.location.hostname !== "localhost" && !/\/api$/i.test(API_BASE)) {
  console.warn(
    `[API CONFIG] API_BASE final '${API_BASE}' no termina en '/api'. Revise configuración de entorno/layout.`,
  );
}

function isNetworkOrCorsError(error) {
  if (!error) return false;
  const message = String(error.message || error).toLowerCase();
  return (
    message.includes("failed to fetch") ||
    message.includes("networkerror") ||
    message.includes("network request failed") ||
    message.includes("load failed")
  );
}

async function readApiResponse(response) {
  const contentType = (response.headers.get("content-type") || "").toLowerCase();
  const text = await response.text();

  if (!text) {
    return null;
  }

  if (contentType.includes("application/json")) {
    try {
      return JSON.parse(text);
    } catch (error) {
      console.warn("[API PARSE] JSON inválido recibido:", error);
      return { raw: text };
    }
  }

  return {
    message: text,
    raw: text,
  };
}

function getApiErrorMessage(payload, fallbackMessage) {
  if (!payload) return fallbackMessage;

  if (Array.isArray(payload.errors)) {
    const firstError = payload.errors[0];
    if (typeof firstError === "string" && firstError.trim()) {
      return firstError.trim();
    }
    if (firstError && typeof firstError === "object") {
      if (typeof firstError.message === "string" && firstError.message.trim()) {
        return firstError.message.trim();
      }
      if (typeof firstError.errorMessage === "string" && firstError.errorMessage.trim()) {
        return firstError.errorMessage.trim();
      }
    }
  }

  if (payload.errors && typeof payload.errors === "object") {
    const values = Object.values(payload.errors).flat();
    for (const value of values) {
      if (typeof value === "string" && value.trim()) {
        return value.trim();
      }
      if (value && typeof value === "object") {
        if (typeof value.message === "string" && value.message.trim()) {
          return value.message.trim();
        }
        if (typeof value.errorMessage === "string" && value.errorMessage.trim()) {
          return value.errorMessage.trim();
        }
      }
    }
  }

  if (payload.Errors && typeof payload.Errors === "object") {
    const values = Object.values(payload.Errors).flat();
    for (const value of values) {
      if (typeof value === "string" && value.trim()) {
        return value.trim();
      }
      if (value && typeof value === "object") {
        if (typeof value.message === "string" && value.message.trim()) {
          return value.message.trim();
        }
        if (typeof value.errorMessage === "string" && value.errorMessage.trim()) {
          return value.errorMessage.trim();
        }
      }
    }
  }

  const rawMessage = payload.message || payload.Message || payload.title || payload.raw || fallbackMessage;
  return typeof rawMessage === "string" ? rawMessage : fallbackMessage;
}

/**
 * Realiza un fetch autenticado con manejo automático de errores 401
 * @param {string} url - URL relativa o absoluta del endpoint
 * @param {object} options - Opciones de fetch (method, body, etc.)
 * @returns {Promise<Response>} - Promesa con la respuesta del fetch
 * 
 * Uso ejemplo:
 * const { response, payload } = await requestApi('/empleados?soloActivos=true');
 * if (response.ok) {
 *   console.log(payload);
 * }
 */
async function authenticatedFetch(url, options = {}) {
  // Get authentication token
  const token = localStorage.getItem('accessToken') || localStorage.getItem('token');
  
  if (!token) {
    console.error('No authentication token found');
    window.location.href = '/Auth/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
    throw new Error('No authentication token');
  }
  
  // Ensure URL is absolute (add API_BASE if relative)
  const fullUrl = buildApiUrl(url);
  
  // Detectar si el body es FormData (no agregar Content-Type para que browser lo maneje)
  const isFormData = options.body instanceof FormData;
  
  // Merge headers with Authorization
  const headers = {
    'Authorization': `Bearer ${token}`,
    ...options.headers
  };
  
  // Solo agregar Content-Type si NO es FormData
  if (!isFormData && !headers['Content-Type']) {
    headers['Content-Type'] = 'application/json';
  }
  
  // Perform fetch
  const response = await fetch(fullUrl, {
    ...options,
    headers
  });
  
  // Handle 401 Unauthorized (expired/invalid token)
  if (response.status === 401) {
    await Swal.fire({
      title: 'Sesión Expirada',
      text: 'Tu sesión ha expirado. Por favor inicia sesión nuevamente.',
      icon: 'warning',
      confirmButtonText: 'Ir a Login'
    });

    if (window.clearClientSession) {
      await window.clearClientSession();
    } else {
      localStorage.removeItem('accessToken');
      localStorage.removeItem('token');
      sessionStorage.clear();
    }

    window.location.href = '/Auth/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
    throw new Error('Unauthorized - Session expired');
  }
  
  return response;
}

async function requestApi(path, options = {}, config = {}) {
  const useAuth = config.auth !== false;
  const requestFn = useAuth ? authenticatedFetch : fetch;
  const url = useAuth ? path : buildApiUrl(path);
  const response = await requestFn(url, useAuth ? options : {
    ...options,
    headers: {
      ...(options.headers || {}),
    },
  });
  const payload = await readApiResponse(response);
  return { response, payload };
}

/**
 * Renderiza estrellas de calificación basado en un rating numérico
 * @param {number} rating - Calificaci\u00f3n de 0 a 5
 * @returns {string} HTML con iconos de estrellas
 */
function renderStars(rating) {
  if (!rating || rating < 0) rating = 0;
  if (rating > 5) rating = 5;

  const fullStars = Math.floor(rating);
  const hasHalfStar = rating % 1 >= 0.5;
  const emptyStars = 5 - fullStars - (hasHalfStar ? 1 : 0);

  let html = "";

  // Estrellas llenas
  for (let i = 0; i < fullStars; i++) {
    html += '<i class="fa fa-star"></i>';
  }

  // Media estrella
  if (hasHalfStar) {
    html += '<i class="fa fa-star-half-alt"></i>';
  }

  // Estrellas vac\u00edas
  for (let i = 0; i < emptyStars; i++) {
    html += '<i class="far fa-star"></i>';
  }

  return html;
}

/**
 * Carga din\u00e1micamente provincias en un select
 * @param {string} selectId - ID del elemento select
 * @param {string} defaultOption - Texto de la opci\u00f3n por defecto
 */
async function loadProvincias(
  selectId,
  defaultOption = "-- Seleccione Provincia --",
) {
  try {
    const { response, payload } = await requestApi(
      window.API_ENDPOINTS?.CATALOGOS?.PROVINCIAS || "/catalogos/provincias",
      {},
      { auth: false },
    );

    if (!response.ok) {
      throw new Error(getApiErrorMessage(payload, "No se pudieron cargar las provincias"));
    }

    const provincias = Array.isArray(payload) ? payload : [];

    const select = document.getElementById(selectId);
    if (!select) return;

    // Limpiar opciones existentes
    select.innerHTML = "";

    // Agregar opci\u00f3n por defecto
    const defaultOpt = document.createElement("option");
    defaultOpt.value = "";
    defaultOpt.textContent = defaultOption;
    select.appendChild(defaultOpt);

    // Agregar provincias
    provincias.forEach((p) => {
      const option = document.createElement("option");
      option.value = p.nombre;
      option.textContent = p.nombre;
      select.appendChild(option);
    });
  } catch (error) {
    console.error("Error cargando provincias:", error);
  }
}

/**
 * Carga din\u00e1micamente sectores en un select
 * @param {string} selectId - ID del elemento select
 * @param {string} defaultOption - Texto de la opci\u00f3n por defecto
 */
async function loadSectores(
  selectId,
  defaultOption = "-- Seleccione Sector --",
) {
  try {
    const { response, payload } = await requestApi(
      window.API_ENDPOINTS?.CATALOGOS?.SECTORES || "/catalogos/sectores",
      {},
      { auth: false },
    );

    if (!response.ok) {
      throw new Error(getApiErrorMessage(payload, "No se pudieron cargar los sectores"));
    }

    const sectores = Array.isArray(payload) ? payload : [];

    const select = document.getElementById(selectId);
    if (!select) return;

    // Limpiar opciones existentes
    select.innerHTML = "";

    // Agregar opci\u00f3n por defecto
    const defaultOpt = document.createElement("option");
    defaultOpt.value = "";
    defaultOpt.textContent = defaultOption;
    select.appendChild(defaultOpt);

    // Agregar sectores
    sectores.forEach((s) => {
      const option = document.createElement("option");
      option.value = s.sector;
      option.textContent = s.sector;
      select.appendChild(option);
    });
  } catch (error) {
    console.error("Error cargando sectores:", error);
  }
}

/**
 * Carga din\u00e1micamente servicios en un select
 * @param {string} selectId - ID del elemento select
 * @param {string} defaultOption - Texto de la opci\u00f3n por defecto
 */
async function loadServicios(
  selectId,
  defaultOption = "-- Seleccione Servicio --",
) {
  try {
    const { response, payload } = await requestApi(
      window.API_ENDPOINTS?.CATALOGOS?.SERVICIOS || "/catalogos/servicios",
      {},
      { auth: false },
    );

    if (!response.ok) {
      throw new Error(getApiErrorMessage(payload, "No se pudieron cargar los servicios"));
    }

    const servicios = Array.isArray(payload) ? payload : [];

    const select = document.getElementById(selectId);
    if (!select) return;

    // Limpiar opciones existentes
    select.innerHTML = "";

    // Agregar opci\u00f3n por defecto
    const defaultOpt = document.createElement("option");
    defaultOpt.value = "";
    defaultOpt.textContent = defaultOption;
    select.appendChild(defaultOpt);

    // Agregar servicios
    servicios.forEach((s) => {
      const option = document.createElement("option");
      option.value = s.descripcion;
      option.textContent = s.descripcion;
      select.appendChild(option);
    });
  } catch (error) {
    console.error("Error cargando servicios:", error);
  }
}

/**
 * Formatea un n\u00famero como moneda RD$
 * @param {number} amount - Cantidad a formatear
 * @returns {string} Cantidad formateada como RD$ 1,234.56
 */
function formatCurrency(amount) {
  if (!amount || isNaN(amount)) return "RD$ 0.00";
  return (
    "RD$ " +
    parseFloat(amount).toLocaleString("es-DO", {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2,
    })
  );
}

/**
 * Formatea una fecha en formato DD/MM/YYYY
 * @param {string|Date} dateStr - Fecha a formatear
 * @returns {string} Fecha formateada
 */
function formatDate(dateStr) {
  if (!dateStr) return "N/A";
  const date = new Date(dateStr);
  if (isNaN(date)) return "N/A";

  const day = String(date.getDate()).padStart(2, "0");
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const year = date.getFullYear();

  return `${day}/${month}/${year}`;
}

function sanitizeNumeric(value, maxLength = Number.MAX_SAFE_INTEGER) {
  return String(value || "").replace(/\D/g, "").slice(0, maxLength);
}

function formatPhoneDigits(digits) {
  const clean = sanitizeNumeric(digits, 10);
  if (clean.length > 6) return `${clean.slice(0, 3)}-${clean.slice(3, 6)}-${clean.slice(6)}`;
  if (clean.length > 3) return `${clean.slice(0, 3)}-${clean.slice(3)}`;
  return clean;
}

function formatCedulaDigits(digits) {
  const clean = sanitizeNumeric(digits, 11);
  if (clean.length > 10) return `${clean.slice(0, 3)}-${clean.slice(3, 10)}-${clean.slice(10)}`;
  if (clean.length > 3) return `${clean.slice(0, 3)}-${clean.slice(3)}`;
  return clean;
}

function applyMaskedInput(input, formatter, maxDigits) {
  if (!input) return;
  const previousValue = String(input.value || "");
  const previousStart = input.selectionStart || 0;
  const digits = sanitizeNumeric(previousValue, maxDigits);
  const nextValue = formatter(digits);
  input.value = nextValue;

  const delta = nextValue.length - previousValue.length;
  const nextCaret = Math.max(0, Math.min(nextValue.length, previousStart + delta));
  if (typeof input.setSelectionRange === "function") {
    requestAnimationFrame(() => input.setSelectionRange(nextCaret, nextCaret));
  }
}

async function loadMunicipios(
  selectId,
  provincia,
  defaultOption = "-- Seleccione Municipio --",
) {
  try {
    const path = window.API_ENDPOINTS?.CATALOGOS?.MUNICIPIOS
      ? window.API_ENDPOINTS.CATALOGOS.MUNICIPIOS(provincia)
      : `/catalogos/municipios${provincia ? `?provincia=${encodeURIComponent(provincia)}` : ""}`;

    const { response, payload } = await requestApi(path, {}, { auth: false });

    if (!response.ok) {
      throw new Error(getApiErrorMessage(payload, "No se pudieron cargar los municipios"));
    }

    const municipios = Array.isArray(payload) ? payload : [];
    const select = document.getElementById(selectId);
    if (!select) return;

    select.innerHTML = "";

    const defaultOpt = document.createElement("option");
    defaultOpt.value = "";
    defaultOpt.textContent = defaultOption;
    select.appendChild(defaultOpt);

    municipios.forEach((m) => {
      const option = document.createElement("option");
      option.value = m.nombre;
      option.textContent = m.nombre;
      select.appendChild(option);
    });
  } catch (error) {
    console.error("Error cargando municipios:", error);
  }
}

function formatPhoneInput(input) {
  applyMaskedInput(input, formatPhoneDigits, 10);
}

function formatCedulaInput(input) {
  applyMaskedInput(input, formatCedulaDigits, 11);
}

// Exponer funciones globalmente
window.renderStars = renderStars;
window.loadProvincias = loadProvincias;
window.loadMunicipios = loadMunicipios;
window.loadSectores = loadSectores;
window.loadServicios = loadServicios;
window.formatCurrency = formatCurrency;
window.formatDate = formatDate;
window.sanitizeNumeric = sanitizeNumeric;
window.formatPhoneDigits = formatPhoneDigits;
window.formatCedulaDigits = formatCedulaDigits;
window.formatPhoneInput = formatPhoneInput;
window.formatCedulaInput = formatCedulaInput;
window.API_BASE = API_BASE;
window.buildApiUrl = buildApiUrl;
window.readApiResponse = readApiResponse;
window.getApiErrorMessage = getApiErrorMessage;
window.requestApi = requestApi;
window.isNetworkOrCorsError = isNetworkOrCorsError;

// Accessibility hardening for modal focus transitions.
document.addEventListener("hide.bs.modal", () => {
  if (document.activeElement && typeof document.activeElement.blur === "function") {
    document.activeElement.blur();
  }
});

document.addEventListener("hidden.bs.modal", () => {
  if (document.body && typeof document.body.focus === "function") {
    document.body.focus();
  }
});
