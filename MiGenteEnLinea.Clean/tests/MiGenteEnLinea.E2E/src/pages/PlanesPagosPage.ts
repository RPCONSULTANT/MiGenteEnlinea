import { Page } from "@playwright/test";

export class PlanesPagosPage {
  constructor(private readonly page: Page) {}

  async openEmpleadorAdquirirPlan(): Promise<void> {
    await this.page.goto("/Empleador/AdquirirPlan", { waitUntil: "domcontentloaded" });
  }

  async openContratistaAdquirirPlan(): Promise<void> {
    await this.page.goto("/Contratista/AdquirirPlan", { waitUntil: "domcontentloaded" });
  }

  async openEmpleadorCheckout(planId = 1): Promise<void> {
    await this.page.goto(`/Empleador/Checkout?planId=${planId}`, { waitUntil: "domcontentloaded" });
  }

  async openContratistaCheckout(planId = 4): Promise<void> {
    await this.page.goto(`/Contratista/Checkout?planId=${planId}`, { waitUntil: "domcontentloaded" });
  }
}
