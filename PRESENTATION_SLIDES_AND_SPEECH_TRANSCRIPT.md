# SIH1329 — Complete Presentation Deck & Viva Speech Transcript

**Project ID:** SIH1329  
**Project Title:** Web-based Platform for Collecting 360-Degree Feedback on Government of India News Stories from Regional Media  
**Target Course:** INT402 Modern Web Programming Tools and Techniques  
**Presentation File:** [SIH1329_Project_Presentation.pptx](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/SIH1329_Project_Presentation.pptx)  
**Total Slides:** 16 Widescreen (16:9) Professional Slides  

---

## Slide-by-Slide Outline & Verbatim Viva Speech Script

### Slide 1: Title Slide (Dark Navy Theme)
- **Header Badge:** Smart India Hackathon • ID: SIH1329
- **Title:** Web-based Platform for Collecting 360-Degree Feedback on Government of India News Stories from Regional Media
- **Subtitle:** Empowering Citizen Engagement, Regional Sentiment Intelligence & Public Policy Communication Analysis
- **Course Information:** INT402 – Modern Web Programming Tools and Techniques
- **Tech Stack:** ASP.NET Core 10 MVC • Entity Framework Core • Linux SQL Server 2022 • Docker • Chart.js • Bootstrap 5
- **Academic Disclaimer:** Academic research prototype developed for INT402 evaluation.
> **Speaker Notes:**
> *"Good morning/afternoon respected evaluators. Today I am presenting my academic project for INT402: A Web-based Platform for Collecting 360-Degree Feedback on Government of India News Stories from Regional Media, addressing Problem Statement SIH1329. This platform bridges the vital communication gap between grassroots regional media reporting and central public policy feedback using modern .NET 10, Docker, and SQL Server."*

---

### Slide 2: Problem Statement & Background
- **The Core Challenge:**
  - Grassroots Sentiment Disconnect: Central government communication is predominantly analyzed in English and Hindi national media, missing ground realities captured in regional press.
  - Absence of Structured Feedback: Existing portals provide unidirectional press releases with no systematic mechanism for citizens to provide multi-dimensional qualitative and quantitative ratings.
  - Regional Disparities: Policies affecting agriculture, infrastructure, or healthcare impact states differently, requiring state-level sentiment breakdown.
- **Why SIH1329 Matters:**
  - Regional newspapers and digital outlets possess deep contextual knowledge of local citizen concerns.
  - Enables 360-degree evaluation beyond simple 'like/dislike'.
  - Provides policymakers with real-time sentiment intelligence.
> **Speaker Notes:**
> *"In India, regional newspapers and digital outlets have massive readership and capture nuances that national media overlooks. The problem identified in SIH1329 is that there is no structured mechanism to aggregate feedback on these regional stories. Our solution enables citizens from all 31 States and Union Territories to voice feedback across 5 dimensions."*

---

### Slide 3: Project Objectives & Technical Vision
1. **Centralized Regional News Discovery:** Aggregates regional stories across 31 States/UTs, 13 languages, and 10 governance sectors.
2. **Multi-Dimensional 360° Review Engine:** Captures 1-to-5 star ratings, focus dimension, sentiment, and constructive suggestions.
3. **Automated Lexicon Sentiment Analysis:** Algorithmic sentiment classification comparing declared vs detected sentiment.
4. **Role-Based Portals:** Dedicated Citizen Dashboard (self-editing/history) and Admin Workspace (moderation, CRUD).
5. **Regional Analytics & Stored Procedures:** Real-time Chart.js visual dashboards powered by SQL Server Views and stored procedures.
6. **RESTful APIs & Open Standards:** Full Swagger/OpenAPI-documented REST APIs.
> **Speaker Notes:**
> *"Here are our 6 core project objectives. Notice how they satisfy every aspect of modern web application engineering: content aggregation, multi-dimensional user feedback, natural language processing, role-based access control, advanced relational database optimization via stored procedures, and open REST APIs."*

---

### Slide 4: N-Tier System Architecture
- **Presentation Layer:** Razor Views, Bootstrap 5.3, `_AdminLayout.cshtml`, Chart.js, Vanilla JavaScript.
- **API & Gateway Layer:** ASP.NET Core 10 Web APIs, Swagger, Anti-Forgery Validation, `ReferenceHandler.IgnoreCycles`.
- **Business Logic Layer:** `NewsService`, `FeedbackService`, `SentimentService`, `AnalyticsService`, `DataSeeder`.
- **Data Access Layer:** `ApplicationDbContext`, LINQ, SQL Views (`vw_RegionalFeedbackSummary`), Stored Procedures (`sp_GetDashboardKPIs`, `sp_GetRegionalFeedbackAnalytics`).
- **Database & Container Infrastructure:** SQL Server 2022 Linux Container, Docker Multi-Container Network, Persistent Volume.
> **Speaker Notes:**
> *"The system strictly follows an N-Tier Clean Architecture. Controllers depend on interface abstractions, decoupling business logic from database interactions. Data access uses Entity Framework Core 10 with direct fallback to high-performance stored procedures. Everything is encapsulated in Linux Docker containers."*

