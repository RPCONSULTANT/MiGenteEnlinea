export type Role = "empleador" | "contratista" | "admin";

function getEnv(name: string, fallback?: string): string {
  const value = process.env[name] ?? fallback;
  if (!value || !value.trim()) {
    throw new Error(`Missing required environment variable: ${name}`);
  }
  return value.trim();
}

export const env = {
  webBaseUrl: getEnv("E2E_WEB_BASE_URL", "http://plattaformv2.migenteenlinea.do"),
  apiBaseUrl: getEnv("E2E_API_BASE_URL", "http://api2.migenteenlinea.do"),
  allowWrite: (process.env.E2E_ALLOW_WRITE ?? "false").toLowerCase() === "true",
  runId: process.env.E2E_RUN_ID ?? `run_${Date.now()}`,
  seedKey: process.env.E2E_SEED_KEY
};

export function getRoleCredentials(role: Role): { email: string; password: string } {
  const map: Record<Role, { email: string; password: string }> = {
    empleador: {
      email: getEnv("E2E_USER_EMPLEADOR_EMAIL"),
      password: getEnv("E2E_USER_EMPLEADOR_PASSWORD")
    },
    contratista: {
      email: getEnv("E2E_USER_CONTRATISTA_EMAIL"),
      password: getEnv("E2E_USER_CONTRATISTA_PASSWORD")
    },
    admin: {
      email: getEnv("E2E_USER_ADMIN_EMAIL"),
      password: getEnv("E2E_USER_ADMIN_PASSWORD")
    }
  };

  return map[role];
}
