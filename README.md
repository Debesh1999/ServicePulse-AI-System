# ServicePulse AI System

Full-stack customer service quality inspection system demonstrating Clean Architecture + React + SQLite + JWT Auth.

## Architecture Layers

| Layer | Responsibility | Path |
|---|---|---|
| Domain | Entities, enums, domain rules | `src/ServicePulse.Domain` |
| Application | DTOs, requests, shared services | `src/ServicePulse.Application` |
| Infrastructure | EF Core, migrations, data seeding | `src/ServicePulse.Infrastructure` |
| Api | HTTP endpoints, middleware | `src/ServicePulse.Api` |
| Frontend | React 18 + Reactstrap SPA | `frontend/ServicePulse.Frontend` |

## API Endpoints

- `GET /api/calls?page=&pageSize=` — Paginated call list
- `GET /api/calls/{id}` — Call detail (with analysis results and manager review)
- `GET /api/calls/search?agentName=&customerName=&status=&from=&to=` — Search and filter
- `GET /api/calls/stats` — Dashboard statistics
- `POST /api/calls` — Create new call
- `GET /api/health` — Health check (includes database connectivity)
- `POST /api/auth/register` — Register new user
- `POST /api/auth/login` — Login and receive JWT token
- `GET /api/auth/me` — Get current user info

## Running the Application

### Backend
```bash
cd backend/src/ServicePulse.Api
dotnet run --no-launch-settings
# Access http://localhost:5295/api/health
```

### Frontend (combined with backend proxy)
```bash
cd backend/frontend/ServicePulse.Frontend
npm install  # if not already done
npm run build  # builds React to wwwroot
dotnet run --no-launch-settings
# Access http://localhost:5212/
```

### Tests
```bash
cd backend
dotnet test src/ServicePulse.Tests/ServicePulse.Tests.csproj
```

## Default Credentials

| Email | Password | Role |
|---|---|---|
| admin@servicepulse.local | Admin@123 | Admin |
| priya.sharma@servicepulse.local | Priya@123 | Agent |
| alice.johnson@servicepulse.local | Alice@123 | Agent |

## V2 Feature Checklist

- [x] Calls CRUD endpoints (5)
- [x] Search with pagination
- [x] Analysis results and manager review display
- [x] Dashboard statistics
- [x] Health check endpoint
- [x] Global exception handling middleware
- [x] Input validation (Required, Range, MaxLength)
- [x] Unit tests (17/17 passing)
- [x] Frontend Pager component
- [x] JWT Authentication
- [x] User registration and login
- [x] Role-based authorization
- [x] React SPA with login/register pages
- [x] Protected routes
- [x] Reverse proxy for API requests
- [x] Postman interface verification
