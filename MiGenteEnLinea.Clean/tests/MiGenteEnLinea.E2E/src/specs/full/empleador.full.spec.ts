import { test, expect } from "../../fixtures/test-fixtures";
import { getRoleCredentials } from "../../config/env";
import { AuthPage } from "../../pages/AuthPage";
import { EmpleadorPage } from "../../pages/EmpleadorPage";
import { apiCall } from "../../helpers/api-client";

test.describe("@full @empleador Empleador module", () => {
  test("@full @empleador dashboard pages render", async ({ page }) => {
    const creds = getRoleCredentials("empleador");
    const authPage = new AuthPage(page);
    const empleadorPage = new EmpleadorPage(page);

    await authPage.openLogin();
    await authPage.login(creds.email, creds.password);
    await page.waitForLoadState("domcontentloaded");

    await empleadorPage.openIndex();
    await expect(page).toHaveURL(/Empleador\/Index/i);

    await empleadorPage.openEmpleados();
    await expect(page).toHaveURL(/Empleador\/Empleados/i);

    await empleadorPage.openContrataciones();
    await expect(page).toHaveURL(/Empleador\/Contrataciones/i);

    await empleadorPage.openAdquirirPlan();
    await expect(page).toHaveURL(/Empleador\/AdquirirPlan/i);
  });

  test("@full @empleador profile endpoint by-user responds", async ({ api }) => {
    const login = await apiCall(api, "/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: getRoleCredentials("empleador")
    });

    expect(login.status).toBe(200);
    const token = (login.json as any)?.accessToken;
    const userId = (login.json as any)?.user?.userId;
    expect(userId).toBeTruthy();

    const profile = await apiCall(api, `/api/empleadores/by-user/${userId}`, {
      method: "GET",
      token
    });

    expect([200, 404]).toContain(profile.status);
  });

  test("@full @empleador directory hire flow asks fixed or temporary", async ({ page, runtimeIssues }) => {
    const creds = getRoleCredentials("empleador");
    const authPage = new AuthPage(page);

    await authPage.openLogin();
    await authPage.login(creds.email, creds.password);
    await page.goto("/Empleador/Buscador", { waitUntil: "domcontentloaded" });

    const hireButton = page.locator(".btn-contratar").first();
    const count = await hireButton.count();
    if (count === 0) {
      runtimeIssues.push({
        type: "ui-error",
        message: "No contract buttons found in Empleador/Buscador",
        url: page.url()
      });
      test.skip(true, "No contractors available for hire flow");
    }

    await hireButton.click();
    await expect(page.getByText(/Tipo de contratación/i)).toBeVisible();

    await page.getByRole("button", { name: /Temporal/i }).click();
    await expect(page).toHaveURL(/Empleador\/Contrataciones/i);
    await expect(page.locator("#modalNuevaContratacion.show")).toBeVisible();
  });

  test("@full @empleador directory fixed hire opens empleados modal prefill", async ({ page, runtimeIssues }) => {
    const creds = getRoleCredentials("empleador");
    const authPage = new AuthPage(page);

    await authPage.openLogin();
    await authPage.login(creds.email, creds.password);
    await page.goto("/Empleador/Buscador", { waitUntil: "domcontentloaded" });

    const hireButton = page.locator(".btn-contratar").first();
    const count = await hireButton.count();
    if (count === 0) {
      runtimeIssues.push({
        type: "ui-error",
        message: "No contract buttons found in Empleador/Buscador for fixed flow",
        url: page.url()
      });
      test.skip(true, "No contractors available for fixed hire flow");
    }

    await hireButton.click();
    await expect(page.getByText(/Tipo de contratación/i)).toBeVisible();

    await page.getByRole("button", { name: /^Fija$/i }).click();
    await expect(page).toHaveURL(/Empleador\/Empleados/i);
    await expect(page.locator("#registroEmpleadoModal.show")).toBeVisible();
  });
});
