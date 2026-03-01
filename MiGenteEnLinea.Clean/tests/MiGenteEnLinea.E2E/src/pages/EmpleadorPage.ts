import { Page } from "@playwright/test";

export class EmpleadorPage {
  constructor(private readonly page: Page) {}

  async openIndex(): Promise<void> {
    await this.page.goto("/Empleador/Index", { waitUntil: "domcontentloaded" });
  }

  async openEmpleados(): Promise<void> {
    await this.page.goto("/Empleador/Empleados", { waitUntil: "domcontentloaded" });
  }

  async openContrataciones(): Promise<void> {
    await this.page.goto("/Empleador/Contrataciones", { waitUntil: "domcontentloaded" });
  }

  async openAdquirirPlan(): Promise<void> {
    await this.page.goto("/Empleador/AdquirirPlan", { waitUntil: "domcontentloaded" });
  }
}
