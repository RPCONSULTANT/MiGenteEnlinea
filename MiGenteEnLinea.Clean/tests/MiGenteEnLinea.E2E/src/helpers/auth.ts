import { APIRequestContext } from "@playwright/test";
import { apiCall } from "./api-client";
import { Role, getRoleCredentials } from "../config/env";

const tokenCache = new Map<Role, string>();

export async function loginByRole(api: APIRequestContext, role: Role): Promise<string> {
  if (tokenCache.has(role)) {
    return tokenCache.get(role)!;
  }

  const credentials = getRoleCredentials(role);
  const result = await apiCall(api, "/api/auth/login", {
    method: "POST",
    body: credentials,
    headers: {
      "Content-Type": "application/json"
    }
  });

  if (!result.ok) {
    throw new Error(`Login failed for role ${role}. Status ${result.status}. Body: ${result.text}`);
  }

  const token =
    (result.json as any)?.accessToken ??
    (result.json as any)?.token ??
    (result.json as any)?.data?.accessToken;

  if (!token) {
    throw new Error(`Login response did not include access token for role ${role}`);
  }

  tokenCache.set(role, token);
  return token;
}

export function clearTokenCache(): void {
  tokenCache.clear();
}
