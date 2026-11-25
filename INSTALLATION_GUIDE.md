# SIH1329 — Installation & Setup Guide

**Project Title:** Web-based Platform for Collecting 360-Degree Feedback on Government of India News Stories from Regional Media  
**Target Environment:** Windows 10/11, macOS, Linux  
**Runtime:** .NET 10.0 SDK (`10.0.400` or higher)  
**Database:** Microsoft SQL Server 2019/2022 or SQL Server LocalDB (`(localdb)\mssqllocaldb`)  

---

## 1. Prerequisites

Before setting up the project, ensure the following software is installed on your workstation:
1. **.NET 10.0 SDK** (Verify by running `dotnet --version`).
2. **Microsoft SQL Server LocalDB** (included with Visual Studio) or **SQL Server Express/Developer Edition**.
3. **Web Browser** (Google Chrome, Microsoft Edge, Mozilla Firefox).
4. *(Optional)* **Visual Studio 2022/2025** or **Visual Studio Code** with C# Dev Kit extension.

---

## 2. Clone / Locate the Project

The project is structured with a root directory containing documentation and the web application project folder `GovernmentFeedbackPlatform`:

```text
360-Degree Government News Feedback Platform/
├── GovernmentFeedbackPlatform/        <-- ASP.NET Core MVC Project
│   ├── Controllers/
│   ├── Models/
│   ├── ViewModels/
│   ├── Services/
│   ├── Data/
│   ├── Views/
│   ├── wwwroot/
│   ├── appsettings.json
│   ├── Program.cs
│   └── GovernmentFeedbackPlatform.csproj
├── README.md
├── PROJECT_AUDIT_REPORT.md
├── PROJECT_ARCHITECTURE.md
├── DATABASE_SCHEMA.md
├── API_DOCUMENTATION.md
├── COURSE_OUTCOME_MAPPING.md
└── TESTING_REPORT.md
```

---

## 3. Database Configuration

The application is configured to connect to SQL Server LocalDB by default.

### Check `appsettings.json`:
Open `GovernmentFeedbackPlatform/appsettings.json` and verify the connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GovernmentFeedbackDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

*Note:* If using a named SQL Server Express instance, change the connection string to:
`"Server=localhost\\SQLEXPRESS;Database=GovernmentFeedbackDB;Trusted_Connection=True;TrustServerCertificate=True"`

### Ensure LocalDB is Running (Windows):
In PowerShell or Command Prompt, run:
```powershell
sqllocaldb start MSSQLLocalDB
```

---

## 4. Building the Application

Open a terminal in the project directory:
```powershell
cd "GovernmentFeedbackPlatform"
dotnet restore
dotnet build --no-restore
```
The build should succeed with **0 Errors**.

---

## 5. Running the Application

### Option A: Using Visual Studio (Recommended on Windows)
1. Open Visual Studio.
2. Open `GovernmentFeedbackPlatform.csproj`.
3. Press `F5` or click **Run (https / http)**.
4. Visual Studio automatically handles developer execution policies, starts the server, initializes the database tables and seeder data, and opens the browser.

### Option B: Using .NET CLI
In the project directory:
```powershell
dotnet run
```
The application will listen on:
- `http://localhost:5189`
- `https://localhost:7106`

*Windows Security Note:* If your Windows 11 machine has **Smart App Control** or **Windows Defender Application Control (WDAC)** enabled in strict block mode (error `0x800711C7`), either run the project via Visual Studio (which runs in an approved developer context) or run PowerShell as Administrator and configure developer mode permissions for local user-built DLLs.

---

## 6. Accessing the Application

Open your browser and navigate to:
- **Public Homepage:** `http://localhost:5189/`
- **News Stories Listing:** `http://localhost:5189/News`
- **Swagger / OpenAPI Documentation:** `http://localhost:5189/swagger`
- **Citizen Portal / Login:** `http://localhost:5189/Account/Login`

---

## 7. Demonstration Accounts

The platform automatically seeds demonstration accounts on first launch:

### Platform Administrator Account:
- **Email:** `admin@govfeedback.in`
- **Password:** `Admin@123456`
- **Capabilities:** Admin Dashboard, Create/Edit/Delete News, Feedback Moderation, User Demographics, Analytics, CSV Export Reports.

### Citizen Demonstration Account:
- **Email:** `priya.sharma@example.com`
- **Password:** `Citizen@123456`
- **Capabilities:** Browse news, submit 360° feedback, view feedback history, edit/delete submitted feedback, view citizen dashboard.
