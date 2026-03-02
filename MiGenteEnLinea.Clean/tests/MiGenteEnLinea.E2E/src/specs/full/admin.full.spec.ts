import { test, expect } from "../../fixtures/test-fixtures";
import { loginByRole } from "../../helpers/auth";
import { apiCall } from "../../helpers/api-client";
import { env } from "../../config/env";
import { requireWriteAccess } from "../../config/env";

test.describe("@full @admin Admin database controls", () => {
  test("@full @admin repair plans endpoint works when enabled", async ({ api }) => {
    requireWriteAccess("admin-repair-plans");
    expect(env.seedKey, "E2E_SEED_KEY es obligatorio para escenarios admin").toBeTruthy();

    const adminToken = await loginByRole(api, "admin");
    const result = await apiCall(api, "/api/admin/database/repair-plans", {
      method: "POST",
      token: adminToken,
      headers: {
        "Content-Type": "application/json",
        "X-Seed-Key": env.seedKey!
      }
    });

    expect([200, 204, 403], `Admin repair-plans devolvió estado inesperado: ${result.status} - ${result.text}`).toContain(result.status);
  });
});
