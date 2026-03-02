import { test, expect } from "../../fixtures/test-fixtures";
import { getRoleCredentials } from "../../config/env";
import { AuthPage } from "../../pages/AuthPage";
import { apiCall } from "../../helpers/api-client";
import { env } from "../../config/env";
import { requireWriteAccess } from "../../config/env";

test.describe("@full @auth Auth end-to-end", () => {
  test("@full @auth register legacy + activate + login + rollback", async ({ page, api, rollback }) => {
    requireWriteAccess("auth-register-activate");

    const unique = `${Date.now()}_${Math.floor(Math.random() * 10000)}`;
    const email = `e2e_${unique}@example.com`;
    const password = "E2E.Test@123";

    const register = await apiCall(api, "/api/auth/register", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: {
        tipo: 1,
        nombre: "E2E",
        apellido: `User_${unique}`,
        email,
        telefono1: "8095550000",
        host: env.webBaseUrl
      }
    });

    expect([200, 201]).toContain(register.status);

    const userId = (register.json as any)?.userId ?? (register.json as any)?.data?.userId;
    expect(userId).toBeTruthy();

    rollback.register(async () => {
      await apiCall(api, "/api/auth/delete-user", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: { userId, email }
      });
    });

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

    expect([200, 204]).toContain(activate.status);

    const authPage = new AuthPage(page);
    await authPage.openLogin();
    await authPage.login(email, password);
    await page.waitForLoadState("domcontentloaded");
    await page.waitForTimeout(1000);

    await expect(page.url()).not.toContain("/Auth/Login");

    const forgot = await apiCall(api, "/api/auth/forgot-password", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: { email }
    });
    expect(forgot.status).toBe(200);
  });

  test("@full @auth refresh and revoke contract", async ({ api }) => {
    const creds = getRoleCredentials("empleador");

    const login = await apiCall(api, "/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: creds
    });

    expect(login.status).toBe(200);

    const refreshToken = (login.json as any)?.refreshToken;
    expect(refreshToken).toBeTruthy();

    const refresh = await apiCall(api, "/api/auth/refresh", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: { refreshToken }
    });

    expect([200, 401]).toContain(refresh.status);

    const revoke = await apiCall(api, "/api/auth/revoke", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: { refreshToken }
    });

    expect([200, 204, 401]).toContain(revoke.status);
  });
});
