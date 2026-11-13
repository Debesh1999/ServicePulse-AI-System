# ServicePulse AI — Architecture & System Design Guide

This document provides a comprehensive technical blueprint of the **ServicePulse AI** platform. It is designed to explain **what** we are building, **how** components communicate, and **why** specific architectural decisions were made.

---

## 🗺️ 1. High-Level Architecture Overview

```
                                  [ USER BROWSER ]
                                         |
                                         | HTTPS / REST (JSON)
                                         v
                            +--------------------------+
                            |     REACT FRONTEND       |
                            |   (Vite / React 18)      |
                            +--------------------------+
                                         |
                                         | REST API (Axios)
                                         v
+-----------------------------------------------------------------------------------+
|                            ASP.NET CORE 9 WEB API                                 |
|                                                                                   |
|  [ API Layer ]         --->  [ Application Layer ] ---> [ Domain Layer ]          |
|  Controllers, Auth           CQRS, Commands/Queries     Entities & Rules          |
|                                        |                                          |
|                                        v                                          |
|                              [ Infrastructure Layer ]                             |
|                              EF Core, FastAPI Client                              |
+-----------------------------------------------------------------------------------+
           |                                                      |
           | EF Core / SQL Client                                 | HTTP POST (Internal)
           v                                                      v
+---------------------+                                +----------------------------+
|     SQL SERVER      |                                |   PYTHON FASTAPI SERVICE   |
| (System of Record)  |                                |    (Transcript Analyzer)   |
|                     |                                |                            |
| - Calls             |                                | - RuleBasedAnalyzer        |
| - Transcripts       |                                | - GeminiLLM (Optional)     |
| - AnalysisResults   |                                | - Sentiment & Risk Engine  |
| - ManagerReviews    |                                +----------------------------+
| - AuditLogs         |                                               |
+---------------------+                                               | (Optional)
                                                                      v
                                                           +--------------------+
                                                           | Hosted Gemini API  |
                                                           +--------------------+
```

---

## 💡 2. Architectural Principles & Component Boundaries

### Why this architecture?
As a **.NET Developer**, you are building a system where **ASP.NET Core owns the business domain**. Python and React are supporting technologies that fulfill specialized roles.

| Component | Responsibility (What it OWNS) | Forbidden Boundaries (What it MUST NOT OWN) |
|---|---|---|
| **React (Frontend)** | User interface, routing, forms, presentation logic. | Direct database access, business rules, or direct calls to Python. |
| **ASP.NET Core (Backend)** | Workflow orchestration, security, data persistence, audit logging, API endpoints. | Heavy Machine Learning or raw NLP processing code. |
| **SQL Server (Database)** | Relational data persistence, integrity constraints, transactional boundaries. | Business logic or UI state. |
| **Python FastAPI (AI Service)**| Text sentiment detection, keyword matching, score calculation, text summarization. | User accounts, persistence, business records, or state management. |

---

## 🏛️ 3. Backend Clean Architecture (.NET)

The ASP.NET Core solution is structured into four distinct layers according to **Clean Architecture** principles:

```
                  +-----------------------------------+
                  |         ServicePulse.Api          |
                  |   (Controllers, Swagger, Middleware)|
                  +-----------------------------------+
                                    |
                                    v
                  +-----------------------------------+
                  |     ServicePulse.Infrastructure   |
                  | (EF Core, Repositories, FastAPI)  |
                  +-----------------------------------+
                                    |
                                    v
                  +-----------------------------------+
                  |     ServicePulse.Application      |
                  |    (Commands, Queries, DTOs)      |
                  +-----------------------------------+
                                    |
                                    v
                  +-----------------------------------+
                  |       ServicePulse.Domain         |
                  |   (Entities, Enums, Core Rules)   |
                  +-----------------------------------+
```

### Dependency Rules:
1. **Domain:** Has zero dependencies. Contains core business entities (`Call`, `CallTranscript`, `AnalysisResult`, `ManagerReview`, `AuditLog`).
2. **Application:** Depends only on Domain. Defines DTOs, request handlers (CQRS commands/queries), and repository interfaces.
3. **Infrastructure:** Implements interfaces defined in Application (EF Core DbContext, HTTP clients to Python, Hangfire background jobs).
4. **API:** Entry point. Configures Dependency Injection, HTTP routes, JWT authentication, and global exception handling.

---

## 🗄️ 4. Entity Relationship Diagram (ERD) & Database Schema

SQL Server stores all business records. Tables are linked using Foreign Keys (`uniqueidentifier` / `GUID`).

