document.write(new Date().getFullYear());

// ========================================
// FUNCIONES COMPARTIDAS PARA TODA LA APP
// ========================================

/**
 * URL base del API
 * Prioridad:
 * 1. window.API_BASE (inyectado desde servidor - producción)
 * 2. localhost:5015 (desarrollo)
 * 3. /api (fallback relativo)
 */
const API_BASE =
  window.API_BASE ||
  (window.location.hostname === "localhost"
    ? "http://localhost:5015/api"
    : "/api");

/**
 * Realiza un fetch autenticado con manejo automático de errores 401
 * @param {string} url - URL relativa o absoluta del endpoint
 * @param {object} options - Opciones de fetch (method, body, etc.)
 * @returns {Promise<Response>} - Promesa con la respuesta del fetch
 * 
 * Uso ejemplo:
 * const response = await authenticatedFetch('/empleados?soloActivos=true');
 * if (response.ok) {
 *   const data = await response.json();
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
  const fullUrl = url.startsWith('http') ? url : `${API_BASE}${url}`;
  
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
    
    localStorage.removeItem('accessToken');
    localStorage.removeItem('token');
    window.location.href = '/Auth/Login?returnUrl=' + encodeURIComponent(window.location.pathname);
    throw new Error('Unauthorized - Session expired');
  }
  
  return response;
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
    const response = await fetch(`${API_BASE}/catalogos/provincias`);
    const provincias = await response.json();

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
    const response = await fetch(`${API_BASE}/catalogos/sectores`);
    const sectores = await response.json();

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
    const response = await fetch(`${API_BASE}/catalogos/servicios`);
    const servicios = await response.json();

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

// Exponer funciones globalmente
window.renderStars = renderStars;
window.loadProvincias = loadProvincias;
window.loadSectores = loadSectores;
window.loadServicios = loadServicios;
window.formatCurrency = formatCurrency;
window.formatDate = formatDate;
window.API_BASE = API_BASE;
