# SIH1329 — Database Architecture & Migration Plan

**Project:** SIH1329 — 360-Degree Government News Feedback Platform  
**Target Course:** INT402 Modern Web Programming Tools and Techniques  
**Database Technology:** Microsoft SQL Server (Mandatory per INT402 CO5)  
**ORM:** Entity Framework Core 10.0  

---

## 1. Technical Problem Statement

In the existing development environment on the Windows host, the application uses **SQL Server LocalDB** (`(localdb)\mssqllocaldb;Database=GovernmentFeedbackDB`).

### The Docker Challenge:
- SQL Server LocalDB is a Windows-only user-instance database engine that operates through local Windows Named Pipes (`np:\\.\pipe\LOCALDB#...`).
- When the ASP.NET Core application runs inside a **Linux Docker container**, the container runs under a Linux kernel (WSL2/Hyper-V).
- Linux containers cannot communicate over Windows named pipes and therefore **cannot connect to SQL Server LocalDB directly**.
- Furthermore, under the INT402 curriculum, **SQL Server cannot be replaced with MySQL or MongoDB**. SQL Server must remain the primary database to satisfy **Course Outcome 5 (CO5)**.

---

## 2. Evaluation of Strategic Database Options

### OPTION A: Host-Based SQL Server Exposed via TCP/IP
- **Concept:** Expose the Windows host's SQL Server over TCP/IP port 1433 and connect from Docker using `host.docker.internal,1433`.
- **Feasibility on this Machine:**
  - SQL Server LocalDB does **not** listen on TCP/IP by default; it is strictly a named-pipe local process.
  - Enabling TCP/IP on LocalDB requires undocumented manual registry hacks and fixed-port batch scripts that violate our rule against altering security/system registry settings.
  - A standalone SQL Server Express/Developer service with TCP enabled is not currently listening on port 1433 (verified: port 1433 is completely unallocated).
- **Conclusion:** Not viable without modifying Windows system configurations.

---

### OPTION B: Dedicated SQL Server 2022 Linux Container (*RECOMMENDED*)
- **Concept:** Deploy the official Microsoft SQL Server 2022 container (`mcr.microsoft.com/mssql/server:2022-latest`) inside the same isolated Docker network (`govfeedback360-network`).
- **How It Works:**
  - Both the ASP.NET Core application container and the SQL Server container communicate over Docker's internal DNS network:
    ```text
    Server=govfeedback360-sqlserver;Database=GovernmentFeedbackDB;User Id=sa;Password=<SecurePassword>;TrustServerCertificate=True;
    ```
  - Data persistence is guaranteed by mounting a dedicated Docker volume: `govfeedback360-sql-data:/var/opt/mssql/data`.
  - Windows host port 1433 is optionally mapped (`1433:1433`), allowing tools like SQL Server Management Studio (SSMS) or Azure Data Studio on Windows to inspect the database directly.
- **Why This is the Best Architectural Choice:**
  1. **Zero Changes to Windows Security:** Code Integrity, Smart App Control, and Windows Defender are 100% untouched.
  2. **100% SQL Server & CO5 Compliant:** Uses authentic Microsoft SQL Server 2022 engine running T-SQL, Stored Procedures, Views, and EF Core relational constraints.
  3. **High Portability:** Entire academic project can be demonstrated on any machine running Docker Desktop with a single command (`docker compose up`).
  4. **Isolated from Existing Containers:** Does not touch Jenkins, EzyEduTube, MySQL, or Cassandra containers on your workstation.

---

### OPTION C: Remote Cloud SQL Server (Azure SQL / AWS RDS)
- **Concept:** Provision a cloud-hosted Microsoft Azure SQL Database and connect over the internet.
- **Feasibility:** Requires an active Azure subscription, internet dependency during offline evaluation, and recurring cloud costs.
- **Conclusion:** Unnecessary given that local containerization is free, fast, and self-contained.

---

## 3. Recommended Architecture: Multi-Container Docker Compose

```
┌────────────────────────────────────────────────────────────────────────┐
│                        Windows 11 Workstation                          │
│                                                                        │
│   Web Browser ────────▶ http://localhost:5189 (Public Web UI & API)   │
│   SSMS / Azure Studio ─▶ localhost:1433       (Direct SQL Inspection)  │
└───────────────────────────────────┬────────────────────────────────────┘
                                    │ Port Forwarding
                                    ▼
┌────────────────────────────────────────────────────────────────────────┐
│               Docker Desktop (WSL2 / Linux Micro-VM)                   │
│                                                                        │
│  [ govfeedback360-network (Isolated Bridge Network) ]                  │
│                                                                        │
│  ┌──────────────────────────────┐     ┌─────────────────────────────┐  │
│  │   govfeedback360-dev         │     │  govfeedback360-sqlserver   │  │
│  │                              │     │                             │  │
│  │ • ASP.NET Core 10 MVC / API  │────▶│ • Microsoft SQL Server 2022 │  │
│  │ • Port: 5189                 │ SQL │ • Port: 1433                │  │
│  │ • C# 13 / EF Core 10         │     │ • Tables, Views, Procedures │  │
│  └──────────────────────────────┘     └──────────────┬──────────────┘  │
│                                                      │                 │
│                                                      ▼                 │
│                                      [ govfeedback360-sql-data ]       │
│                                      (Persistent Docker Volume)        │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 4. Migration & Schema Initialization Plan

### Step 1: Volume Persistence
Create a persistent Docker volume named `govfeedback360-sql-data`. This ensures that even if containers are stopped, all news stories, citizen profiles, and submitted feedbacks persist across reboots.

### Step 2: Database Initialization on First Boot
Because LocalDB records are local to the Windows user session, the application's built-in seeder (`DataSeeder.cs` and `db.Database.EnsureCreated()`) will automatically execute when the container starts against the clean SQL Server container:
1. **Schema Generation:** Automatically creates all Identity tables (`AspNetUsers`, `AspNetRoles`, etc.) and domain tables (`Categories`, `NewsStories`, `Feedbacks`, `AuditLogs`).
2. **Constraint Enforcement:** Creates composite unique indexes and foreign key cascades.
3. **Seed Data:** Populates:
   - 10 Government sector categories.
   - Platform Administrator (`admin@govfeedback.in` / `Admin@123456`).
   - 19 Diverse citizen profiles across Indian States and Languages (`priya.sharma@example.com`, etc.).
   - 15 Detailed regional news stories.
   - 55 Realistic citizen 360° feedback submissions with sentiment scores.
4. **SQL Views & Stored Procedures (CO5):** Executes `DatabaseViewsAndProcedures.sql`, creating:
   - `vw_NewsFeedbackSummary`
   - `vw_RegionalFeedbackSummary`
   - `vw_SentimentSummary`
   - `sp_GetDashboardKPIs`
   - `sp_GetRegionalFeedbackAnalytics`

---

## 5. Security & Licensing Requirements for Approval

| Parameter | Specification | Compliance Detail |
|---|---|---|
| **SQL Server Image** | `mcr.microsoft.com/mssql/server:2022-latest` | Official Microsoft container image |
| **Download Size** | ~600 MB compressed | One-time download from Microsoft MCR |
| **Disk Requirement** | ~1.6 GB on disk | Available on Drive C: **175.3 GB** (More than enough) |
| **RAM Requirement** | Minimum 2 GB RAM | Fits within workstation resources |
| **Licensing / EULA** | Developer Edition (`ACCEPT_EULA=Y`) | Fully legal and free for academic and development usage |
| **Password Policy** | Strong SA Password | Configured via environment file; not committed to git |
