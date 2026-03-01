import { test, expect } from "../../fixtures/test-fixtures";
import { getRoleCredentials } from "../../config/env";
import { AuthPage } from "../../pages/AuthPage";
import { NominaPage } from "../../pages/NominaPage";
import { apiCall } from "../../helpers/api-client";
import { env } from "../../config/env";

test.describe("@full @nomina Nomina and receipts", () => {
  test("@full @nomina nomina page renders", async ({ page }) => {
    const authPage = new AuthPage(page);
    const nomina = new NominaPage(page);

    await authPage.openLogin();
    await authPage.login(getRoleCredentials("empleador").email, getRoleCredentials("empleador").password);
    await page.waitForLoadState("domcontentloaded");

    await nomina.openNomina();
    await expect(page).toHaveURL(/Empleador\/Nomina/i);
  });

  test("@full @nomina nomina api contract", async ({ api }) => {
    test.skip(!env.allowWrite, "E2E_ALLOW_WRITE=false, skipping full write scenario");

    const login = await apiCall(api, "/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: getRoleCredentials("empleador")
    });

    expect(login.status).toBe(200);
    const token = (login.json as any)?.accessToken;
    const userId = (login.json as any)?.user?.userId;

    const historial = await apiCall(api, `/api/nominas/historial/${userId}`, {
      method: "GET",
      token
    });

    expect([200, 204, 404]).toContain(historial.status);
  });
});
