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
    extraHTTPHeaders: {
      Accept: "application/json"
    }
  });
}

export async function apiCall(ctx: APIRequestContext, path: string, opts: ApiCallOptions = {}) {
  const method = opts.method ?? "GET";
  const headers: Record<string, string> = { ...(opts.headers ?? {}) };

  if (opts.token) {
    headers.Authorization = `Bearer ${opts.token}`;
  }

  const response = await ctx.fetch(joinUrl(env.apiBaseUrl, path), {
    method,
    headers,
    data: opts.body
  });

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
