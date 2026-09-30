# User Preferences

Display theme and notification settings, saved and retrieved per user.

## API

Base route: `/v1/users/{userId}/preferences` (nested resource; `userId` is a positive integer).

Until authentication exists, the caller supplies the user identifier. **Do not ship to production without replacing this with an authenticated identity.**

### GET /v1/users/{userId}/preferences

Returns the user's preferences wrapped in the standard envelope. A first-run user (nothing saved yet) receives **system defaults** with HTTP 200: theme `light`, notifications enabled.

### PUT /v1/users/{userId}/preferences

Creates or updates the user's preferences (upsert) and returns the persisted values in the envelope.

```json
{
  "theme": "dark",
  "notificationsEnabledIndicator": true
}
```

- `theme` — required, `light` or `dark` (case-insensitive; stored lowercase). Other values → HTTP 400 with code `ORG-VAL-001` and a `theme` field detail.
- `notificationsEnabledIndicator` — boolean flag; per-channel granularity (email/push) is a deferred follow-up.
- `userId <= 0` → HTTP 400 with code `ORG-VAL-001`.

### Response item shape

```json
{
  "item": {
    "userId": 1,
    "theme": "dark",
    "notificationsEnabledIndicator": true,
    "createdDate": "2026-09-30T12:00:00Z",
    "updatedDate": "2026-09-30T12:00:00Z"
  },
  "metadata": { "timestamp": "...", "transactionId": "...", "totalCount": null },
  "links": { "self": "...", "next": null, "prev": null }
}
```

## Backend structure

| Layer | File |
|-------|------|
| Entity | `backend/src/Api/Domain/UserPreference.cs` (includes `DisplayThemes` constants) |
| DbContext | `backend/src/Api/Data/AppDbContext.cs` (InMemory provider for now) |
| DTOs | `backend/src/Api/DTOs/Preferences/` |
| Repository | `backend/src/Api/Repositories/UserPreferencesRepository.cs` |
| Service | `backend/src/Api/Services/UserPreferencesService.cs` (owns defaults + mapping) |
| Controller | `backend/src/Api/Controllers/PreferencesController.cs` (owns envelope + validation) |

`CreatedDate` is preserved across updates; `UpdatedDate` is stamped on every save.

## Frontend

- Route: `/preferences` ([PreferencesPage.tsx](../frontend/src/routes/preferences/PreferencesPage.tsx)), linked in the app shell nav.
- Types in [types.ts](../frontend/src/lib/types.ts): `UserPreferences`, `UpdateUserPreferencesRequest`, `DisplayTheme`.
- The selected theme is applied by toggling the `dark` class on the document root; Tailwind is configured with `darkMode: 'class'`. Actual dark color styles are not implemented yet.

## Known follow-ups

1. **Authentication** — derive `userId` from the authenticated identity instead of the URL.
2. **Relational database + migration** — `AppDbContext` currently uses the InMemory provider; the `UserPreference` entity is migration-ready (PK on `UserId`, `Theme` max length 10).
3. **Notification granularity** — email/push channels if a precedent emerges.
4. **Upsert concurrency** — `UpsertAsync` is check-then-act; harden against duplicate-key races when moving to a relational store.
5. **Dark theme styles** — add `dark:` variants / CSS variable overrides to actually restyle the app.
