# ServicePulse AI System — Runbook

## 1. Environment Requirements

| Component | Version |
|---|---|
| .NET SDK | 10.0.x |
| Node.js | 18+ |
| Database | SQLite (auto-created, no installation needed) |

## 2. First-Time Setup

### 2.1 Backend API
```bash
cd backend/src/ServicePulse.Api
dotnet run
```
The server starts on `http://localhost:5295`. Verify with:
```bash
curl http://localhost:5295/api/health
```
Expected response:
```json
{"status":"Healthy","environment":"Development","database":"Connected","timestamp":"..."}
```
On first run, EF Core migrations apply automatically and test data is seeded.

### 2.2 Frontend
```bash
cd backend/frontend/ServicePulse.Frontend/ClientApp
npm install
npm start
```
Open `http://localhost:3000` in your browser.

### 2.3 Unit Tests
```bash
cd backend
dotnet test src/ServicePulse.Tests/ServicePulse.Tests.csproj
```
Expected: `Passed! - Failed: 0, Passed: 9, Skipped: 0, Total: 9`

## 3. Common Operations

### 3.1 Stop Services
```bash
# Stop the API server
taskkill //F //IM dotnet.exe

# Stop the frontend (Ctrl+C in the npm terminal)
```

### 3.2 Reset the Database
```bash
cd backend/src/ServicePulse.Api
del ServicePulseDb.sqlite
dotnet run
```
This drops and recreates the database with fresh seed data.

### 3.3 Add a New API Endpoint
1. Create a new controller file in `src/ServicePulse.Api/Controllers/`
2. Follow the pattern: `[ApiController]` + `[Route("api/[controller]")]`
3. Restart `dotnet run` — changes take effect immediately

### 3.4 Frontend Development
```bash
cd backend/frontend/ServicePulse.Frontend/ClientApp
npm start
```
Hot reload is enabled — save any `.js` or `.jsx` file and the browser refreshes automatically.

## 4. Postman Testing Guide

### 4.1 Collection Setup
Create a new collection called **ServicePulse API** with base URL set to:
```
http://localhost:5295
```

### 4.2 Test Requests

| # | Name | Method | Endpoint | Expected Result |
|---|---|---|---|---|
| 1 | Get Calls | GET | `/api/calls?page=1&pageSize=10` | Array of call objects |
| 2 | Get Call Detail | GET | `/api/calls/{id}` | Full DTO with analysis and review |
| 3 | Search by Agent | GET | `/api/calls/search?agentName=Alice` | Only Alice's calls |
| 4 | Search by Status | GET | `/api/calls/search?status=Completed` | Only completed calls |
| 5 | Get Stats | GET | `/api/calls/stats` | Dashboard JSON with counts |
| 6 | Create Call | POST | `/api/calls` | 201 Created with call details |
| 7 | Create - Missing Agent | POST | `/api/calls` | 400 Bad Request |
| 8 | Unknown Call ID | GET | `/api/calls/00000000-0000-0000-0000-000000000000` | 404 Not Found |

### 4.3 Create Call — Request Body
```json
{
  "agentName": "Alice Johnson",
  "customerName": "Sarah O'Brien",
  "externalCallId": "EXT-001",
  "startedAtUtc": "2026-01-01T10:00:00Z",
  "durationSeconds": 120,
  "transcriptText": "Hello, this is a test call transcript."
}
```
Headers: `Content-Type: application/json`

## 5. Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| Port 5295 already in use | Old API process still running | Run `taskkill //F //IM dotnet.exe`, then restart |
| Frontend shows blank page | Dependencies missing | Run `npm install` in the ClientApp folder |
| SQLite locked error | Multiple processes writing simultaneously | Close all `dotnet run` instances, delete `.sqlite-wal`, restart |
| Unit tests fail after build | DLL locked by running API | Kill dotnet processes, then rebuild |
| `/api/health` returns 404 | HealthController not registered | Verify file exists at `src/ServicePulse.Api/Controllers/HealthController.cs` and rebuild |

## 6. Project Structure

```
ServicePulse_AI_System/
├── backend/
│   ├── src/
│   │   ├── ServicePulse.Domain/          # Entities, enums, domain rules
│   │   ├── ServicePulse.Application/     # DTOs, request models
│   │   ├── ServicePulse.Infrastructure/  # EF Core, migrations, seeding
│   │   ├── ServicePulse.Api/             # Controllers, middleware, Program.cs
│   │   └── ServicePulse.Tests/           # xUnit unit tests
│   └── frontend/
│       └── ServicePulse.Frontend/        # React + Reactstrap SPA
├── database/                             # Seed SQL scripts
├── docs/                                 # Architecture documentation
├── README.md                             # Project overview
└── RUNBOOK.md                            # This file
```

## 7. V1 Feature Checklist

- [x] Calls CRUD endpoints (GetCalls, GetCall, SearchCalls, GetStats, CreateCall)
- [x] Pagination and search/filter (by agent name, customer name, status, date range)
- [x] Analysis results display (sentiment, risk level, quality score, keywords)
- [x] Manager review display (reviewer name, score, coaching notes)
- [x] Dashboard statistics endpoint
- [x] Health check endpoint (`GET /api/health`)
- [x] Global exception handling middleware
- [x] Input validation (required fields, range checks, max length)
- [x] Unit tests (9/9 passing)
- [x] Frontend pager component
- [x] Postman API verification complete

## 8. Deploying to GitHub

```bash
# Check current state
git log --oneline -3
git remote -v

# Add remote (replace with your repo URL)
git remote add origin https://github.com/YOUR_USERNAME/ServicePulse-AI-System.git

# Push
git branch -M main
git push -u origin main
```