---

### Slide 5: Relational Database Design & Schema Integrity (CO5)
- **Table Entities:** `AspNetUsers`, `Categories`, `NewsStories`, `Feedbacks`, `AuditLogs`.
- **Key Integrity Enforcements:**
  1. *Duplicate Feedback Prevention:* Composite unique index on `(UserId, NewsStoryId)` in `ApplicationDbContext`.
  2. *Cascading Delete Protections:* `Categories` set to `OnDelete: Restrict`; `Feedbacks` cascade on story deletion.
  3. *SQL Views:* `vw_NewsFeedbackSummary` & `vw_RegionalFeedbackSummary`.
  4. *Stored Procedures:* `sp_GetDashboardKPIs` & `sp_GetRegionalFeedbackAnalytics`.
> **Speaker Notes:**
> *"Data integrity is paramount. In ApplicationDbContext, we configured a composite unique index on UserId and NewsStoryId to prevent review spamming at the database engine level. We also created SQL views and stored procedures for CO5 compliance."*

---

### Slide 6: Public News Discovery & Regional Filtering
- **Multi-Faceted Filtering:** 31 Indian States/UTs, 13 Regional Languages, 10 Governance Sectors, keyword search.
- **Story Reader & AI Summaries:** Full article reader with regional media attribution, external source link, and AI summary box.
- **Community Sentiment Breakdown:** Real-time average rating, positive/neutral/negative percentage bar, and 1-5 star vote counts.
> **Speaker Notes:**
> *"The public news discovery portal allows citizens to browse national and regional stories with zero friction. Every news card shows the originating regional outlet, such as Lokmat or Dinamalar, and displays current feedback metrics."*

---

### Slide 7: 360-Degree Feedback Engine & Dual Sentiment
- **5-Dimensional Assessment:**
  - *Dim 1:* 1-to-5 Star Interactive Rating with live labels.
  - *Dim 2:* User-Declared Sentiment (Positive / Neutral / Negative).
  - *Dim 3:* Evaluation Focus Area (Implementation, Transparency, Public Impact, Clarity, Regional Relevance).
  - *Dim 4:* Qualitative Commentary.
  - *Dim 5:* Constructive Suggestions for policymakers.
- **Dual Sentiment Architecture:**
  - Compares declared citizen sentiment against an automated lexicon scoring algorithm (`SentimentService`).
> **Speaker Notes:**
> *"Why is it called 360-Degree feedback? Because it captures 5 distinct dimensions: Star rating, declared sentiment, focus dimension, descriptive comments, and actionable suggestions. Crucially, we cross-validate user sentiment with an algorithmic sentiment detection service."*

---

### Slide 8: Citizen Portal & Feedback Management
- **Personal Review Hub:** Citizen Dashboard (`/Citizen/Dashboard`) displaying review counts and average rating given.
- **Historical Activity Table:** Chronological list of past reviews with sentiment tags.
- **Self-Editing & Soft Deletion:** Edit ratings or comments as policies evolve; ownership validation prevents unauthorized tampering.
> **Speaker Notes:**
> *"Citizens have complete transparency and ownership over their data. They can revisit any feedback they previously submitted, edit their rating if government implementation changes, or soft-delete it. Their dashboard also tracks their review contributions."*

---

### Slide 9: Administrative Governance & Content Management
- **News Story Lifecycle:** Full CRUD (`/Admin/ManageNews`, `/Admin/CreateNews`, `/Admin/EditNews`) with publish/unpublish toggles.
- **Feedback Moderation Queue:** Review citizen comments, filter by sentiment or flag status, and approve or flag inappropriate submissions.
- **Directories:** Registered citizen demographic directory (`/Admin/Users`) and Governance sectors directory (`/Admin/Categories`).
> **Speaker Notes:**
> *"Administrators have a dedicated control center. They can publish new regional stories, review feedback in a moderation queue, and inspect registered citizen demographics to ensure balanced regional representation across India."*

---

### Slide 10: Real-Time Visual Analytics & Sentiment Intelligence
- **Sentiment Doughnut Chart:** Visual distribution of Positive, Neutral, and Negative citizen sentiment.
- **Regional Satisfaction Bar Chart:** Side-by-side comparison of feedback volume and satisfaction across Indian states.
- **Monthly Engagement Trend:** Line chart tracking participation volume over time.
- **Rating Distribution Bar Chart:** Breakdown of 1-star through 5-star ratings.
> **Speaker Notes:**
> *"The Analytics dashboard leverages Chart.js to render 4 distinct visual perspectives. All metrics are aggregated directly from SQL Server using views and stored procedures, ensuring lightning-fast performance even with large datasets."*

---

