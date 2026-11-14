# ServicePulse AI System

**Full-stack customer service quality inspection platform** — ASP.NET Core 10 Clean Architecture + React 19 + SQL Server + JWT Auth + Python AI microservice with Gemini LLM fallback.

---

## 🎯 What It Does

ServicePulse AI analyzes customer service call transcripts to:
- Calculate quality scores (0–100) using rule-based heuristics
- Detect sentiment and risk levels automatically
- Flag calls requiring manager review
- Enable managers to add coaching notes and override scores
- Provide role-based dashboards for agents, managers, and admins

---

## 🏗️ Architecture

| Layer | Technology | Responsibility |
|---|---|---|
| **Frontend** | React 19 + Reactstrap 9 + Vite | SPA with protected routes, JWT auth, call search/filter |
| **Backend API** | ASP.NET Core 10 | REST endpoints, JWT auth, Clean Architecture |
| **Database** | SQL Server 2022 (Docker or LocalDB) | Persistent storage with EF Core 10 |
| **AI Service** | Python FastAPI | Rule-based analyzer + Gemini LLM fallback |
| **Background Jobs** | Hangfire | Async transcript analysis queue |
| **Auth** | JWT Bearer + BCrypt | 1-hour token expiry, role-based claims |

### System Diagram

```
[React SPA] → [ASP.NET Core API] ↔ [SQL Server]
                    ↓
            [Python FastAPI] → [Gemini API (optional)]
                    ↑
            [Hangfire Jobs]
```

---

## 🚀 Quick Start (Docker)

```bash
# Clone and enter directory
cd ServicePulse_AI_System

# Copy environment template
cp .env.example .env

# Start all services (database, AI service, API)
docker-compose up -d

# Check logs
docker-compose logs -f api

# Health check
curl http://localhost:8080/api/health
```

**Access the app:** http://localhost:8080

---

## 🔐 Default Credentials

| Email | Password | Role | Access |
|---|---|---|---|
| `admin@servicepulse.local` | `Admin@123` | Admin | All calls, all features |
| `priya.sharma@servicepulse.local` | `Priya@123` | Manager | All calls, can review/coach |
| `alice.johnson@servicepulse.local` | `Alice@123` | Agent | Own calls only |

---

## 📡 API Endpoints

### Authentication
| Endpoint | Method | Auth | Description |
|---|---|---|---|
| `/api/auth/register` | POST | No | Register new user |
| `/api/auth/login` | POST | No | Login, get JWT token |
| `/api/auth/me` | GET | Yes | Get current user |

### Calls
| Endpoint | Method | Auth | Description |
|---|---|---|---|
| `/api/calls` | GET | Yes | Paginated list (role-filtered) |
| `/api/calls/{id}` | GET | Yes | Call detail with analysis & review |
| `/api/calls/search` | GET | Yes | Filter by agent, customer, status, date |
| `/api/calls/stats` | GET | Yes | Dashboard statistics |
| `/api/calls` | POST | Yes | Create new call |

### Manager Reviews
| Endpoint | Method | Auth | Description |
|---|---|---|---|
| `/api/manager-reviews/{callId}` | GET | Manager/Admin | Get review for call |
| `/api/manager-reviews` | POST | Manager/Admin | Create/edit review |

### Utility
| Endpoint | Method | Auth | Description |
|---|---|---|---|
| `/api/health` | GET | No | Health + DB connectivity check |
| `/api/lookup/agents` | GET | Yes | Agent dropdown data |
| `/api/lookup/customers` | GET | Yes | Customer dropdown data |

---

## 🧪 Running Tests

```bash
# Run all tests (23 passing)
cd backend
dotnet test src/ServicePulse.Tests/ServicePulse.Tests.csproj
```

**Test Coverage:**
- 17 unit tests (controllers, auth, validation)
- 6 integration tests (provider tracking, fallback audit, role validation, review integrity)

---

## 📊 Quality Score Formula

```
Base Score:        50 points
+ Greeting:        10 points (detects "hello", "hi", "good morning/afternoon/evening")
+ Resolution:      20 points (detects "resolved", "fixed", "all set", "taken care of")
+ Apology:         10 points (detects "sorry", "apologize", "regret")
+ Next Step:       15 points (detects "will", "going to", "follow up", "next steps")
- Refund:          10 points (detects "refund", "chargeback", "money back")
- Frustrated:      15 points (detects "frustrated", "disappointed", "unacceptable")
- Escalation:      20 points (detects "supervisor", "manager", "escalate", "speak to someone")

Final Score: Clamped to 0–100
```

---

## 🐳 Docker Configuration

### Services

| Service | Image | Port | Purpose |
|---|---|---|---|
| `db` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 | SQL Server Developer Edition |
| `ai-service` | Custom Python 3.12 slim | 8000 | FastAPI transcript analyzer |
| `api` | Custom .NET 10 | 8080 | Main API + React SPA |

### Environment Variables

```bash
# .env file
JWT_SECRET=SuperSecretKeyValue_ForDevOnly_ChangeInProd_2026!
GEMINI_API_KEY=  # Leave empty for rule-based fallback only
```

---

## 📁 Project Structure

```
ServicePulse_AI_System/
├── backend/
│   ├── src/
│   │   ├── ServicePulse.Domain/        # Entities, enums, domain rules
│   │   ├── ServicePulse.Application/   # DTOs, commands, queries
│   │   ├── ServicePulse.Infrastructure/# EF Core, Hangfire, AI client
│   │   └── ServicePulse.Api/           # Controllers, middleware, Program.cs
│   └── tests/
│       └── ServicePulse.Tests/         # xUnit tests (23 passing)
├── ai-service/
│   └── app/
│       ├── main.py                     # FastAPI entry point
│       ├── analyzers/
│       │   ├── rule_based.py           # Heuristic quality scorer
│       │   └── gemini_analyzer.py      # LLM fallback
│       └── requirements.txt
├── frontend/ (Vite build output in backend/wwwroot)
├── docker-compose.yml
├── Dockerfile
├── ai-service/Dockerfile
├── .env.example
└── docs/
    ├── architecture.md
    ├── v1-release-notes.md
    └── v2-release-notes.md
```

---

## 🔍 Key Features

### v1.0 — Core Platform
- Calls CRUD with pagination and search
- Dashboard statistics
- Health check endpoint
- Global exception handling
- Input validation

### v1.1 — Background Analysis
- Hangfire job queue for async processing
- Python FastAPI microservice
- Rule-based quality scoring
- Fake AI analyzer for offline testing

### v1.2 — Manager Review
- Manager review workflow
- Coaching notes and score overrides
- Audit logging for reviews
- Frontend review form and panel

### v1.3 — Hosted AI Coach
- Provider tracking (Gemini / RuleBased)
- Fallback reason audit logging
- HTTP timeout with structured error handling
- Correlation ID tracking

### v1.4 — Security Polish
- Role-based call visibility
- Agent users see only their own calls
- Manager/Admin users see all calls
- Health endpoint with DB probe

### v2.0 — Docker Deployment
- Full Docker Compose stack
- Production-ready Vite build
- 15 fictional transcripts seeded
- Portfolio presentation ready

---

## 📝 License

MIT — Built as a portfolio project demonstrating full-stack .NET + React + AI integration.

---

## 🤖 Generated with [Claude Code](https://claude.com/claude-code)
