import fs from "node:fs";
import path from "node:path";
import { Page, TestInfo } from "@playwright/test";

export type RuntimeIssue = {
  type: "console-error" | "console-warn" | "page-error" | "request-failed" | "http-error";
  message: string;
  url?: string;
};

export function attachRuntimeMonitors(page: Page, issues: RuntimeIssue[]): void {
  page.on("console", (msg) => {
    if (msg.type() === "error") {
      issues.push({ type: "console-error", message: msg.text(), url: page.url() });
      return;
    }

    if (msg.type() === "warning") {
      issues.push({ type: "console-warn", message: msg.text(), url: page.url() });
    }
  });

  page.on("pageerror", (err) => {
    issues.push({ type: "page-error", message: String(err), url: page.url() });
  });

  page.on("requestfailed", (req) => {
    const failure = req.failure();
    issues.push({
      type: "request-failed",
      message: `${req.method()} ${req.url()} => ${failure?.errorText ?? "unknown"}`,
      url: page.url()
    });
  });

  page.on("response", (res) => {
    if (res.status() >= 400) {
      const req = res.request();
      if (req.resourceType() !== "image" && req.resourceType() !== "font") {
        issues.push({
          type: "http-error",
          message: `${req.method()} ${res.url()} => ${res.status()}`,
          url: page.url()
        });
      }
    }
  });
}

export async function persistIssues(testInfo: TestInfo, issues: RuntimeIssue[]): Promise<void> {
  const dir = path.join("artifacts", "e2e");
  fs.mkdirSync(dir, { recursive: true });

  const fileName = testInfo.titlePath.join("__").replace(/[^a-zA-Z0-9_-]/g, "_");
  const payload = {
    test: testInfo.title,
    file: testInfo.file,
    status: testInfo.status,
    issues
  };

  fs.writeFileSync(path.join(dir, `${fileName}.json`), JSON.stringify(payload, null, 2));
}

export function assertNoCriticalIssues(issues: RuntimeIssue[], allowPatterns: RegExp[] = []): void {
  const critical = issues.filter((issue) => !allowPatterns.some((pattern) => pattern.test(issue.message)));
  if (critical.length > 0) {
    throw new Error(`Critical runtime issues detected: ${JSON.stringify(critical, null, 2)}`);
  }
}
