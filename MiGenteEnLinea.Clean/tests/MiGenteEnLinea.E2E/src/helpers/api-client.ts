import { APIRequestContext, request } from "@playwright/test";
import { env } from "../config/env";
import { joinUrl } from "./url";

export type ApiCallOptions = {
  method?: "GET" | "POST" | "PUT" | "DELETE" | "OPTIONS";
  token?: string;
  headers?: Record<string, string>;
  body?: unknown;
};

export async function createApiContext(): Promise<APIRequestContext> {
  return request.newContext({
    baseURL: env.apiBaseUrl,
    timeout: 45000,
    extraHTTPHeaders: {
      Accept: "application/json"
    }
  });
}

export async function apiCall(ctx: APIRequestContext, path: string, opts: ApiCallOptions = {}) {
  const method = opts.method ?? "GET";
  const headers: Record<string, string> = { ...(opts.headers ?? {}) };
  const maxAttempts = Number.parseInt(process.env.E2E_API_RETRY_ATTEMPTS ?? "3", 10);
  const attempts = Number.isFinite(maxAttempts) && maxAttempts > 0 ? maxAttempts : 3;

  if (opts.token) {
    headers.Authorization = `Bearer ${opts.token}`;
  }

  let response;
  let lastError: unknown;

  for (let attempt = 1; attempt <= attempts; attempt += 1) {
    try {
      response = await ctx.fetch(joinUrl(env.apiBaseUrl, path), {
        method,
        headers,
        data: opts.body
      });

      if (response.status() >= 500 && attempt < attempts) {
        await new Promise((r) => setTimeout(r, attempt * 350));
        continue;
      }

      break;
    } catch (error) {
      lastError = error;
      if (attempt >= attempts) {
        throw error;
      }
      await new Promise((r) => setTimeout(r, attempt * 500));
    }
  }

  if (!response) {
    throw lastError ?? new Error(`No response returned for ${method} ${path}`);
  }

  const contentType = response.headers()["content-type"] ?? "";
  const text = await response.text();
  let json: unknown = null;

  if (contentType.includes("application/json") && text) {
    try {
      json = JSON.parse(text);
    } catch {
      json = null;
    }
  }

  return {
    response,
    status: response.status(),
    ok: response.ok(),
    text,
    json,
    headers: response.headers()
  };
}
