import { test, expect } from "../../fixtures/test-fixtures";
import { APIRequestContext } from "@playwright/test";
import { apiCall } from "../../helpers/api-client";
import { ensureTempHiringChain } from "../../helpers/scenario-builder";
import { requireWriteAccess } from "../../config/env";
import { getRoleCredentials } from "../../config/env";

async function loginEmpleadorToken(api: APIRequestContext): Promise<string> {
  const creds = getRoleCredentials("empleador");
  const login = await apiCall(api, "/api/auth/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: creds
  });

  expect(login.status, `Login empleador falló: ${login.text}`).toBe(200);
  const token = (login.json as any)?.accessToken;
  expect(token).toBeTruthy();
  return token;
}

test.describe("@full @empleador @contrataciones Contrataciones temporales flow", () => {
  test.setTimeout(180000);

  test("@full @empleador create -> accept -> start -> complete -> contract-pdf", async ({ api }) => {
    requireWriteAccess("temp-flow-state-machine");
    const chain = await ensureTempHiringChain(api);
    const token = await loginEmpleadorToken(api);

    const accept = await apiCall(api, `/api/contrataciones/${chain.detalleId}/accept`, {
      method: "PUT",
      token
    });
    expect(accept.status, `Accept falló: ${accept.text}`).toBe(200);

    const contractPdf = await apiCall(api, `/api/contrataciones/${chain.detalleId}/contrato-pdf`, {
      method: "GET",
      token
    });
    expect(contractPdf.status, `Contrato PDF falló: ${contractPdf.text}`).toBe(200);
    expect((contractPdf.headers["content-type"] ?? "").toLowerCase()).toContain("application/pdf");

    const start = await apiCall(api, `/api/contrataciones/${chain.detalleId}/start`, {
      method: "PUT",
      token
    });
    expect(start.status, `Start falló: ${start.text}`).toBe(200);

    const complete = await apiCall(api, `/api/contrataciones/${chain.detalleId}/complete`, {
      method: "PUT",
      token
    });
    expect(complete.status, `Complete falló: ${complete.text}`).toBe(200);
  });

  test("@full @empleador process temporary payment and verify historial-unificado", async ({ api }) => {
    requireWriteAccess("temp-flow-payment");
    const chain = await ensureTempHiringChain(api);
    const token = await loginEmpleadorToken(api);

    const accept = await apiCall(api, `/api/contrataciones/${chain.detalleId}/accept`, {
      method: "PUT",
      token
    });
    expect([200, 400], `Accept pre-payment falló: ${accept.text}`).toContain(accept.status);

    const start = await apiCall(api, `/api/contrataciones/${chain.detalleId}/start`, {
      method: "PUT",
      token
    });
    expect([200, 400], `Start pre-payment falló: ${start.text}`).toContain(start.status);

    const complete = await apiCall(api, `/api/contrataciones/${chain.detalleId}/complete`, {
      method: "PUT",
      token
    });
    expect([200, 400], `Complete pre-payment falló: ${complete.text}`).toContain(complete.status);

    const now = new Date();
    const payment = await apiCall(api, "/api/nominas/contrataciones/procesar-pago", {
      method: "POST",
      token,
      headers: { "Content-Type": "application/json" },
      body: {
        userId: chain.userId,
        contratacionId: chain.contratacionId,
        detalleId: chain.detalleId,
        fechaRegistro: now.toISOString(),
        fechaPago: now.toISOString(),
        conceptoPago: "Pago Final",
        tipo: 2,
        detalles: [{ concepto: "Pago Final", monto: 1000 }]
      }
    });
    expect([200, 201], `Pago temporal falló: ${payment.text}`).toContain(payment.status);

    const historial = await apiCall(api, "/api/nominas/historial-unificado?pageIndex=1&pageSize=50", {
      method: "GET",
      token
    });
    expect(historial.status, `Historial unificado falló: ${historial.text}`).toBe(200);

    const items = Array.isArray(historial.json) ? historial.json : (historial.json as any)?.items ?? [];
    expect(items.length, "El historial unificado debe retornar registros").toBeGreaterThan(0);
  });
});
