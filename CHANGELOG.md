# Changelog

All notable changes to ServicePulse AI System will be documented in this file.

Format follows [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [v2.0.0] — 2026-11-14 — Docker Deployment & Project Completion

### Added
- Docker Compose configuration for full-stack deployment (`docker-compose.yml`)
- .NET API Dockerfile with multi-stage build and static SPA hosting
- Python AI service Dockerfile for standalone analyzer containerization
- `.env.example` with JWT secret and Gemini API key configuration
- `.dockerignore` for optimized build contexts
- 15 fictional call transcripts seeded into the database for demo purposes
- Role-based call visibility: agents see only their calls; managers/admins see all

### Changed
- Frontend rebuilt with Vite and production assets served from `wwwroot/`
- CallsController updated with AuthService integration for role-based filtering
- Integration tests expanded from 4 to 6 covering provider tracking, fallback behavior, audit correlation, and review integrity
- Total test coverage: 23 passing tests (17 unit + 6 integration)

### Infrastructure
- **Backend:** ASP.NET Core 10, Entity Framework Core 10, SQL Server LocalDB / Docker
- **Frontend:** React 19 + Reactstrap 9, Vite 8 build tooling
- **AI Service:** Python FastAPI with rule-based engine + Gemini LLM fallback
- **Auth:** JWT Bearer tokens with BCrypt password hashing
- **Background Jobs:** Hangfire with SQL Server storage and automatic retry

### Default Credentials
| Email | Password | Role |
|---|---|---|
| admin@servicepulse.local | Admin@123 | Admin |
| priya.sharma@servicepulse.local | Priya@123 | Manager |
| alice.johnson@servicepulse.local | Alice@123 | Agent |

### Running with Docker
```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Health check
curl http://localhost:8080/api/health
```

---

## [v1.4.0] — 2026-11-14 — Security Polish

### Added
- Role-based call visibility filtering in `CallsController`
- `AuthService` integration for user identity resolution in controllers
- Integration tests for manager review data integrity and role validation
- Health endpoint with database connectivity probe (`GET /api/health`)

### Changed
- All authenticated endpoints now enforce `[Authorize]` attribute consistently
- CallsController constructor accepts optional `AuthService` for backward-compatible testing

---

## [v1.3.0] — 2026-11-14 — Hosted AI Quality Coach

### Added
- `Provider` field (required, max 30 chars, default "RuleBased") to `AnalysisResult` entity
- `FallbackReason` field (optional, max 1500 chars) to `AnalysisResult` entity
- HTTP timeout (30 seconds) with `CancellationTokenSource` in `PythonAiClient`
- Structured error handling: Timeout, ConnectionError, ParseError paths
- Audit log entries with correlation IDs on AI analysis success/failure
- `GetCalls()` and `SearchCalls()` pass through `Provider` and `FallbackReason` DTOs
- EF migration `AddProviderAndFallbackToAnalysisResult`
- 4 integration tests verifying Provider/FallbackReason persistence and defaults

### Changed
- `IAiAnalyzerService.AnalyzeAsync` returns `AiAnalysisResult` with `Provider` and `FallbackReason`
- `FakeAiAnalyzer` updated for new signature
- Python AI service supports `fallback_reason` in responses

---

## [v1.2.0] — 2026-11-13 — Manager Review Workflow

### Added
- `ManagerReview` entity with review status, manager score override, coaching notes, override reason
- `ManagerReviewsController` (protected: Manager/Admin roles only)
- `IManagerReviewService` application service layer
- Frontend `ReviewForm` component for creating/editing manager reviews
- Frontend `ReviewPanel` component displaying review details in call detail page
- Global exception handling middleware (`ExceptionHandlingMiddleware`)
- Audit logging for all manager review create/update operations
- 6 new controller tests for review workflow (+ 9 auth tests)

---

## [v1.1.0] — 2026-11-13 — Background Analysis Pipeline

### Added
- Python FastAPI microservice (`ai-service/app/`) with rule-based transcript analyzer
- Quality score formula implementation: Base 50 + greeting(10) + resolution(20) + apology/next_step(15) - refund(10) - frustrated(15) - escalation(20), clamped 0–100
- Hangfire background job queue (`CallAnalysisJob`) for async transcript processing
- `IAiAnalyzerService` interface and `PythonAiClient` infrastructure implementation
- Calls return "Queued" status immediately; analysis completes asynchronously
- Fake AI analyzer for offline testing (`FakeAiAnalyzer`)

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

### Technical Stack
- **Backend:** ASP.NET Core 10, Entity Framework Core 10, SQL Server LocalDB
- **Frontend:** React 18, Reactstrap, Bootstrap 5
- **Testing:** xUnit, EF Core in-memory SQLite pattern
- **Architecture:** Clean Architecture (Domain / Application / Infrastructure / API)

---

*Future versions will document new features, breaking changes, and migrations here.*
