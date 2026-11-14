# ServicePulse AI System — Demo Video Script

**Target Length:** 5–7 minutes  
**Audience:** Portfolio reviewers, hiring managers, technical leads  
**Tone:** Professional, concise, technically impressive

---

## 🎬 Video Structure

| Segment | Duration | Content |
|---|---|---|
| **0:00–0:30** | 30s | Intro + Architecture Overview |
| **0:30–1:30** | 60s | Login & Role-Based Dashboards |
| **1:30–3:00** | 90s | Calls List + Search/Filter |
| **3:00–4:15** | 75s | Call Detail: Transcript + AI Analysis |
| **4:15–5:15** | 60s | Manager Review Workflow |
| **5:15–6:00** | 45s | Backend: API, Tests, Docker |
| **6:00–6:30** | 30s | Closing + Tech Stack Summary |

---

## 📝 Detailed Script

### 0:00–0:30 | Intro & Architecture
**[Screen: Terminal → `docker-compose up -d` → Browser opens to localhost:8080]**

> "ServicePulse AI is a full-stack customer service quality inspection platform. Built with ASP.NET Core 10 Clean Architecture on the backend, React 19 on the frontend, SQL Server for persistence, and a Python FastAPI microservice for AI-powered transcript analysis with Gemini LLM fallback."

**[Show architecture diagram from README]**

> "The system enforces clear boundaries: React owns the UI only, ASP.NET Core owns business logic and orchestration, SQL Server owns data integrity, and Python owns NLP processing. This separation allows each layer to scale and evolve independently."

---

### 0:30–1:30 | Login & Role-Based Dashboards
**[Screen: Login page → Login as `alice.johnson@servicepulse.local` / `Alice@123` (Agent)]**

> "Three roles with different permissions: Agents see only their own calls. Managers and Admins see everything. Let me log in as Alice, a Tier 1 Support agent."

**[Show dashboard with stat cards: Total Calls, Avg Quality Score, Escalation Rate, Pending Reviews]**

> "The dashboard shows Alice's personal metrics — 3 calls this month, 91 average quality score, zero escalations. Notice the navigation: Calls, Dashboard, and her profile dropdown. No Manager Review menu — she can't access it."

**[Logout → Login as `priya.sharma@servicepulse.local` / `Priya@123` (Manager)]**

> "Now as Priya, a Manager. Same dashboard but with ALL team calls visible — 15 total. And notice the new 'Manager Reviews' tab in the nav."

---

### 1:30–3:00 | Calls List + Search/Filter
**[Screen: /calls page with paginated table]**

> "The calls list supports server-side pagination — 10 per page by default, configurable. Each row shows agent, customer, date, duration, status, and AI quality score with color coding."

**[Demonstrate search bar: type "Whitfield" → filters to 1 row]**

> "Real-time search by customer name. Let me filter by status — 'Escalated' shows 2 calls. Date range picker works too. All filtering happens on the backend with proper indexes."

**[Click page 2 → shows different calls]**

> "Pagination is zero-offset server-side. Total count queries are optimized."

---

### 3:00–4:15 | Call Detail: Transcript + AI Analysis
**[Screen: Click CALL-003 (Haruto Tanaka — escalated, negative sentiment)]**

> "Call detail page shows three tabs: Transcript, AI Analysis, Manager Review."

**[Transcript tab — scroll through conversation]**

> "Full transcript with speaker labels. This is the angry customer threatening cancellation after 3 failed contacts."

**[AI Analysis tab — show sentiment: Negative, Risk: High, Score: 71]**

> "AI analysis runs asynchronously via Hangfire. Rule-based engine scored this 71 — negative sentiment, high risk due to churn indicators. Detected keywords: cancellation, escalation, enterprise, export timeout, repeated issue, churn risk, credit. The analysis also captures the provider — RuleBased here — and would show FallbackReason if Gemini had been attempted and failed."

**[Show CALL-001 (Alice/James — positive, 92.5)]**

> "Contrast with a high-scoring call: 92.5, positive sentiment, low risk. Keywords: double charge, refund, loyalty, courtesy credit, billing."

---

### 4:15–5:15 | Manager Review Workflow
**[Screen: Manager Reviews tab on CALL-003]**

> "Managers can create reviews with score override, coaching notes, and override reason. Let me open the review Priya already wrote."

**[Show existing review: Manager Score 78, coaching notes about de-escalation, process failure flagged]**

> "Priya agreed with the AI's direction but adjusted the score up to 78, noting Alice handled the situation well but the underlying process failure needs escalation to service delivery."

**[Create new review for CALL-012 (Emily Chen — retention save)]**

> "Let me add a review for the retention save. Score: 90. Coaching: 'Excellent retention save — listened to customer, analyzed usage, offered right alternative. Revenue preserved at $9.99 vs total loss.' Submit."

**[Show audit log concept — no UI but explain]**

> "Every review action creates an audit log entry with actor, timestamp, correlation ID — immutable trail for compliance."

---

### 5:15–6:00 | Backend: API, Tests, Docker
**[Screen: Terminal → `dotnet test` → 23 passing]**

> "23 tests passing: 17 unit tests covering controllers, auth, validation; 6 integration tests verifying provider tracking, fallback audit logging, role-based filtering, and review data integrity."

**[Screen: Swagger UI at /swagger]**

> "Full OpenAPI/Swagger documentation. All endpoints secured with JWT Bearer auth except health and auth endpoints."

**[Screen: `docker-compose.yml` → `docker-compose up -d`]**

> "Production deployment via Docker Compose: SQL Server, Python AI service, and .NET API all containerized. Multi-stage builds for minimal images. Health checks on every service."

---

### 6:00–6:30 | Closing
**[Screen: Terminal showing git log with tags v1.0, v1.1, v1.2, v1.3, v1.4, v2.0]**

> "Six incremental releases from core platform through Docker deployment. Tech stack: .NET 10, React 19, SQL Server, Python FastAPI, Hangfire, JWT, BCrypt, xUnit, Docker. Clean Architecture throughout. Thank you for watching!"

**[End screen: GitHub repo URL + LinkedIn]**

---

## 🎯 Key Talking Points to Emphasize

1. **Clean Architecture** — Not just folders, but enforced dependency direction
2. **Role-based security** — Not cosmetic; enforced at controller + query level
3. **Async AI pipeline** — Hangfire + FastAPI, not blocking requests
4. **Provider tracking** — Know exactly which AI model analyzed each call
5. **Audit logging** — Compliance-ready immutable trail
6. **Test discipline** — 23 tests, integration tests with real LocalDB
7. **Docker-native** — Runs identically in dev, CI, and prod

---

## 🎥 Recording Tips

- Record at 1080p/60fps minimum
- Use OBS or similar; capture browser + terminal side-by-side
- Narrate live — don't add voiceover later (more authentic)
- Zoom in on code/UI when explaining details
- Keep terminal font large (16pt+)
- Mute notifications before recording
- Do one clean take per segment; edit together

---

## 📎 Assets to Prepare

- [ ] LocalDB with seed data pre-loaded
- [ ] Docker Compose running in background
- [ ] Browser at 125% zoom for readability
- [ ] Terminal with `dotnet test` pre-run (show green output instantly)
- [ ] Swagger UI pre-loaded
- [ ] Git log with tags visible

---

*Generated for ServicePulse AI System v2.0 portfolio demonstration*