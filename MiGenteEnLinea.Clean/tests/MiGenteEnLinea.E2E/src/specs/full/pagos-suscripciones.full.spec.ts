import { test, expect } from "../../fixtures/test-fixtures";
import { getRoleCredentials } from "../../config/env";
import { AuthPage } from "../../pages/AuthPage";
import { PlanesPagosPage } from "../../pages/PlanesPagosPage";
import { apiCall } from "../../helpers/api-client";

test.describe("@full @pagos @suscripciones Planes, pagos, suscripciones", () => {
  test("@full @pagos checkout pages load", async ({ page }) => {
    const creds = getRoleCredentials("empleador");
    const authPage = new AuthPage(page);
    const plansPage = new PlanesPagosPage(page);

    await authPage.openLogin();
    await authPage.login(creds.email, creds.password);
    await page.waitForLoadState("domcontentloaded");

    await plansPage.openEmpleadorAdquirirPlan();
    await expect(page).toHaveURL(/Empleador\/AdquirirPlan/i);

    await plansPage.openEmpleadorCheckout(1);
    await expect(page).toHaveURL(/Empleador\/Checkout/i);
  });

  test("@full @pagos preflight and simple checkout contract", async ({ api }) => {
    const preflight = await api.fetch("/api/pagos/procesar", {
      method: "OPTIONS",
      headers: {
        Origin: "http://plattaformv2.migenteenlinea.do",
        "Access-Control-Request-Method": "POST",
        "Access-Control-Request-Headers": "authorization,content-type"
      }
    });

    expect([200, 204]).toContain(preflight.status());

    const login = await apiCall(api, "/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: getRoleCredentials("empleador")
    });

    expect(login.status).toBe(200);

    const token = (login.json as any)?.accessToken;
    const userId = (login.json as any)?.user?.userId;

    const planes = await apiCall(api, "/api/suscripciones/planes/empleadores", { method: "GET" });
    expect(planes.status).toBe(200);

    const firstPlanId = (planes.json as any)?.[0]?.planId ?? (planes.json as any)?.[0]?.id;
    expect(firstPlanId).toBeTruthy();

    const simple = await apiCall(api, "/api/pagos/procesar-simple", {
      method: "POST",
      token,
      headers: { "Content-Type": "application/json" },
      body: {
        userId,
        planId: firstPlanId,
        motivo: "E2E fake simple checkout"
      }
    });

    expect([200, 409]).toContain(simple.status);
    if (simple.status === 200) {
      expect((simple.json as any)?.ventaId).toBeTruthy();
    }

    const invalidCard = await apiCall(api, "/api/pagos/procesar", {
      method: "POST",
      token,
      headers: { "Content-Type": "application/json" },
      body: {
        userId,
        planId: firstPlanId,
        cardNumber: "1111",
        cvv: "12",
        expirationDate: "0120",
        referenceNumber: "E2E-BAD-CARD",
        invoiceNumber: "E2E-BAD-CARD"
      }
    });

    expect(invalidCard.status).toBe(400);
    const message = (invalidCard.json as any)?.message;
    expect(typeof message).toBe("string");
    expect(message.length).toBeGreaterThan(0);
  });
});
