# Database Seeding API

## Purpose
Run catalog/demo seeding directly from the API in a controlled and auditable way.

## Endpoints
- `POST /api/admin/database/seed-catalogs`
- `POST /api/admin/database/repair-plans`
- `POST /api/admin/database/seed-demo`
- `POST /api/admin/database/migrate-and-seed-catalogs`

## Security requirements
- `Authorization: Bearer <token>`
- User must satisfy `[Authorize(Roles="Admin")]`
- Security header required by config (default):
  - `X-Seed-Key: <configured key>`

## Configuration
Section: `DatabaseSeedingSecurity`

```json
{
  "DatabaseSeedingSecurity": {
    "Enabled": true,
    "RequireHeaderKey": true,
    "HeaderName": "X-Seed-Key",
    "HeaderValue": "CHANGE_ME_PRODUCTION_SEED_KEY",
    "AllowDemoSeedInProduction": false
  }
}
```

## Sample call
```bash
curl -X POST "https://api.example.com/api/admin/database/seed-catalogs" \
  -H "Authorization: Bearer <admin-jwt>" \
  -H "X-Seed-Key: <seed-key>"
```

## Expected response
- HTTP `200`: seeding completed with execution report.
- HTTP `409`: another seeding run is in progress.
- HTTP `401/403`: unauthorized or invalid seed header.

## Operational notes
- Keep startup seed flags disabled in production.
- Use `repair-plans` when checkout fails due to missing/inactive plans.
- `scripts/seed-catalogs.sql` remains emergency fallback only.
