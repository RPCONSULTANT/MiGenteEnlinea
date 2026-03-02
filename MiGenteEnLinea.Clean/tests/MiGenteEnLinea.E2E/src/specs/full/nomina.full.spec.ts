import { test, expect } from "../../fixtures/test-fixtures";
import { getRoleCredentials } from "../../config/env";
import { AuthPage } from "../../pages/AuthPage";
import { NominaPage } from "../../pages/NominaPage";
import { apiCall } from "../../helpers/api-client";
import { requireWriteAccess } from "../../config/env";

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
    requireWriteAccess("nomina-api-contract");

    const login = await apiCall(api, "/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: getRoleCredentials("empleador")
    });

    expect(login.status).toBe(200);
    const token = (login.json as any)?.accessToken;

    const historial = await apiCall(api, `/api/nominas/historial-unificado?pageIndex=1&pageSize=10`, {
      method: "GET",
      token
    });

    expect(historial.status, `Historial unificado falló: ${historial.text}`).toBe(200);
  });
});
