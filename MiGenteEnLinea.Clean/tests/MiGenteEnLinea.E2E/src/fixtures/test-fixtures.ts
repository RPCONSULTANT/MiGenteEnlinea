import { test as base } from "@playwright/test";
import { createApiContext } from "../helpers/api-client";
import { RollbackManager } from "../helpers/rollback";
import { RuntimeIssue, attachRuntimeMonitors, persistIssues } from "../helpers/error-monitor";
import { env } from "../config/env";

type Fixtures = {
  api: Awaited<ReturnType<typeof createApiContext>>;
  rollback: RollbackManager;
  runtimeIssues: RuntimeIssue[];
};

export const test = base.extend<Fixtures>({
  api: async ({}, use) => {
    const api = await createApiContext();
    await use(api);
    await api.dispose();
  },

  rollback: async ({}, use) => {
    const rollback = new RollbackManager();
    await use(rollback);
    await rollback.executeAll();
  },

  runtimeIssues: async ({ page }, use, testInfo) => {
    const issues: RuntimeIssue[] = [];
    attachRuntimeMonitors(page, issues);
    await use(issues);
    await persistIssues(testInfo, issues);

    if (issues.length > 0) {
      const issuePreview = issues
        .slice(0, 6)
        .map((i) => `[${i.type}] ${i.message}`)
        .join(" | ");
      console.log(`[E2E runtime issues] ${testInfo.title}: ${issues.length} issue(s). ${issuePreview}`);
    }

    if (env.strictRuntimeIssues && issues.length > 0) {
      throw new Error(
        `Strict runtime issues mode is enabled and detected ${issues.length} issue(s): ${issues
          .slice(0, 10)
          .map((i) => `[${i.type}] ${i.message}`)
          .join(" | ")}`
      );
    }
  }
});

export { expect } from "@playwright/test";
