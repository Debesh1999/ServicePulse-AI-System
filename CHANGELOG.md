# Changelog

All notable changes to ServicePulse AI System will be documented in this file.

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

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
