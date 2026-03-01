import { defineConfig } from "@playwright/test";

const webBaseUrl = process.env.E2E_WEB_BASE_URL ?? "http://plattaformv2.migenteenlinea.do";

export default defineConfig({
  testDir: "./src/specs",
  fullyParallel: false,
  retries: process.env.CI ? 1 : 0,
  workers: process.env.CI ? 2 : undefined,
  timeout: 60000,
  expect: {
    timeout: 10000
  },
  reporter: [
    ["list"],
    ["html", { outputFolder: "playwright-report", open: "never" }],
    ["json", { outputFile: "artifacts/e2e/playwright-report.json" }]
  ],
  outputDir: "test-results",
  use: {
    baseURL: webBaseUrl,
    headless: true,
    trace: "on-first-retry",
    screenshot: "only-on-failure",
    video: "retain-on-failure",
    viewport: { width: 1440, height: 900 },
    actionTimeout: 20000,
    navigationTimeout: 30000
  },
  projects: [
    {
      name: "chromium",
      use: { browserName: "chromium" }
    }
  ]
});
