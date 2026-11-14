# ServicePulse AI System — v2 Release Notes

## Version: v2.0.0
**Date:** 2026-11-13
**Status:** Authentication & Authorization Complete

---

## Overview

v2 adds JWT-based authentication and role-based authorization to the v1 call monitoring platform. The React SPA now supports login, registration, protected routes, and a reverse proxy that forwards `/api/*` requests to the backend.

---

## Features Delivered

### Authentication & Authorization

| Feature | Description |
|---|---|
| JWT Bearer auth | `Microsoft.AspNetCore.Authentication.JwtBearer` with 1-hour token expiry |
| BCrypt password hashing | `BCrypt.Net-Next` v4.2.0 for all seeded and registered users |
| Registration | `POST /api/auth/register` — email, displayName, password, role |
| Login | `POST /api/auth/login` — returns JWT access token |
| Current user | `GET /api/auth/me` — returns user profile from token claims |
| Role-based auth | `[Authorize]` on `CallsController` and `LookupController` |
| Public endpoints | `[Controllers.HealthController]` and auth endpoints use `[Controllers.Anonymous]` |

### Backend Changes

- `AuthService` registered as **scoped** (was incorrectly singleton)
- All password hashes updated from placeholder strings to real BCrypt hashes in `DataSeeder`
- Admin seed user auto-created on first startup via `SeedAdminIfNeeded`
- 9 new `AuthService` unit tests (register success, duplicate email, invalid role, login success, wrong password, inactive account, admin seeding, case-insensitive email normalization)
- Total test coverage: **17 passing unit tests**

### Frontend Changes

- `AuthService.js` — localStorage token persistence (`servicepulse_token`, `servicepulse_user`)
- `LoginPage.js` — email/password form, post-auth redirect preserves `state.from`
- `RegisterPage.js` — email, displayName, password, role select
- `ProtectedRoute.js` — redirects unauthenticated users to `/login`
- `NavMenu.js` — functional component with user dropdown and logout
- `api.js` — axios interceptors inject Bearer token, 401 → redirect to `/login`
- `AppRoutes.js` — `/login` and `/register` unprotected; `/dashboard`, `/calls` wrapped in `ProtectedRoute`
- Reverse proxy in `Program.cs` handles `/api/*` → backend on `:5295`

### Infrastructure

- Custom reverse proxy replaces `Microsoft.AspNetCore.SpaProxy`
- `API_BASE_URL` env var configures the proxy target (default `http://localhost:5295`)
- React build output served from `wwwroot/` after `npm run build`
- `UseHttpsRedirection` removed — local dev runs over HTTP only

---

## Default Credentials (seeded on first run)

| Email | Password | Role |
|---|---|---|
| admin@servicepulse.local | Admin@123 | Admin |
| priya.sharma@servicepulse.local | Priya@123 | Manager |
| alice.johnson@servicepulse.local | Alice@123 | Agent |

---

## Known Limitations (to be addressed in future versions)

- No real AI/ML analysis pipeline integrated
- No WebSocket support for real-time updates
- SQLite used instead of production-grade RDBMS
- No API rate limiting or caching layer
- No Docker containerization

---

## How to Run

See [RUNBOOK.md](../RUNBOOK.md) for detailed setup and testing instructions.

---

## Git History

- `5e9520b` — V1 core implementation commit
- `d35dff9` — Add runbook and project documentation
- `ad108e4` — Rewrite runbook entirely in English
- `990922a` — Add v1 release notes and CHANGELOG for version tracking
- `fdaed66` — v2: Add JWT authentication, registration, login, and role-based authorization
- `4a810bc` — feat(frontend): integrate JWT auth into React SPA with reverse proxy
- `ee21198` — fix: resolve React Dropdown toggle error and translate docs to English