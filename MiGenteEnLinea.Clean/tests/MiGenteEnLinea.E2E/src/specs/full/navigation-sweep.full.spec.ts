import { test, expect } from "../../fixtures/test-fixtures";
import { AuthPage } from "../../pages/AuthPage";
import { getRoleCredentials } from "../../config/env";

type RoleConfig = {
  role: "empleador" | "contratista";
  rootPath: string;
  requiredRoutes: string[];
};

const roleConfigs: RoleConfig[] = [
  {
    role: "empleador",
    rootPath: "/Empleador",
    requiredRoutes: [
      "/Empleador/Index",
      "/Empleador/Empleados",
      "/Empleador/Contrataciones",
      "/Empleador/AdquirirPlan",
      "/Empleador/Calificaciones"
    ]
  },
  {
    role: "contratista",
    rootPath: "/Contratista",
    requiredRoutes: [
      "/Contratista/Index",
      "/Contratista/Perfil",
      "/Contratista/Suscripciones",
      "/Contratista/Directorio",
      "/Contratista/AdquirirPlan"
    ]
  }
];

function normalizeRoute(pathname: string): string {
  if (!pathname.startsWith("/")) return `/${pathname}`;
  return pathname;
}

test.describe("@full @navigation Navigation sweep by role", () => {
  for (const config of roleConfigs) {
    test(`@full @navigation ${config.role} navigates all known views`, async ({ page, runtimeIssues }) => {
      const authPage = new AuthPage(page);
      const creds = getRoleCredentials(config.role);

      await authPage.openLogin();
      await authPage.login(creds.email, creds.password);
      await page.waitForLoadState("domcontentloaded");

      const discoveredRoutes = await page.evaluate((rootPath) => {
        const routes = new Set<string>();
        for (const anchor of Array.from(document.querySelectorAll("a[href]"))) {
          const href = anchor.getAttribute("href");
          if (!href || href.startsWith("#") || href.startsWith("javascript:")) {
            continue;
          }

          try {
            const url = new URL(href, window.location.origin);
            if (!url.pathname.toLowerCase().startsWith(rootPath.toLowerCase())) {
              continue;
            }
            routes.add(url.pathname);
          } catch {
            // ignore malformed links
          }
        }
        return Array.from(routes.values());
      }, config.rootPath);

      const routeSet = new Set<string>([
        ...config.requiredRoutes.map(normalizeRoute),
        ...discoveredRoutes.map(normalizeRoute)
      ]);

      const orderedRoutes = Array.from(routeSet.values()).sort((a, b) => a.localeCompare(b));
      const failedRoutes: string[] = [];

      for (const route of orderedRoutes) {
        const response = await page.goto(route, { waitUntil: "domcontentloaded" });
        const status = response?.status() ?? 0;

        await page.waitForTimeout(300);

        const finalPath = new URL(page.url()).pathname;
        const hasMain = await page.locator("main, h1, .container, .container-fluid").first().isVisible().catch(() => false);

        if (status >= 400 || !hasMain || !finalPath.toLowerCase().startsWith(config.rootPath.toLowerCase())) {
          failedRoutes.push(`${route} -> status=${status}, final=${finalPath}, hasMain=${hasMain}`);
          runtimeIssues.push({
            type: "http-error",
            message: `navigation-sweep route failed: ${route} -> status=${status}, final=${finalPath}, hasMain=${hasMain}`,
            url: page.url()
          });
        }
      }

      expect(failedRoutes, `Navigation sweep failed for ${config.role}: ${failedRoutes.join(" | ")}`).toHaveLength(0);
    });
  }
});
