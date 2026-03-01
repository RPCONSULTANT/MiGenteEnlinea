import { test, expect } from "../../fixtures/test-fixtures";
import { loginByRole } from "../../helpers/auth";
import { apiCall } from "../../helpers/api-client";
import { env } from "../../config/env";

test.describe("@full @admin Admin database controls", () => {
  test("@full @admin repair plans endpoint works when enabled", async ({ api }) => {
    test.skip(!env.allowWrite, "E2E_ALLOW_WRITE=false, skipping full write scenario");
    test.skip(!env.seedKey, "E2E_SEED_KEY missing, skipping admin seeding controls");

    const adminToken = await loginByRole(api, "admin");
    const result = await apiCall(api, "/api/admin/database/repair-plans", {
      method: "POST",
      token: adminToken,
      headers: {
        "Content-Type": "application/json",
        "X-Seed-Key": env.seedKey!
      }
    });

    expect([200, 204]).toContain(result.status);
  });
});
