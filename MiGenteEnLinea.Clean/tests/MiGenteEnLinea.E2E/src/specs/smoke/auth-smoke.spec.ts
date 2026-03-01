import { test, expect } from "../../fixtures/test-fixtures";
import { getRoleCredentials } from "../../config/env";
import { AuthPage } from "../../pages/AuthPage";
import { assertNoCriticalIssues, persistIssues } from "../../helpers/error-monitor";

test.describe("@smoke @auth Auth smoke", () => {
  test("@smoke login page posts credentials and reaches protected area", async ({ page, runtimeIssues }, testInfo) => {
    const creds = getRoleCredentials("empleador");
    const authPage = new AuthPage(page);

    await authPage.openLogin();
    await authPage.login(creds.email, creds.password);

    await page.waitForLoadState("domcontentloaded");
    await page.waitForTimeout(1500);

    await expect(page.url()).not.toContain("/Auth/Login");

    await persistIssues(testInfo, runtimeIssues);
    assertNoCriticalIssues(runtimeIssues, [/401/, /403/, /favicon.ico/i]);
  });

  test("@smoke forgot password endpoint accepts neutral request", async ({ api }) => {
    const response = await api.post("/api/auth/forgot-password", {
      data: { email: "e2e_probe_not_found@example.com" },
      headers: { "Content-Type": "application/json" }
    });
    expect(response.status()).toBe(200);
  });
});
