# Changelog

All notable changes to ServicePulse AI System will be documented in this file.

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [v2.0.0] — 2026-11-13 — Authentication & Authorization
### Added
- JWT Bearer authentication with `Microsoft.AspNetCore.Authentication.JwtBearer`
- `POST /api/auth/register` — create a new user account (Admin / Manager / Agent roles)
- `POST /api/auth/login` — authenticate and receive a JWT token (1-hour expiry)
- `GET /api/auth/me` — return currently authenticated user profile from token claims
- BCrypt.Net-Next v4 password hashing for all seeded and registered users
- Role-based authorization: `[Authorize]` applied to CallsController and LookupController
- Admin seed user auto-created on first startup via `SeedAdminIfNeeded`
- Health endpoint remains publicly accessible (`[AllowAnonymous]`)
- 9 new AuthService unit tests (register success, duplicate email, invalid role, login success, wrong password, inactive account, admin seeding, case-insensitive email normalization)
- Total test coverage: 18 passing unit tests

### Changed
- `AuthService` registered as scoped (was incorrectly singleton)
- All password hashes updated from placeholder strings to real BCrypt hashes in DataSeeder

### Default Credentials
| Email | Password | Role |
|---|---|---|
| admin@servicepulse.local | Admin@123 | Admin |
| priya.sharma@servicepulse.local | Priya@123 | Manager |
| alice.johnson@servicepulse.local | Alice@123 | Agent |

### Technical Stack Updates
- **Auth:** Microsoft.AspNetCore.Authentication.JwtBearer v6, BCrypt.Net-Next v4.2.0
- **JWT Config:** Issuer = "ServicePulse.Api", Audience = "ServicePulse.Client"
- **Token lifetime:** 1 hour, configurable via `JWT_SECRET` environment variable

---

## [v1.0.0] — 2026-11-13 — Initial Release

### Added
- Full-stack .NET 10 + React application with Clean Architecture
- Calls CRUD API (GetCalls, GetCall, SearchCalls, GetStats, CreateCall)
- Health check endpoint with database connectivity probe
- Lookup endpoints for agents and customers
- Input validation with range, required, and length checks
- Global exception handling middleware returning consistent JSON errors
- Dashboard statistics endpoint
- Frontend calls list with pagination component
- Search/filter bar (agent name, customer name, status, date range)
- Call detail page showing transcript, AI analysis results, manager review
- 9 unit tests (all passing)
- SQLite database with automatic seeding on startup
- Postman-ready API with verified endpoints
- Complete runbook (setup, testing, troubleshooting, deployment)
- v1 release notes and architecture documentation

### Technical Stack
- **Backend:** ASP.NET Core 10, Entity Framework Core 10, SQLite
- **Frontend:** React 18, Reactstrap, Bootstrap 5
- **Testing:** xUnit, EF Core in-memory SQLite pattern
- **Architecture:** Clean Architecture (Domain / Application / Infrastructure / API)

---

*Future versions will document new features, breaking changes, and migrations here.*