### Slide 11: Reporting & Real-Time Data Export (ReportsController)
- **Executive Reporting Views:** Top Discussed Stories ranking table and Regional Media Response summary table.
- **Live CSV Exports:**
  - `GET /Reports/ExportNewsCsv` — Full news catalogue with media attribution and view counts.
  - `GET /Reports/ExportFeedbackCsv` — All citizen reviews with ratings, sentiments, and suggestions.
  - `GET /Reports/ExportUsersCsv` — Registered citizen demographics (excluding password hashes).
> **Speaker Notes:**
> *"Reports are not just visual; they are fully exportable. We implemented ReportsController with live CSV generators so policy analysts can download the raw datasets into Microsoft Excel, Python, or Power BI for external statistical analysis."*

---

### Slide 12: RESTful Web APIs & Swagger/OpenAPI Documentation
- **News API (`/api/news`):** Paginated GET, GET by ID, POST (Admin), PUT (Admin), DELETE (Admin).
- **Feedback API (`/api/feedback`):** GET by story, GET stats, POST feedback, PUT (own), DELETE (own).
- **Analytics API (`/api/analytics`):** GET overview, GET sentiment, GET regional, GET monthly trends.
- **Interactive Documentation:** Available at `/swagger/index.html`.
> **Speaker Notes:**
> *"In addition to Razor MVC views, the platform exposes a complete RESTful Web API suite documented with Swagger/OpenAPI at /swagger. We configured ReferenceHandler.IgnoreCycles to ensure seamless JSON serialization without object cycles."*

---

### Slide 13: Production Deployment & Windows SAC Resolution
- **Windows 11 SAC Challenge (0x800711C7):**
  - Smart App Control blocked bare-metal execution of unsigned local DLLs.
  - Resolved cleanly without modifying host security policies by containerizing into official Linux Docker images.
- **Multi-Container Stack:**
  - `govfeedback360-app` on Port 5189.
  - `govfeedback360-sqlserver` on Port 1433 with persistent volume `govfeedback360-sql-data`.
- **Existing Host Containers:** All pre-existing containers (`ezyedutube-jenkins`, EzyEduTube stack) were 100% preserved.
> **Speaker Notes:**
> *"A major technical achievement of this project was overcoming the Windows 11 Smart App Control restriction. Instead of attempting risky registry bypasses, we engineered a production-grade multi-container Docker solution running ASP.NET Core 10 and Linux SQL Server 2022."*

---

### Slide 14: INT402 Course Outcome (CO) Mapping
- **CO1:** Clean N-Tier ASP.NET Core 10 MVC architecture with DI and middleware.
- **CO2:** 28 responsive Razor views with Bootstrap 5.3, interactive star widgets, and Chart.js.
- **CO3:** 5 server-side data services with asynchronous LINQ processing.
- **CO4:** ASP.NET Identity, PBKDF2 password hashing, RBAC, and CSRF protection.
- **CO5:** Relational schema, composite unique index, SQL views, and stored procedures.
- **CO6:** Swagger-documented REST APIs and containerized Linux Docker deployment.
> **Speaker Notes:**
> *"This slide maps the project directly against every course outcome of INT402: Modern web architecture (CO1), responsive design (CO2), server-side services (CO3), security and identity (CO4), database stored procedures (CO5), and RESTful web services with containerized deployment (CO6)."*

---

### Slide 15: Live Demonstration Metrics & Verification Evidence
- **Active Seeded Metrics:** 15 Regional News Stories • 51 Feedback Submissions • 20 Users • 2.7 National Average Rating.
- **Verified Endpoints:**
  - `/` (Home) => HTTP 200 OK
  - `/News` (News Index) => HTTP 200 OK
  - `/News/Details/1` (Story Details) => HTTP 200 OK
  - `/Account/Login` (Login) => HTTP 200 OK
  - `/api/analytics/overview` (API) => HTTP 200 OK
  - `/swagger/index.html` (Swagger) => HTTP 200 OK
- **Stored Procedures:** `sp_GetDashboardKPIs` & `sp_GetRegionalFeedbackAnalytics` verified via `sqlcmd`.
> **Speaker Notes:**
> *"Here are the live metrics currently running in our Docker environment. Every endpoint has been verified with curl and returns HTTP 200 OK. 51 feedbacks across 5 states and 4 languages demonstrate the platform's multi-regional capabilities."*

---

### Slide 16: Conclusion & Future Roadmap
- **Key Achievements:**
  - Solves SIH1329 with a complete 360-degree feedback loop.
  - Zero compilation errors; 28 Razor views and full DI services.
  - Production-ready Linux Docker deployment.
  - 100% INT402 Course Outcome compliance.
- **Future Roadmap:**
  - AI Bhashini / Indic-BERT automated translation for all 22 scheduled Indian languages.
  - Voice-to-text input for rural citizens.
  - WhatsApp / Telegram bot integration.
  - Integration with PIB Fact Check database.
> **Speaker Notes:**
> *"In conclusion, SIH1329 demonstrates how modern web programming tools (.NET 10, EF Core, Docker, SQL Server) can be applied to solve an essential national governance communication challenge. In the future, we plan to integrate Bhashini AI translation and voice input. Thank you, and I am now open to your questions."*
