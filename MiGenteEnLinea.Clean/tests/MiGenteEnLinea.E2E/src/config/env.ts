export type Role = "empleador" | "contratista" | "admin";

function getEnv(name: string, fallback?: string): string {
  const value = process.env[name] ?? fallback;
  if (!value || !value.trim()) {
    throw new Error(`Missing required environment variable: ${name}`);
  }
  return value.trim();
}

function getEnvAny(names: string[]): string {
  for (const name of names) {
    const value = process.env[name];
    if (value && value.trim()) {
      return value.trim();
    }
  }

  throw new Error(`Missing required environment variable. Tried: ${names.join(", ")}`);
}

export const env = {
  webBaseUrl: getEnv("E2E_WEB_BASE_URL", "http://plattaformv2.migenteenlinea.do"),
  apiBaseUrl: getEnv("E2E_API_BASE_URL", "http://api2.migenteenlinea.do"),
  allowWrite: (process.env.E2E_ALLOW_WRITE ?? "false").toLowerCase() === "true",
  strictRuntimeIssues: (process.env.E2E_STRICT_RUNTIME_ISSUES ?? "true").toLowerCase() === "true",
  runId: process.env.E2E_RUN_ID ?? `run_${Date.now()}`,
  seedKey: process.env.E2E_SEED_KEY
};

export function requireWriteAccess(context: string): void {
  if (!env.allowWrite) {
    throw new Error(`[E2E][${context}] E2E_ALLOW_WRITE debe ser true para este escenario funcional.`);
  }
}

export function getRoleCredentials(role: Role): { email: string; password: string } {
  const map: Record<Role, { email: string; password: string }> = {
    empleador: {
      email: getEnvAny(["E2E_USER_EMPLEADOR_EMAIL", "E2E_EMAIL_EMPLEADOR"]),
      password: getEnvAny(["E2E_USER_EMPLEADOR_PASSWORD", "E2E_PASSWORD_EMPLEADOR"])
    },
    contratista: {
      email: getEnvAny(["E2E_USER_CONTRATISTA_EMAIL", "E2E_EMAIL_CONTRATISTA"]),
      password: getEnvAny(["E2E_USER_CONTRATISTA_PASSWORD", "E2E_PASSWORD_CONTRATISTA"])
    },
    admin: {
      email: getEnvAny(["E2E_USER_ADMIN_EMAIL", "E2E_EMAIL_ADMIN"]),
      password: getEnvAny(["E2E_USER_ADMIN_PASSWORD", "E2E_PASSWORD_ADMIN"])
    }
  };

  return map[role];
}
