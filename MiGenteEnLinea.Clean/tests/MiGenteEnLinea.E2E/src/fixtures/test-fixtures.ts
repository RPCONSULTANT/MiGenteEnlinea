import { test as base } from "@playwright/test";
import { createApiContext } from "../helpers/api-client";
import { RollbackManager } from "../helpers/rollback";
import { RuntimeIssue, attachRuntimeMonitors } from "../helpers/error-monitor";

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

  runtimeIssues: async ({ page }, use) => {
    const issues: RuntimeIssue[] = [];
    attachRuntimeMonitors(page, issues);
    await use(issues);
  }
});

export { expect } from "@playwright/test";
