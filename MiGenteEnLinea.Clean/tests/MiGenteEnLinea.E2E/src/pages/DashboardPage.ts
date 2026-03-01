import { Page } from "@playwright/test";

export class DashboardPage {
  constructor(private readonly page: Page) {}

  async openEmpleadorDashboard(): Promise<void> {
    await this.page.goto("/Empleador/Index", { waitUntil: "domcontentloaded" });
  }

  async openContratistaDashboard(): Promise<void> {
    await this.page.goto("/Contratista/Index", { waitUntil: "domcontentloaded" });
  }
}
