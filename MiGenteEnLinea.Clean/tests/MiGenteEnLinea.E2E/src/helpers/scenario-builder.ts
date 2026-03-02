import { APIRequestContext, expect } from "@playwright/test";
import { env, getRoleCredentials, Role, requireWriteAccess } from "../config/env";
import { apiCall } from "./api-client";

type LoginDetails = {
  token: string;
  userId: string;
};

export type DirectoryData = {
  contratistaId: number;
  empleadorId: number;
  contratistaNombre: string;
};

export type TempHiringChain = {
  contratacionId: number;
  detalleId: number;
  contratistaId: number;
  userId: string;
};

async function loginWithDetails(api: APIRequestContext, role: Role): Promise<LoginDetails> {
  const login = await apiCall(api, "/api/auth/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: getRoleCredentials(role)
  });

  expect(login.status, `Login ${role} debe responder 200`).toBe(200);

  const token = (login.json as any)?.accessToken;
  const userId = (login.json as any)?.user?.userId;
  expect(token, `Token faltante para ${role}`).toBeTruthy();
  expect(userId, `userId faltante para ${role}`).toBeTruthy();

  return { token, userId };
}

async function registerAndActivateUser(api: APIRequestContext, tipo: 1 | 2): Promise<void> {
  requireWriteAccess("bootstrap-register");

  const unique = `${env.runId}_${Date.now()}_${Math.floor(Math.random() * 10000)}`;
  const email = `e2e_${tipo}_${unique}@example.com`;
  const password = "E2E.Test@123";

  const register = await apiCall(api, "/api/auth/register", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: {
      tipo,
      nombre: "E2E",
      apellido: `Auto_${unique}`.slice(0, 40),
      email,
      telefono1: "8095550000",
      host: env.webBaseUrl
    }
  });

  expect([200, 201], `Register tipo=${tipo} falló: ${register.text}`).toContain(register.status);
  const userId = (register.json as any)?.userId ?? (register.json as any)?.data?.userId;
  expect(userId, "Register no retornó userId").toBeTruthy();

  const activate = await apiCall(api, "/api/auth/activate", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: {
      userId,
      email,
      password,
      confirmPassword: password
    }
  });

  expect([200, 204], `Activate tipo=${tipo} falló: ${activate.text}`).toContain(activate.status);
}

async function getContratistas(api: APIRequestContext, token: string): Promise<any[]> {
  const response = await apiCall(api, "/api/contratistas?pageIndex=1&pageSize=20", {
    method: "GET",
    token
  });

  expect(response.status, `GET /api/contratistas falló: ${response.text}`).toBe(200);
  const payload = response.json as any;
  return Array.isArray(payload?.contratistas) ? payload.contratistas : Array.isArray(payload) ? payload : [];
}

async function getEmpleadores(api: APIRequestContext, token: string): Promise<any[]> {
  const response = await apiCall(api, "/api/empleadores?pageIndex=1&pageSize=20", {
    method: "GET",
    token
  });

  expect(response.status, `GET /api/empleadores falló: ${response.text}`).toBe(200);
  const payload = response.json as any;
  return Array.isArray(payload?.empleadores) ? payload.empleadores : Array.isArray(payload) ? payload : [];
}

export async function ensureDirectoryLinkData(api: APIRequestContext): Promise<DirectoryData> {
  const empleadorLogin = await loginWithDetails(api, "empleador");
  const contratistaLogin = await loginWithDetails(api, "contratista");

  let contratistas = await getContratistas(api, empleadorLogin.token);
  if (contratistas.length === 0) {
    await registerAndActivateUser(api, 2);
    contratistas = await getContratistas(api, empleadorLogin.token);
  }

  let empleadores = await getEmpleadores(api, contratistaLogin.token);
  if (empleadores.length === 0) {
    await registerAndActivateUser(api, 1);
    empleadores = await getEmpleadores(api, contratistaLogin.token);
  }

  expect(contratistas.length, "No hay contratistas disponibles para pruebas funcionales.").toBeGreaterThan(0);
  expect(empleadores.length, "No hay empleadores disponibles para pruebas funcionales.").toBeGreaterThan(0);

  const selectedContratista = contratistas.find((x) => Number(x?.contratistaId) > 0) ?? contratistas[0];
  const selectedEmpleador = empleadores.find((x) => Number(x?.empleadorId) > 0) ?? empleadores[0];

  return {
    contratistaId: Number(selectedContratista?.contratistaId ?? 0),
    empleadorId: Number(selectedEmpleador?.empleadorId ?? 0),
    contratistaNombre: String(
      selectedContratista?.nombreCompleto ??
        `${selectedContratista?.nombre ?? ""} ${selectedContratista?.apellido ?? ""}`.trim()
    )
  };
}

export async function ensureTempHiringChain(api: APIRequestContext): Promise<TempHiringChain> {
  requireWriteAccess("temp-hiring-chain");

  const { token, userId } = await loginWithDetails(api, "empleador");
  const directory = await ensureDirectoryLinkData(api);
  const now = new Date();
  const start = new Date(now.getTime() + 24 * 60 * 60 * 1000);
  const end = new Date(now.getTime() + 7 * 24 * 60 * 60 * 1000);
  const isoDate = (d: Date) => d.toISOString().slice(0, 10);
  const unique = `${Date.now()}`;

  const temporal = await apiCall(api, "/api/empleados/temporales", {
    method: "POST",
    token,
    headers: { "Content-Type": "application/json" },
    body: {
      userId: "client-should-be-ignored",
      tipo: 1,
      nombre: "Temp",
      apellido: `Flow_${unique}`,
      identificacion: `TMP${unique}`.slice(0, 20),
      telefono: "8095550000",
      direccion: "Santo Domingo",
      servicio: "Servicio general",
      fechaInicio: start.toISOString(),
      fechaFinal: end.toISOString(),
      pago: 1000,
      lugarTrabajo: "Remoto",
      horarioTrabajo: "08:00-17:00",
      estatus: 1
    }
  });

  expect([200, 201], `Crear temporal falló: ${temporal.text}`).toContain(temporal.status);
  const contratacionId = Number((temporal.json as any)?.contratacionId ?? temporal.json ?? 0);
  expect(contratacionId, "No se pudo obtener contratacionId del temporal").toBeGreaterThan(0);

  const contratacion = await apiCall(api, "/api/contrataciones", {
    method: "POST",
    token,
    headers: { "Content-Type": "application/json" },
    body: {
      contratacionId,
      contratistaId: directory.contratistaId,
      servicioId: null,
      descripcionCorta: `E2E Temporal ${unique}`.slice(0, 60),
      descripcionAmpliada: `Flujo temporal E2E ${env.runId}`.slice(0, 250),
      fechaInicio: isoDate(start),
      fechaFinal: isoDate(end),
      montoAcordado: 1000,
      esquemaPagos: "Pago único",
      notas: `runId=${env.runId}`.slice(0, 500)
    }
  });

  expect([200, 201], `Crear contratación falló: ${contratacion.text}`).toContain(contratacion.status);
  const detalleId = Number((contratacion.json as any)?.detalleId ?? contratacion.json ?? 0);
  expect(detalleId, "No se pudo obtener detalleId").toBeGreaterThan(0);

  return {
    contratacionId,
    detalleId,
    contratistaId: directory.contratistaId,
    userId
  };
}
