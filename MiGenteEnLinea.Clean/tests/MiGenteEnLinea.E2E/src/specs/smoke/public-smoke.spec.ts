import { test, expect } from "../../fixtures/test-fixtures";
import { assertNoCriticalIssues, persistIssues } from "../../helpers/error-monitor";

test.describe("@smoke @public Public web and api smoke", () => {
  test("@smoke home page renders and links auth", async ({ page, runtimeIssues }, testInfo) => {
    await page.goto("/");
    await expect(page).toHaveTitle(/Mi Gente/i);

    await page.goto("/Auth/Login");
    await expect(page).toHaveTitle(/Iniciar Sesion|Iniciar Sesión/i);

    await page.goto("/Auth/Registrar");
    await expect(page).toHaveTitle(/Registrar/i);

    await persistIssues(testInfo, runtimeIssues);
    assertNoCriticalIssues(runtimeIssues, [/favicon.ico/i]);
  });

  test("@smoke api health and swagger available", async ({ api }) => {
    const health = await api.get("/health");
    expect(health.status()).toBe(200);

    const swagger = await api.get("/swagger/v1/swagger.json");
    expect(swagger.status()).toBe(200);
  });

  test("@smoke catalog and plans endpoints respond", async ({ api }) => {
    const provincias = await api.get("/api/catalogos/provincias");
    expect(provincias.status()).toBe(200);

    const planesEmp = await api.get("/api/suscripciones/planes/empleadores");
    expect(planesEmp.status()).toBe(200);
    const payloadEmp = await planesEmp.json();
    expect(Array.isArray(payloadEmp)).toBeTruthy();
    expect(payloadEmp.length).toBeGreaterThan(0);

    const planesCont = await api.get("/api/suscripciones/planes/contratistas");
    expect(planesCont.status()).toBe(200);
    const payloadCont = await planesCont.json();
    expect(Array.isArray(payloadCont)).toBeTruthy();
    expect(payloadCont.length).toBeGreaterThan(0);
  });
});
