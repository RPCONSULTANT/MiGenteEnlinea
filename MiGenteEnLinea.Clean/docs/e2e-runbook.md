# E2E Runbook

## Local run
1. `cd MiGenteEnLinea.Clean/tests/MiGenteEnLinea.E2E`
2. Set environment variables for credentials and endpoints.
3. `npm install`
4. `npx playwright install chromium`
5. Smoke: `npm run test:e2e:smoke`
6. Full: `npm run test:e2e:full`
7. Summary: `npm run report:summary`

## CI policy
- PR: run `@smoke` with `E2E_ALLOW_WRITE=false`
- Nightly: run `@full` with `E2E_ALLOW_WRITE=true`

## Failure troubleshooting
- Review `playwright-report/` and `test-results/`.
- Review machine report: `artifacts/e2e/summary.json`.
- Review per-test issue files under `artifacts/e2e/*.json`.
- For auth failures, validate E2E role credentials.
- For CORS failures, validate API origin allow-list and host bindings.
- For 404 route mismatches, cross-check `docs/e2e-endpoint-matrix.md`.
- For rollback failures, inspect API delete endpoint logs and rerun cleanup manually.
