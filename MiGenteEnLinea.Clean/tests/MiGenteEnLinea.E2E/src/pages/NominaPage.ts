import { Page } from "@playwright/test";

export class NominaPage {
  constructor(private readonly page: Page) {}

  async openNomina(): Promise<void> {
    await this.page.goto("/Empleador/Nomina", { waitUntil: "domcontentloaded" });
  }
}