```
 +--------------------+       1      *       +--------------------+
 |   ApplicationUser  |----------------------|    ManagerReview   |
 +--------------------+                      +--------------------+
 | Id (PK)            |                      | Id (PK)            |
 | Email              |                      | CallId (FK)        |
 | PasswordHash       |                      | ReviewerUserId(FK) |
 | DisplayName        |                      | ManagerScore       |
 | Role               |                      | CoachingNotes      |
 +--------------------+                      | OverrideReason     |
           | 1                               +--------------------+
           |                                           |
           | * (Optional)                              | *
 +--------------------+                      +--------------------+
 |       Agent        |                      |      AuditLog      |
 +--------------------+                      +--------------------+
 | Id (PK)            |                      | Id (PK)            |
 | UserId (FK)        |                      | EntityName         |
 | EmployeeCode       |                      | EntityId           |
 | FullName           |                      | Action             |
 | TeamName           |                      | ActorUserId (FK)   |
 +--------------------+                      +--------------------+
           | 1
           |
           | *
 +--------------------+       *      1       +--------------------+
 |        Call        |----------------------|      Customer      |
 +--------------------+                      +--------------------+
 | Id (PK)            |                      | Id (PK)            |
 | AgentId (FK)       |                      | ExternalReference  |
 | CustomerId (FK)    |                      | DisplayName        |
 | StartedAtUtc       |                      +--------------------+
 | DurationSeconds    |
 | Status             |
 +--------------------+
      | 1        | 1
      |          |
      | 1        | *
 +--------------------+    +--------------------+
 |   CallTranscript   |    |   AnalysisResult   |
 +--------------------+    +--------------------+
 | Id (PK)            |    | Id (PK)            |
 | CallId (FK)        |    | CallId (FK)        |
 | Text (nvarchar)    |    | Sentiment          |
 | LanguageCode       |    | RiskLevel          |
 | WordCount          |    | QualityScore       |
 +--------------------+    | Summary            |
                           | AnalyzerVersion    |
                           +--------------------+
```

---

## 🔄 5. Core Application Workflows

### Sequence: Submitting & Analyzing a Call Transcript

```
React (UI)            ASP.NET Core API          SQL Server           Hangfire           Python FastAPI
   |                         |                      |                   |                     |
   |--- 1. POST /calls ----->|                      |                   |                     |
   |   (Metadata + Text)     |--- 2. Begin Trans --->|                   |                     |
   |                         |--- 3. Insert Call -->|                   |                     |
   |                         |--- 4. Insert Text -->|                   |                     |
   |                         |--- 5. Commit Trans ->|                   |                     |
   |<-- 6. 202 Accepted -----|                      |                   |                     |
   |   (Status: Queued)      |--- 7. Queue Job ------------------------>|                     |
   |                         |                      |                   |--- 8. Run Job ----->|
   |                         |                      |                   |   (POST /analyze)   |
   |                         |                      |                   |<-- 9. Result -------|
   |                         |<-- 10. Save Result --|                   |   (Score, Summary)  |
   |                         |--- 11. Status=Comp ->|                   |                     |
   |                         |                      |                   |                     |
   |--- 12. Poll status ---->|                      |                   |                     |
   |<-- 13. Completed Data --|                      |                   |                     |
```

---

## 🐍 6. Python Analysis Microservice Design

The Python service runs using **FastAPI** and **Pydantic**. It evaluates transcripts using a rule-based engine (with optional LLM enhancement).

### Explainable Quality Score Calculation Formula:
```
Base Score = 50.0

+ 10.0  if Greeting detected ("hello", "thank you for calling")
+ 20.0  if Resolution wording detected ("issue is resolved", "fixed")
+ 15.0  if Apology + Next step present ("sorry for the inconvenience", "let me help")
- 10.0  if Refund / Cancellation keywords present
- 15.0  if Frustrated / Angry language present
- 20.0  if Supervisor escalation requested ("speak to a manager")

Final Quality Score = Clamped between 0.0 and 100.0
```

---

## 🎯 7. Next Steps & Development Roadmap

We are following a **6-Stage Release Plan**:
1. **v1.0 (Current):** Setup ASP.NET Core Clean Architecture, EF Core Migrations, Seed Data, and React CRUD.
2. **v1.1:** Build Python FastAPI service & Hangfire background job queue.
3. **v1.2:** Build Manager Review workflow, coaching notes, AI score override & audit log.
4. **v1.3:** Connect hosted AI (Gemini) with automatic rule-based fallback.
5. **v1.4:** Security polish (JWT authentication, role authorization, integration testing).
6. **v2.0:** Docker Compose containerization & portfolio presentation.
