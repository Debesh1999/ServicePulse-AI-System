# ServicePulse AI System — v1 Release Notes

## Version: v1.0.0
**Date:** 2026-11-13
**Status:** Initial Public Release

---

## Overview

v1 introduces the core call monitoring and quality management system with full-stack architecture: Clean Architecture backend (.NET 10) + React frontend.

---

## Features Delivered

### Backend API (6 endpoints)

| Endpoint | Method | Description |
|---|---|---|
| `/api/calls` | GET | Paginated list of all calls |
| `/api/calls/{id}` | GET | Full call detail with transcript, analysis, review |
| `/api/calls/search` | GET | Filtered search by agent, customer, status, date range |
| `/api/calls/stats` | GET | Dashboard statistics (counts, averages, breakdowns) |
| `/api/calls` | POST | Create new call from transcript text |
| `/api/health` | GET | Health check with database connectivity probe |
| `/api/lookup/agents` | GET | Agent lookup for dropdown selects |
| `/api/lookup/customers` | GET | Customer lookup for dropdown selects |

### Input Validation & Error Handling

- Required field validation on create endpoints
- Range validation for duration (1–7200 seconds)
- MaxLength validation for string fields
- Global exception handling middleware returning consistent JSON errors
- Graceful 400 / 404 / 500 response codes

### Frontend

- Dashboard with stat cards
- Calls list with pagination component (`Pager.js`)
- Search and filter bar (agent name, customer name, status, date range)
- Call detail page showing transcript, analysis results, and manager review
- Responsive layout using Reactstrap

### Testing

- 9 unit tests covering all controller actions (GetCalls, GetCall, SearchCalls, GetStats, CreateCall)
- SQLite in-memory pattern with proper isolation per test
- All tests pass consistently

### Infrastructure

- Entity Framework Core 10 Code-First migrations
- SQLite database with shared cache mode
- Automatic data seeding on development startup
- Connection string configurable via appsettings.json

---

## Architecture Layers

```
Domain     → Entities (Agent, Call, AnalysisResult, ManagerReview, etc.)
Application → DTOs and request models
Infrastructure → EF Core DbContext, Migrations, SeedData
Api        → Controllers, Middleware, Program.cs
Frontend   → React components, pages, services
Tests      → xUnit controller unit tests
```

---

## Known Limitations (to be addressed in future versions)

- No authentication or authorization yet
- No real AI/ML analysis pipeline integrated
- No WebSocket support for real-time updates
- SQLite used instead of production-grade RDBMS
- No API rate limiting or caching layer
- Frontend not built behind the API (separate ports: 3000 and 5295)

---

## How to Run

See [RUNBOOK.md](../RUNBOOK.md) for detailed setup and testing instructions.

---

## Git History

- `ea36627` — V1 core implementation commit
- `c1afc68` — Add runbook and project documentation
- `b75f513` — Rewrite runbook entirely in English
