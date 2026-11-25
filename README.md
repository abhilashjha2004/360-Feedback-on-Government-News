# SIH1329 — Web-based Platform for Collecting 360-Degree Feedback on Government of India News Stories from Regional Media

[![.NET 10.0](https://img.shields.io/badge/.NET-10.0%20LTS-purple.svg)](https://dotnet.microsoft.com/)
[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-blue.svg)](https://dotnet.microsoft.com/apps/aspnet/mvc)
[![Entity Framework Core](https://img.shields.io/badge/Entity%20Framework-Core%2010-green.svg)](https://learn.microsoft.com/ef/core/)
[![SQL Server 2022](https://img.shields.io/badge/Database-SQL%20Server%202022-red.svg)](https://www.microsoft.com/sql-server)
[![Docker Containerized](https://img.shields.io/badge/Docker-Containerized-2496ED.svg)](https://www.docker.com/)
[![Academic Prototype](https://img.shields.io/badge/Status-Academic%20Prototype%20(INT402)-orange.svg)]()

> **Academic Prototype Notice:** This software is an individual academic research prototype developed for **INT402 – Modern Web Programming Tools and Techniques** (Project ID: **SIH1329**). It is designed solely for academic and technical evaluation and does not claim official affiliation with the Government of India or the Press Information Bureau (PIB).

---

## 1. Project Overview & Problem Statement

### The Problem:
Central government policies and public initiatives are predominantly analyzed through English and Hindi national media. However, regional news outlets across India's diverse states (published in languages such as Marathi, Tamil, Punjabi, Telugu, and Bengali) capture critical grassroots concerns and regional nuances that remain largely unmeasured. Historically, there has been no structured mechanism for citizens to provide multi-dimensional feedback on regional news coverage.

### The Solution:
**GovFeedback360 (SIH1329)** is a full-stack web platform that bridges this communication gap. It enables citizens to:
1. Discover and read government-related news reported by regional media outlets.
2. Submit structured **360-degree feedback** (star ratings, focus areas, sentiments, comments, and constructive suggestions).
3. Access real-time sentiment intelligence, state-by-state satisfaction rankings, and interactive visual dashboards.
4. Allow administrators to manage news stories, moderate feedback, and export live datasets.

---

## 2. Live Application Access & Seeded Credentials

- **Application URL:** [http://localhost:5189](http://localhost:5189)
- **Interactive Swagger REST API:** [http://localhost:5189/swagger](http://localhost:5189/swagger)
- **Presentation Deck:** [`SIH1329_Project_Presentation.pptx`](./SIH1329_Project_Presentation.pptx)

The database is pre-seeded with **20 users**, **15 regional news stories**, **10 governance categories**, and **51 feedback records**:

| Role | Email | Password | Access Scope |
|---|---|---|---|
| **Administrator** | `admin@govfeedback.in` | `Admin@123` | Full administrative control: News CRUD, Feedback Moderation, Analytics, CSV Exports, User Directory. |
| **Citizen (Default)** | `citizen1@example.com` | `Citizen@123` | News reading, 360° review submission, rating, Citizen Dashboard, feedback self-editing and deletion. |
| **Regional Citizens** | `citizen2@example.com` to `citizen19@example.com` | `Citizen@123` | Pre-seeded regional citizen accounts from Maharashtra, Delhi, Tamil Nadu, UP, Punjab, etc. |

---

## 3. Quick Start with Docker

The entire platform runs in an isolated multi-container Linux Docker environment (ASP.NET Core 10 MVC + Microsoft SQL Server 2022 Linux):

```powershell
# 1. Start the platform in the background
docker compose up -d

# 2. Check running status of containers
docker compose ps

# 3. View live application logs
docker compose logs -f govfeedback360-app

# 4. View SQL Server database logs
docker compose logs -f govfeedback360-sqlserver

# 5. Stop the platform cleanly
docker compose stop
```

---

## 4. Key Features & Workflows

### A. Public Portal & Regional News Discovery
- **Multi-Faceted Regional Filter (`/News`):** Filter by **31 Indian States/UTs**, **13 Regional Languages** (Hindi, Marathi, Tamil, Punjabi, Telugu, etc.), and **10 Governance Sectors**.
- **Keyword Search & Pagination:** Search headlines and summaries with clean 12-card pagination.
- **Story Reader & AI Summaries (`/News/Details/{id}`):** Full news reader displaying publication attribution (e.g. *Lokmat*, *The Hindu*, *Dainik Jagran*), external source link, AI-generated summary box, and live community rating breakdown.

### B. 360-Degree Feedback Engine
- **5-Dimensional Assessment:**
  1. *Star Rating:* Interactive 1-to-5 star selector with dynamic status labels ("Very Poor" to "Excellent").
  2. *User Sentiment:* Positive, Neutral, or Negative selection.
  3. *Focus Dimension:* Implementation, Transparency, Public Impact, Clarity, or Regional Relevance.
  4. *Qualitative Commentary:* Detailed citizen feedback textarea.
  5. *Constructive Suggestions:* Actionable recommendations for policymakers.
- **Duplicate Prevention Engine:** Database-enforced composite unique constraint on `(UserId, NewsStoryId)` guarantees one submission per citizen per story.
- **Dual-Layer Sentiment Analysis:** Employs an automated lexicon-based algorithm (`SentimentService`) that compares user-declared sentiment with algorithmic sentiment detection.

### C. Citizen Portal & Review History
- **Citizen Dashboard (`/Citizen/Dashboard`):** Displays total contributed reviews, personal average rating, and sentiment split.
- **Feedback History (`/Citizen/History`):** Chronological log of past submissions.
- **Self-Editing & Deletion (`/Citizen/EditFeedback/{id}`):** Edit ratings/comments or soft-delete prior feedback.

### D. Administrative Governance (`/Admin/*`)
- **Admin Workspace (`_AdminLayout.cshtml`):** Responsive dark sidebar navigation.
- **News Story Lifecycle:** Full CRUD (`/Admin/ManageNews`, `/Admin/CreateNews`, `/Admin/EditNews`) with publish/unpublish toggles.
- **Feedback Moderation Queue (`/Admin/ManageFeedback`):** Moderate citizen comments, filter by sentiment or flag status, and approve or flag reviews with audit notes.
- **User Directory (`/Admin/Users`):** Audit registered citizens, home state, preferred language, and registration dates.
- **Sector Categories (`/Admin/Categories`):** Manage 10 governance sectors with story count badges and color branding.

### E. Visual Analytics & Intelligence (`/Admin/Analytics`)
- **Sentiment Doughnut Chart:** Real-time distribution of Positive, Neutral, and Negative citizen sentiment.
- **Regional Reception Bar Chart:** Compares feedback volume and average satisfaction rating across Indian states.
- **Monthly Feedback Trends:** Line chart tracking citizen participation volume over 6-month intervals.
- **Rating Distribution:** Bar chart displaying counts of 1-star through 5-star ratings.

### F. Reports & CSV Data Export (`/Admin/Reports`)
- **Discussion Ranking Tables:** Top discussed news stories and regional response summaries.
- **Live CSV Downloads:**
  - `GET /Reports/ExportNewsCsv` — Complete news catalogue with media attribution and view counts.
  - `GET /Reports/ExportFeedbackCsv` — All citizen reviews with ratings, sentiments, and suggestions.
  - `GET /Reports/ExportUsersCsv` — Registered citizen demographics (excluding password hashes).

---

## 5. System Architecture & Technical Stack

```
├── Presentation Layer      -> ASP.NET Core 10 MVC (Razor Views, Bootstrap 5.3, Chart.js)
├── API Gateway             -> RESTful Web APIs (/api/*) + Swagger / OpenAPI Documentation
├── Business Logic Layer    -> INewsService, IFeedbackService, ISentimentService, IAnalyticsService
├── Data Access Layer       -> Entity Framework Core 10 + LINQ + Stored Procedures
└── Database Infrastructure -> Microsoft SQL Server 2022 (Linux Docker Container)
```

- **Backend:** C# 13, ASP.NET Core 10.0 MVC, .NET 10 LTS
- **Database:** Microsoft SQL Server 2022 Linux Container (`mcr.microsoft.com/mssql/server:2022-latest`)
- **ORM:** Entity Framework Core 10.0 (`Microsoft.EntityFrameworkCore.SqlServer`)
- **Authentication:** ASP.NET Core Identity with PBKDF2 password hashing & Role-Based Access Control (RBAC)
- **Frontend:** Razor Views, Bootstrap 5.3, FontAwesome 6, Chart.js 4.4, jQuery Validation
- **Containerization:** Multi-stage Docker build on `mcr.microsoft.com/dotnet/aspnet:10.0`
- **Port Bindings:** `5189` (Web App), `1433` (SQL Server)

---

## 6. Database Schema & Advanced SQL Objects (CO5)

- **Primary Entities:** `AspNetUsers`, `Categories`, `NewsStories`, `Feedbacks`, `AuditLogs`.
- **Integrity Constraints:**
  - Composite unique index on `(UserId, NewsStoryId)` in `ApplicationDbContext` to block duplicate feedback.
  - `OnDelete: Restrict` on `Category` to safeguard news integrity.
- **SQL Views:**
  - `vw_NewsFeedbackSummary`: Aggregates story feedback totals, average rating, and sentiment counts.
  - `vw_RegionalFeedbackSummary`: State-by-state news volume, feedback submissions, and satisfaction scores.
- **Stored Procedures:**
  - `sp_GetDashboardKPIs`: High-speed platform KPI rollup (Total Users, News, Feedbacks, Avg Rating).
  - `sp_GetRegionalFeedbackAnalytics`: State-wise feedback ranking filtered by optional `@State`.

---

## 7. RESTful Web API Endpoints

| Endpoint | Method | Role | Description |
|---|---|---|---|
| `/api/news` | GET | Public | Paginated published regional news stories with search & filters. |
| `/api/news/{id}` | GET | Public | Single story details by identifier. |
| `/api/news` | POST | Admin | Create a new news story. |
| `/api/news/{id}` | PUT | Admin | Update an existing news story. |
| `/api/news/{id}` | DELETE | Admin | Delete a news story. |
| `/api/feedback?newsId={id}` | GET | Public | Approved feedback for a specific story. |
| `/api/feedback/stats/{id}` | GET | Public | Aggregate ratings and sentiment breakdown. |
| `/api/feedback` | POST | Citizen | Submit a 360° feedback review (duplicate protected). |
| `/api/feedback/{id}` | PUT | Citizen | Edit own feedback. |
| `/api/feedback/{id}` | DELETE | Citizen | Soft-delete own feedback. |
| `/api/auth/register` | POST | Public | Citizen registration endpoint. |
| `/api/auth/login` | POST | Public | User authentication endpoint returning claims. |
| `/api/analytics/overview` | GET | Public | Platform-wide summary KPIs. |
| `/api/analytics/sentiment` | GET | Public | Live sentiment counts and percentage ratios. |
| `/api/analytics/regional` | GET | Public | State-by-state news and feedback statistics. |
| `/api/analytics/monthly` | GET | Public | Multi-month trend dataset. |

---

## 8. INT402 Course Outcome (CO) Mapping

| Course Outcome | Description | Implementation Evidence in SIH1329 |
|---|---|---|
| **CO1** | Modern Web Programming Tools & Architecture | Clean N-Tier ASP.NET Core 10 MVC architecture with DI container, middleware pipeline, and modular services. |
| **CO2** | Responsive Frontend & User Interfaces | 28 Razor views and partials using Bootstrap 5.3, responsive admin layout (`_AdminLayout`), and Chart.js graphs. |
| **CO3** | Server-Side Programming & Services | 5 data services (`NewsService`, `FeedbackService`, `SentimentService`, `AnalyticsService`, `DataSeeder`) with asynchronous LINQ. |
| **CO4** | Web Security & Identity Management | ASP.NET Core Identity, PBKDF2 password hashing, RBAC (`Admin`, `Citizen`), Anti-Forgery tokens (CSRF), and input validation. |
| **CO5** | Database Design & Stored Procedures | EF Core relational modeling, composite unique index, SQL views (`vw_RegionalFeedbackSummary`), and stored procedures (`sp_GetDashboardKPIs`). |
| **CO6** | Web Services & Containerized Deployment | Swagger-documented REST APIs (`/api/*`), JSON cycle prevention, and production-grade Linux Docker multi-container stack. |

---

## 9. Project File & Directory Structure

```
├── .dockerignore                            # Docker context exclusion rules
├── .env.example                             # Environment variable template
├── docker-compose.yml                       # Multi-container orchestration (App + SQL Server)
├── README.md                                # Master project documentation & presentation guide
├── SIH1329_Project_Presentation.pptx        # 16-slide academic PowerPoint presentation deck
└── GovernmentFeedbackPlatform/              # C# Solution Root
    ├── Controllers/                         # MVC Controllers (Home, News, Feedback, Citizen, Admin, Reports)
    │   └── Api/                             # REST API Controllers (News, Feedback, Auth, Analytics)
    ├── Data/                                # DbContext & Stored Procedures SQL scripts
    ├── Dockerfile                           # Multi-stage .NET 10 Docker build definition
    ├── Models/                              # Entity domain models (ApplicationUser, NewsStory, Feedback, Category)
    ├── Services/                            # Business logic services & data seeder
    ├── ViewModels/                          # Strong-typed ViewModels for views & analytics
    ├── Views/                               # 28 Razor views organized by module
    │   ├── Account/                         # Login, Register, Profile, AccessDenied
    │   ├── Admin/                           # Dashboard, ManageNews, Moderation, Analytics, Reports, Users
    │   ├── Citizen/                         # Dashboard, History, EditFeedback
    │   ├── Home/                            # Index, About, Privacy
    │   ├── News/                            # Index, Details (360° review form)
    │   └── Shared/                          # _Layout, _AdminLayout, partials
    └── wwwroot/                             # Static assets (Bootstrap, Chart.js, CSS, JS, images)
```

---

## 10. License & Academic Attribution

This project is developed as an academic submission for **INT402 – Modern Web Programming Tools and Techniques** under Smart India Hackathon Problem Statement **SIH1329**. All trademarks, media names, and logos belong to their respective copyright holders and are referenced purely for academic simulation.
