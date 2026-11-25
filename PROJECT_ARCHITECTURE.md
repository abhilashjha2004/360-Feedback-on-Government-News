# SIH1329 — Project Architecture Documentation

**Project Title:** Web-based Platform for Collecting 360-Degree Feedback on Government of India News Stories from Regional Media  
**Project ID:** SIH1329  
**Target Course:** INT402 – Modern Web Programming Tools and Techniques  
**Technology Stack:** ASP.NET Core 10.0 MVC, Entity Framework Core 10, Microsoft SQL Server 2022, Chart.js, Bootstrap 5.3  

---

## 1. Architectural Style & Design Principles

The application is engineered using the standard **ASP.NET Core Model-View-Controller (MVC)** architectural pattern combined with a clean **Service Layer** and **Dependency Injection** container.

```
                    ┌────────────────────────┐
                    │      Client / UI       │
                    │ (Razor Views, Chart.js,│
                    │  Bootstrap 5, Vanilla) │
                    └───────────┬────────────┘
                                │ HTTP / HTTPS
                                ▼
┌─────────────────────────────────────────────────────────────┐
│                 ASP.NET Core Web Pipeline                   │
│                                                             │
│  Routing ──▶ Authentication ──▶ Authorization ──▶ MVC/API   │
└───────────────────────────────┬─────────────────────────────┘
                                │
        ┌───────────────────────┴────────────────────────┐
        ▼                                                ▼
┌───────────────────────────────┐        ┌───────────────────────────────┐
│        MVC Controllers        │        │        API Controllers        │
│ • HomeController              │        │ • NewsApiController           │
│ • NewsController              │        │ • FeedbackApiController       │
│ • CitizenController           │        │ • AuthApiController           │
│ • AdminController             │        │ • AnalyticsApiController      │
│ • AccountController           │        └───────────────┬───────────────┘
│ • AnalyticsController         │                        │
│ • ReportsController           │                        │
└───────────────┬───────────────┘                        │
                │                                        │
                └───────────────────┬────────────────────┘
                                    │
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│                          Service Layer                                 │
│ • INewsService / NewsService (Querying, Filtering, CRUD)               │
│ • IFeedbackService / FeedbackService (Aggregation, Sentiment, Moder)   │
│ • IAnalyticsService / AnalyticsService (KPIs, Regional & Trends)       │
│ • ISentimentService / SentimentService (Rule-based Opinion Classifier) │
│ • IDataSeeder / DataSeeder (Seed data & DB Views / Stored Procedures)  │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│                        Data Access Layer                               │
│ • ApplicationDbContext (EF Core 10)                                    │
│ • Entities: ApplicationUser, NewsStory, Category, Feedback, AuditLog   │
│ • Composite Unique Indexes & Relationships                             │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│                   Microsoft SQL Server 2022                            │
│ • Tables & Constraints                                                 │
│ • SQL Views (vw_NewsFeedbackSummary, vw_RegionalFeedbackSummary)       │
│ • Stored Procedures (sp_GetDashboardKPIs, sp_GetRegionalFeedback...)   │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Layered Responsibilities

### A. Presentation Layer (Views & ViewModels)
- **Public Views:** Responsive news browsing, searching, sector filtering, and detailed story views.
- **Citizen Views:** Dedicated citizen dashboard, 360° feedback submission form, feedback history, and self-service edit/delete capabilities.
- **Administrative Views:** Secure admin layout (`_AdminLayout.cshtml`), real-time KPI overview, interactive Chart.js visualizations, news story authoring/editing, feedback moderation, user audit, and CSV reporting.
- **ViewModels:** Strongly-typed contracts ensuring complete separation of internal database entities from view representations.

### B. Controller Layer
- Lightweight controllers adhering to Single Responsibility. Complex filtering, statistical calculations, and sentiment determinations are delegated to services.
- **API Controllers:** Attribute-routed controllers under `/api/*` returning standardized JSON with HTTP status codes (200 OK, 201 Created, 204 NoContent, 400 BadRequest, 401 Unauthorized, 404 NotFound, 409 Conflict).

### C. Service Layer
- **NewsService:** Handles multi-parameter filtering (Search, CategoryId, State, Language), pagination calculations, and publishing status.
- **FeedbackService:** Enforces business logic such as one feedback per user per story, sentiment detection, automated rating distribution, and moderation workflow.
- **AnalyticsService:** Calculates dynamic, real-time KPI cards, category aggregates, state-wise breakdowns, and monthly trends using LINQ queries.
- **SentimentService:** Dual-layer sentiment architecture. Analyzes citizen comments and suggestions against positive and negative lexical vectors to provide automated baseline classification without replacing the citizen's self-reported sentiment.

### D. Data Layer
- Configured using **Entity Framework Core 10.0**.
- Uses `Microsoft.EntityFrameworkCore.SqlServer` connecting to SQL Server LocalDB (`(localdb)\mssqllocaldb`).
- Composite unique index on `(UserId, NewsStoryId)` guarantees data integrity at the database engine level.

---

## 3. Security Architecture
- **Identity:** ASP.NET Core Identity with hashed credentials (`PBKDF2` with HMAC-SHA256).
- **Authentication:** Cookie-based session authentication with `SlidingExpiration` and secure HTTP-only cookies.
- **Authorization:** Role-Based Access Control (RBAC) separating `Admin` and `Citizen` roles.
- **CSRF Defense:** `[ValidateAntiForgeryToken]` applied to all state-modifying POST requests.
- **XSS & Injection Protection:** Razor auto-encoding, Entity Framework Core parameterized SQL queries, and input sanitization.
