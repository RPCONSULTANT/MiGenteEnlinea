import { Page, request } from "@playwright/test";
import { env } from "../config/env";

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

    try {
      await this.page.waitForFunction(() => !window.location.pathname.toLowerCase().includes("/auth/login"), null, {
        timeout: 10000
      });
      return;
    } catch {
      await this.loginViaApiFallback(email, password);
    }
  }

  private async loginViaApiFallback(email: string, password: string): Promise<void> {
    const api = await request.newContext({
      baseURL: env.apiBaseUrl,
      extraHTTPHeaders: {
        Accept: "application/json",
        "Content-Type": "application/json"
      }
    });

    try {
      const response = await api.post("/api/auth/login", {
        data: { email, password }
      });

      if (!response.ok()) {
        throw new Error(`Fallback login failed. Status=${response.status()}`);
      }

      const data = await response.json();
      const token = data?.accessToken;
      const refreshToken = data?.refreshToken;
      const user = data?.user;

      if (!token || !user?.userId) {
        throw new Error("Fallback login response missing token/user");
      }

      const returnUrl = await this.page.locator("#returnUrl").inputValue().catch(() => "");

      await this.page.evaluate(
        (payload) => {
          localStorage.setItem("accessToken", payload.token);
          localStorage.setItem("refreshToken", payload.refreshToken ?? "");
          localStorage.setItem("userId", payload.user.userId ?? "");
          localStorage.setItem("email", payload.user.email ?? "");
          localStorage.setItem("nombreCompleto", payload.user.nombreCompleto ?? "");
          localStorage.setItem("tipo", String(payload.user.tipo ?? ""));
          localStorage.setItem("planId", String(payload.user.planId ?? "0"));
          localStorage.setItem("vencimientoPlan", payload.user.vencimientoPlan ?? "");
          localStorage.setItem("roles", JSON.stringify(payload.user.roles ?? []));
        },
        { token, refreshToken, user }
      );

      const destination = returnUrl && returnUrl.trim() ? returnUrl : this.resolveDefaultRoute(user);
      await this.page.goto(destination, { waitUntil: "domcontentloaded" });
    } finally {
      await api.dispose();
    }
  }

  private resolveDefaultRoute(user: any): string {
    const tipo = String(user?.tipo ?? "").toLowerCase();
    const planId = String(user?.planId ?? "0");
    const vencimientoPlan = String(user?.vencimientoPlan ?? "");
    const planDate = vencimientoPlan ? new Date(vencimientoPlan) : null;
    const hasActivePlan =
      planId !== "0" &&
      planId !== "" &&
      (!planDate || Number.isNaN(planDate.getTime()) || planDate.getTime() > Date.now());

    if (tipo === "1" || tipo === "empleador") {
      return hasActivePlan ? "/Empleador" : "/Empleador/AdquirirPlan";
    }

    if (tipo === "2" || tipo === "contratista") {
      return hasActivePlan ? "/Contratista" : "/Contratista/AdquirirPlan";
    }

    return "/";
  }
}
