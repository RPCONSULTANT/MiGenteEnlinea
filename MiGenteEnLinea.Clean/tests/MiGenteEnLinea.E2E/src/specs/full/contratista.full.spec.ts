import { test, expect } from "../../fixtures/test-fixtures";
import { getRoleCredentials } from "../../config/env";
import { AuthPage } from "../../pages/AuthPage";
import { ContratistaPage } from "../../pages/ContratistaPage";
import { apiCall } from "../../helpers/api-client";

test.describe("@full @contratista Contratista module", () => {
  test("@full @contratista dashboard pages render", async ({ page }) => {
    const creds = getRoleCredentials("contratista");
    const authPage = new AuthPage(page);
    const contratistaPage = new ContratistaPage(page);

    await authPage.openLogin();
    await authPage.login(creds.email, creds.password);
    await page.waitForLoadState("domcontentloaded");

    await contratistaPage.openIndex();
    await expect(page).toHaveURL(/Contratista\/Index/i);

    await contratistaPage.openPerfil();
    await expect(page).toHaveURL(/Contratista\/Perfil/i);

    await contratistaPage.openDirectorio();
    await expect(page).toHaveURL(/Contratista\/Directorio/i);

    await contratistaPage.openSuscripciones();
    await expect(page).toHaveURL(/Contratista\/Suscripciones/i);
  });

  test("@full @contratista profile and services endpoints respond", async ({ api }) => {
    const login = await apiCall(api, "/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: getRoleCredentials("contratista")
    });

    expect(login.status).toBe(200);
    const token = (login.json as any)?.accessToken;
    const userId = (login.json as any)?.user?.userId;
    expect(userId).toBeTruthy();

    const byUser = await apiCall(api, `/api/contratistas/by-user/${userId}`, {
      method: "GET",
      token
    });

    expect([200, 404]).toContain(byUser.status);

    const contratistaId = (byUser.json as any)?.contratistaId ?? (byUser.json as any)?.id;
    if (contratistaId) {
      const servicios = await apiCall(api, `/api/contratistas/${contratistaId}/servicios`, {
        method: "GET",
        token
      });
      expect([200, 404]).toContain(servicios.status);
    }
  });
});
