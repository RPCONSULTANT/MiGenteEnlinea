import { test, expect } from "../../fixtures/test-fixtures";

test.describe("@full @public Public pages", () => {
  test("@full @public Home FAQ renders without 5xx", async ({ page, runtimeIssues }) => {
    const response = await page.goto("/Home/FAQ", { waitUntil: "domcontentloaded" });
    const status = response?.status() ?? 0;

    if (status >= 500) {
      runtimeIssues.push({
        type: "http-error",
        message: `Public FAQ failed with status ${status}`,
        url: page.url()
      });
    }

    expect(status).toBeLessThan(500);
    await expect(page.locator("h1")).toContainText(/Preguntas Frecuentes/i);
  });
});

