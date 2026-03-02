// Shared API endpoint catalog for frontend views/scripts.
// Keep paths relative to API base (without "/api" prefix).
(function () {
  const ENDPOINTS = {
    AUTH: {
      LOGIN: "/auth/login",
      REGISTER: "/auth/register",
      ACTIVATE: "/auth/activate",
      FORGOT_PASSWORD: "/auth/forgot-password",
      RESET_PASSWORD: "/auth/reset-password",
      RESEND_ACTIVATION: "/auth/resend-activation",
    },
    PAGOS: {
      PROCESAR: "/pagos/procesar",
      PROCESAR_SIMPLE: "/pagos/procesar-simple",
      HISTORIAL: (userId) => `/pagos/historial/${userId}`,
    },
    SUSCRIPCIONES: {
      ACTIVA: (userId) => `/suscripciones/activa/${userId}`,
      VENTAS: (userId) => `/suscripciones/ventas/${userId}`,
      PLANES_EMPLEADORES: "/suscripciones/planes/empleadores",
      PLANES_CONTRATISTAS: "/suscripciones/planes/contratistas",
      CANCELAR: (userId) => `/suscripciones/${userId}`,
    },
    NOMINAS: {
      PROCESAR_LOTE: "/nominas/procesar-lote",
      RESUMEN: (periodo) => `/nominas/resumen?periodo=${encodeURIComponent(periodo)}`,
      HISTORIAL: "/nominas/historial",
      HISTORIAL_BY_USER: (userId) => `/nominas/historial/${userId}`,
      RECIBO_PDF: (reciboId) => `/nominas/recibo/${reciboId}/pdf`,
    },
    EMPLEADORES: {
      BY_USER: (userId) => `/empleadores/by-user/${userId}`,
    },
    EMPLEADOS: {
      LIST: "/empleados",
      LIST_ACTIVOS: "/empleados?soloActivos=true&pageSize=100",
      PADRON: (cedula) => `/empleados/padron/${cedula}`,
      DETALLE: (empleadoId) => `/empleados/${empleadoId}`,
      NOMINA: (empleadoId) => `/empleados/${empleadoId}/nomina`,
      RECIBOS: (empleadoId) => `/empleados/${empleadoId}/recibos?pageSize=50`,
      DAR_DE_BAJA: (empleadoId) => `/empleados/${empleadoId}/dar-de-baja`,
      REACTIVAR: (empleadoId) => `/empleados/${empleadoId}/reactivar`,
      CONTRATO: (empleadoId, tipo) => `/empleados/${empleadoId}/contrato?tipoContrato=${encodeURIComponent(tipo)}`,
      DESCARGO: (empleadoId, tipo) => `/empleados/${empleadoId}/descargo?tipoContrato=${encodeURIComponent(tipo)}`,
      RESUMEN_USO: "/empleados/resumen-uso",
    },
    CONTRATISTAS: {
      BY_USER: (userId) => `/contratistas/by-user/${userId}`,
      FOTO: (contratistaId) => `/contratistas/${contratistaId}/foto`,
      FOTO_UPLOAD: (userId) => `/contratistas/${userId}/foto`,
      SERVICIOS: (contratistaId) => `/contratistas/${contratistaId}/servicios`,
      ACTIVAR: (userId) => `/contratistas/${userId}/activar`,
      DESACTIVAR: (userId) => `/contratistas/${userId}/desactivar`,
      CEDULA: (userId) => `/contratistas/cedula/${userId}`,
      UPDATE: (userId) => `/contratistas/${userId}`,
      SERVICIO_DELETE: (contratistaId, servicioId) => `/contratistas/${contratistaId}/servicios/${servicioId}`,
    },
    CONTRATACIONES: {
      LIST: "/contrataciones?pageSize=100",
      CREATE: "/contrataciones",
      DETALLE: (detalleId) => `/contrataciones/${detalleId}`,
      START: (detalleId) => `/contrataciones/${detalleId}/start`,
      COMPLETE: (detalleId) => `/contrataciones/${detalleId}/complete`,
      CANCEL: (detalleId) => `/contrataciones/${detalleId}/cancel`,
      NO_CALIFICADAS: "/contrataciones?soloNoCalificadas=true&pageSize=100",
    },
    CALIFICACIONES: {
      POR_EMPLEADOR: (userId) => `/calificaciones/por-empleador/${userId}`,
      POR_CONTRATISTA: (identificacion) => `/calificaciones/contratista/${identificacion}?pageSize=50`,
      CALIFICAR_PERFIL: "/calificaciones/calificar-perfil",
    },
    CONSULTAS: {
      COUNT: (userId) => `/consultas/count/${userId}`,
    },
    DASHBOARD: {
      EMPLEADOR: "/dashboard/empleador",
      CONTRATISTA: "/dashboard/contratista",
      HEALTH: "/dashboard/health",
    },
    CATALOGOS: {
      PROVINCIAS: "/catalogos/provincias",
      SECTORES: "/catalogos/sectores",
      SERVICIOS: "/catalogos/servicios",
    },
    UTILITARIOS: {
      NUMERO_A_LETRAS: "/utilitarios/numero-a-letras",
    },
  };

  window.API_ENDPOINTS = ENDPOINTS;
})();
