import { Page } from "@playwright/test";

export class AuthPage {
  constructor(private readonly page: Page) {}

  async openLogin(): Promise<void> {
    await this.page.goto("/Auth/Login", { waitUntil: "domcontentloaded" });
  }

  async openRegister(): Promise<void> {
    await this.page.goto("/Auth/Registrar", { waitUntil: "domcontentloaded" });
  }

  async login(email: string, password: string): Promise<void> {
    await this.page.fill('input[name="email"], input[type="email"]', email);
    await this.page.fill('input[name="password"], input[type="password"]', password);
    await this.page.click('button[type="submit"], input[type="submit"]');
  }
}
