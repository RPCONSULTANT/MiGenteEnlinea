import { test, expect } from "../../fixtures/test-fixtures";
import { getRoleCredentials } from "../../config/env";
import { apiCall } from "../../helpers/api-client";
import { AuthPage } from "../../pages/AuthPage";
import { DashboardPage } from "../../pages/DashboardPage";

test.describe("@full @dashboard @catalogos @utilitarios Dashboard, catalogos, utilitarios", () => {
  test("@full @dashboard empleador and contratista dashboards render", async ({ page }) => {
    const authPage = new AuthPage(page);
    const dashboard = new DashboardPage(page);

    await authPage.openLogin();
    await authPage.login(getRoleCredentials("empleador").email, getRoleCredentials("empleador").password);
    await page.waitForLoadState("domcontentloaded");

    await dashboard.openEmpleadorDashboard();
    await expect(page).toHaveURL(/Empleador\/Index/i);

    await authPage.openLogin();
    await authPage.login(getRoleCredentials("contratista").email, getRoleCredentials("contratista").password);
    await page.waitForLoadState("domcontentloaded");

    await dashboard.openContratistaDashboard();
    await expect(page).toHaveURL(/Contratista\/Index/i);
  });

  test("@full @catalogos @utilitarios catalog and util endpoints respond", async ({ api }) => {
    const provincias = await apiCall(api, "/api/catalogos/provincias");
    const sectores = await apiCall(api, "/api/catalogos/sectores");
    const servicios = await apiCall(api, "/api/catalogos/servicios");

    expect(provincias.status).toBe(200);
    expect(sectores.status).toBe(200);
    expect(servicios.status).toBe(200);

    const numeroLetras = await apiCall(api, "/api/utilitarios/numero-a-letras?numero=1234.56");
    expect([200, 400]).toContain(numeroLetras.status);
  });
});
