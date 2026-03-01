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

  test("@full @pagos preflight and process payment contract", async ({ api }) => {
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

    const process = await apiCall(api, "/api/pagos/procesar", {
      method: "POST",
      token,
      headers: { "Content-Type": "application/json" },
      body: {
        userId,
        planId: firstPlanId,
        cardNumber: "4111111111111111",
        cardHolderName: "E2E USER",
        expMonth: "12",
        expYear: "30",
        cvv: "123"
      }
    });

    expect([200, 400, 401, 409, 422]).toContain(process.status);
  });
});
