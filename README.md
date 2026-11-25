# SIH1329 — Web-based Platform for Collecting 360-Degree Feedback on Government of India News Stories from Regional Media

[![.NET 10.0](https://img.shields.io/badge/.NET-10.0%20LTS-purple.svg)](https://dotnet.microsoft.com/)
[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-blue.svg)](https://dotnet.microsoft.com/apps/aspnet/mvc)
[![Entity Framework Core](https://img.shields.io/badge/Entity%20Framework-Core%2010-green.svg)](https://learn.microsoft.com/ef/core/)
[![SQL Server 2022](https://img.shields.io/badge/Database-SQL%20Server-red.svg)](https://www.microsoft.com/sql-server)
[![Academic Prototype](https://img.shields.io/badge/Status-Academic%20Prototype%20(INT402)-orange.svg)]()

> **Academic Prototype Notice:** This software is an individual academic research prototype developed for **INT402 – Modern Web Programming Tools and Techniques**. It is designed solely for academic and technical demonstration and does not claim official affiliation with the Government of India or the Press Information Bureau (PIB).

---

## 1. Problem Statement & Objective

### The Problem:
There has historically been an absence of a unified, structured mechanism to measure the regional impact, public sentiment, and local reception of government policies and initiatives reported across regional media outlets in India's diverse states and regional languages.

### The Objective:
**GovFeedback360 (SIH1329)** provides a web-based platform that allows citizens across India to offer **360-degree feedback** on government-related regional news stories. The platform enables systematic analysis of public opinion, strengthens regional media engagement, and assists policy communication strategists in tailoring regional communication efforts.

---

## 2. Key Features

### For Citizens:
- **Regional News Discovery:** Browse, search, and filter news stories across 31 Indian States/UTs, 13 languages, and 10 government sectors (Infrastructure, Healthcare, Education, Agriculture, Economy, Defence, Environment, Social Welfare, Technology, Foreign Policy).
- **Structured 360° Feedback:** Submit feedback across five distinct dimensions:
  1. Overall Rating (1 to 5 Stars).
  2. Public Sentiment (Positive, Neutral, Negative).
  3. Focus Category (Policy Impact, Implementation, Regional Relevance, Transparency).
  4. Detailed Qualitative Comments.
  5. Constructive Policy Recommendations.
- **Duplicate Feedback Protection:** Database-enforced uniqueness guarantees one submission per citizen per story, preventing vote manipulation.
- **Citizen Dashboard & History:** Dedicated dashboard tracking total reviews, average ratings given, sentiment breakdown, and self-service edit/delete options.
- **Profile Management:** Manage state/UT, preferred language, age group, gender, and occupation demographics.

### For Administrators:
- **Administrative Control Panel (`_AdminLayout.cshtml`):** Secure, dedicated admin workspace.
- **Real-Time KPI Dashboard:** High-level metrics for Total Citizens, News Stories, Feedbacks, Average Star Rating, and Positive/Negative sentiment percentages.
- **News Story Management (CRUD):** Author, edit, publish/unpublish, archive, or remove regional news stories with rich metadata and source links.
- **Feedback Moderation Workflow:** Review citizen comments, inspect detected vs. self-reported sentiment, and approve or flag submissions with moderation audit notes.
- **Interactive Visualizations (Chart.js):**
  - Public Sentiment Doughnut Chart.
  - Regional Engagement Bar Chart.
  - 12-Month Longitudinal Trend Line.
  - Sector Category Distribution Bar Chart.
  - 1 to 5 Star Rating Distribution Chart.
  - News Published vs. Feedback Received Comparative Chart.
- **Live CSV Reporting:** Stream dynamically generated CSV exports for News Stories, Feedback Submissions, and User Demographics.

### RESTful Web APIs:
- Full REST API suite under `/api/news`, `/api/feedback`, `/api/auth`, and `/api/analytics`.
- Interactive Swagger / OpenAPI documentation available at `/swagger`.

---

## 3. Technology Stack

- **Framework:** ASP.NET Core 10.0 (C# 13, .NET 10 LTS)
- **Architecture:** Model-View-Controller (MVC) + Service Layer + Dependency Injection
- **Database Engine:** Microsoft SQL Server 2022 / SQL Server LocalDB (`(localdb)\mssqllocaldb`)
- **ORM:** Entity Framework Core 10.0 (`Microsoft.EntityFrameworkCore.SqlServer`)
- **Security & Identity:** ASP.NET Core Identity with PBKDF2 password hashing and Role-Based Access Control (RBAC)
- **Frontend / UI:** Bootstrap 5.3, FontAwesome 6, Chart.js 4.4, jQuery Validation
- **API Documentation:** Swashbuckle / OpenAPI 3.0

---

## 4. Architecture Overview

```
[ Web Browser / Client ]
           │ HTTP/HTTPS
           ▼
[ ASP.NET Core Pipeline: Routing ──▶ Authentication ──▶ Authorization ]
           │
     ┌─────┴─────────────────────────┐
     ▼                               ▼
[ MVC Controllers ]          [ REST API Controllers ]
(Home, News, Citizen,        (News, Feedback, Auth,
 Admin, Account, Reports)     Analytics)
     │                               │
     └───────────────┬───────────────┘
                     ▼
           [ Service Layer ]
    • INewsService / NewsService
    • IFeedbackService / FeedbackService
    • IAnalyticsService / AnalyticsService
    • ISentimentService / SentimentService
    • IDataSeeder / DataSeeder
                     │
                     ▼
       [ Entity Framework Core 10 ]
         (ApplicationDbContext)
                     │
                     ▼
       [ Microsoft SQL Server 2022 ]
   • Tables with Unique Constraints
   • Views: vw_NewsFeedbackSummary, vw_RegionalFeedbackSummary
   • Stored Procedures: sp_GetDashboardKPIs, sp_GetRegionalFeedbackAnalytics
```

---

## 5. Quick Start Instructions

### Prerequisites:
- .NET 10.0 SDK installed (`dotnet --version` => `10.0.400+`).
- Microsoft SQL Server LocalDB or SQL Server Express running.

### 1. Build:
```powershell
cd "GovernmentFeedbackPlatform"
dotnet restore
dotnet build --no-restore
```

### 2. Run:
```powershell
dotnet run
```
*Or open `GovernmentFeedbackPlatform.csproj` in Visual Studio and press F5.*

### 3. Open Browser:
- **Public Site:** `http://localhost:5189/`
- **Swagger Documentation:** `http://localhost:5189/swagger`
- **Login Portal:** `http://localhost:5189/Account/Login`

---

## 6. Pre-seeded Demonstration Credentials

The platform automatically initializes the SQL Server database with demonstration data on first launch:

| Role | Email | Password | Scope |
|---|---|---|---|
| **Administrator** | `admin@govfeedback.in` | `Admin@123456` | Full Admin Dashboard, News CRUD, Moderation, Analytics, CSV Exports |
| **Citizen Reviewer** | `priya.sharma@example.com` | `Citizen@123456` | Citizen Dashboard, 360° Feedback Submission, Feedback History |

---

## 7. Project Documentation Index

1. [PROJECT_AUDIT_REPORT.md](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/PROJECT_AUDIT_REPORT.md) — Initial audit, gap analysis, and issue resolution log.
2. [DEVELOPMENT_PROGRESS.md](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/DEVELOPMENT_PROGRESS.md) — Comprehensive development task progress tracker.
3. [PROJECT_ARCHITECTURE.md](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/PROJECT_ARCHITECTURE.md) — Architectural design, layered patterns, and security architecture.
4. [DATABASE_SCHEMA.md](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/DATABASE_SCHEMA.md) — ER diagram, table schemas, SQL views, and stored procedures.
5. [API_DOCUMENTATION.md](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/API_DOCUMENTATION.md) — RESTful API endpoints, request/response contracts, and status codes.
6. [INSTALLATION_GUIDE.md](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/INSTALLATION_GUIDE.md) — Detailed environment setup and execution instructions.
7. [TESTING_REPORT.md](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/TESTING_REPORT.md) — Build verification, functional test matrix, and security audit.
8. [COURSE_OUTCOME_MAPPING.md](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/COURSE_OUTCOME_MAPPING.md) — Explicit mapping of INT402 Course Outcomes (CO1 to CO6).

---

## 8. Limitations & Future Enhancements

- **Academic Prototype Scope:** Sample news stories and citizen reviews are fictional demonstrations created to validate the 360-degree feedback architecture.
- **NLP Sentiment Engine:** The prototype implements a transparent lexicon-based keyword scoring engine (`SentimentService.cs`). Future iterations could integrate transformer-based Indic multilingual sentiment models (e.g. IndicBERT) for enhanced vernacular nuance.
- **Media Ingestion:** Currently supports administrator news authoring. Future production systems can integrate automated RSS/API ingestion pipelines from Press Information Bureau (PIB) and verified regional media syndication feeds.
