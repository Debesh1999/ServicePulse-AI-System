# ServicePulse AI System — Runbook

## 1. Environment Requirements

| Component | Version |
|---|---|
| .NET SDK | 10.0.x |
| Node.js | 18+ |
| Docker | 24+ (for containerized deployment) |
| Database | SQL Server 2022 (LocalDB for dev, Docker for prod) |

---

## 2. First-Time Setup

### 2.1 Docker Deployment (Recommended)
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

### 2.2 Local Development (Backend + Frontend)

**Backend API (SQL Server LocalDB):**
```bash
cd backend/src/ServicePulse.Api
dotnet run --no-launch-settings
# Access http://localhost:5295/api/health
```

**Frontend (Vite + React):**
```bash
cd backend/frontend/ServicePulse.Frontend.Vite
npm install
npm run dev
# Access http://localhost:5173 (proxies /api to backend)
```

### 2.3 Unit & Integration Tests
```bash
cd backend
dotnet test src/ServicePulse.Tests/ServicePulse.Tests.csproj
```
Expected: `Passed! - Failed: 0, Passed: 23, Skipped: 0, Total: 23`

---

## 3. Docker Operations

| Command | Description |
|---|---|
| `docker-compose up -d` | Start all services in background |
| `docker-compose down` | Stop and remove containers |
| `docker-compose logs -f api` | Follow API logs |
| `docker-compose logs -f ai-service` | Follow AI service logs |
| `docker-compose exec db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'ServicePulse@2026!'` | Connect to SQL Server |
| `docker-compose build --no-cache` | Rebuild all images |

---

## 4. Default Credentials

| Email | Password | Role | Access |
|---|---|---|---|
| `admin@servicepulse.local` | `Admin@123` | Admin | All calls, all features |
| `priya.sharma@servicepulse.local` | `Priya@123` | Manager | All calls, can review/coach |
| `alice.johnson@servicepulse.local` | `Alice@123` | Agent | Own calls only |

---

## 5. API Endpoints

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

## 6. Postman Testing Guide

### 6.1 Collection Setup
Create collection **ServicePulse API** with base URL:
- Docker: `http://localhost:8080`
- Local: `http://localhost:5295`

### 6.2 Auth Flow
1. **Register:** `POST /api/auth/register` with `{email, password, displayName, role}`
2. **Login:** `POST /api/auth/login` → returns `accessToken`
3. **Use token:** Add header `Authorization: Bearer <accessToken>` to all protected requests

### 6.3 Key Test Requests
| # | Name | Method | Endpoint | Notes |
|---|---|---|---|---|
| 1 | Health Check | GET | `/api/health` | No auth required |
| 2 | Get Calls (Agent) | GET | `/api/calls?page=1&pageSize=10` | As Alice - shows only her calls |
| 3 | Get Calls (Manager) | GET | `/api/calls?page=1&pageSize=10` | As Priya - shows all 15 calls |
| 4 | Search | GET | `/api/calls/search?status=Escalated` | Filter by status |
| 5 | Get Call Detail | GET | `/api/calls/{id}` | Includes transcript, analysis, review |
| 6 | Get Stats | GET | `/api/calls/stats` | Dashboard metrics |
| 7 | Create Review | POST | `/api/manager-reviews` | Manager/Admin only |

---

## 7. Quality Score Formula

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

## 8. Common Operations

### 8.1 Stop Services
```bash
# Docker
docker-compose down

# Local dev
taskkill //F //IM dotnet.exe
# Ctrl+C in npm terminal
```

### 8.2 Reset Database (Docker)
```bash
docker-compose down -v  # Removes volumes
docker-compose up -d
```

### 8.3 Reset Database (LocalDB)
```bash
cd backend/src/ServicePulse.Api
# Drop and recreate via SQL:
# DROP DATABASE ServicePulseDb; CREATE DATABASE ServicePulseDb;
# Then run migrations on next dotnet run
```

### 8.4 View Seed Data
```bash
# Docker
docker-compose exec db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'ServicePulse@2026!' -Q "USE ServicePulseDb; SELECT * FROM Calls;"

# LocalDB
sqlcmd -S "(localdb)\mssqllocaldb" -d ServicePulseDb -Q "SELECT * FROM Calls;"
```

---

## 9. Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| Port 8080 in use | Docker API container conflict | `docker-compose down` then restart |
| DB connection failed | SQL Server not ready | Wait for healthcheck (30s), check `docker-compose logs db` |
| AI service unhealthy | Gemini API key missing | Leave `GEMINI_API_KEY=` empty for rule-based fallback |
| Tests fail | LocalDB conflict with test DBs | Tests use isolated DBs; ensure no other tests running |
| Frontend blank | Vite dev server not running | `npm run dev` in `ServicePulse.Frontend.Vite` |
| CORS errors | Reverse proxy misconfig | Verify `API_BASE_URL` in `.env` matches backend |

---

## 10. Project Structure

```
ServicePulse_AI_System/
├── backend/
│   ├── src/
│   │   ├── ServicePulse.Domain/        # Entities, enums, domain rules
│   │   ├── ServicePulse.Application/   # DTOs, commands, queries
│   │   ├── ServicePulse.Infrastructure/# EF Core, Hangfire, AI client
│   │   ├── ServicePulse.Api/           # Controllers, middleware, Program.cs
│   │   └── ServicePulse.Tests/         # xUnit tests (23 passing)
│   └── frontend/
│       ├── ServicePulse.Frontend/          # MVC host (serves wwwroot)
│       └── ServicePulse.Frontend.Vite/     # Vite + React 19 source
├── ai-service/
│   └── app/                            # Python FastAPI microservice
├── database/
│   └── seed/                           # SQL seed scripts
├── docs/                               # Architecture & release docs
├── docker-compose.yml                  # Production stack
├── Dockerfile                          # .NET API image
├── ai-service/Dockerfile               # Python AI service image
├── .env.example                        # Environment template
├── .dockerignore                       # Docker build exclusions
├── CHANGELOG.md                        # Version history
├── README.md                           # Project overview
└── RUNBOOK.md                          # This file
```

---

## 11. Release History

| Version | Date | Highlights |
|---|---|---|
| **v2.0.0** | 2026-11-14 | Docker Compose, 15 transcripts, portfolio README |
| **v1.4.0** | 2026-11-14 | Role-based filtering, 6 integration tests |
| **v1.3.0** | 2026-11-14 | Provider tracking, Gemini fallback, audit logging |
| **v1.2.0** | 2026-11-13 | Manager reviews, coaching notes, audit trail |
| **v1.1.0** | 2026-11-13 | Hangfire, Python AI service, rule-based scoring |
| **v1.0.0** | 2026-11-13 | Core platform, Clean Architecture, React SPA |

---

## 12. Git Tags
```bash
# List all release tags
git tag -l

# Checkout a specific version
git checkout v1.4.0
```

---

## 13. CI/CD Notes

- All 23 tests must pass before merge
- Docker images build on push to main
- Health checks verify container readiness
- SQL Server Developer Edition used (free for dev/test)

---

*Last updated: 2026-11-14 for v2.0.0 release*