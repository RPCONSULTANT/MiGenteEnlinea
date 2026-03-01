import { Page } from "@playwright/test";

export class ContratistaPage {
  constructor(private readonly page: Page) {}

  async openIndex(): Promise<void> {
    await this.page.goto("/Contratista/Index", { waitUntil: "domcontentloaded" });
  }

  async openPerfil(): Promise<void> {
    await this.page.goto("/Contratista/Perfil", { waitUntil: "domcontentloaded" });
  }

  async openDirectorio(): Promise<void> {
    await this.page.goto("/Contratista/Directorio", { waitUntil: "domcontentloaded" });
  }

  async openSuscripciones(): Promise<void> {
    await this.page.goto("/Contratista/Suscripciones", { waitUntil: "domcontentloaded" });
  }
}
