# INT402 — Course Outcome Mapping & Academic Compliance

**Course Title:** Modern Web Programming Tools and Techniques  
**Course Code:** INT402  
**Project ID:** SIH1329  
**Project Title:** Web-based Platform for Collecting 360-Degree Feedback on Government of India News Stories from Regional Media  
**Student Submission:** Individual Academic Project Prototype  

---

## Comprehensive Course Outcome (CO) Evidence Matrix

| Course Outcome | Curriculum Description | Project Implementation Evidence | Source Code File References |
|---|---|---|---|
| **CO1** | Understand .NET Core architecture, development environment setup, project configuration, and CLI commands. | Configured on modern LTS `.NET 10.0` (`net10.0`). Demonstrated complete project configuration via `csproj`, `appsettings.json`, `launchSettings.json`, `.NET CLI` workflow (`dotnet restore`, `dotnet build`), and NuGet dependency management. | [GovernmentFeedbackPlatform.csproj](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/GovernmentFeedbackPlatform.csproj)<br>[appsettings.json](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/appsettings.json)<br>[launchSettings.json](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Properties/launchSettings.json) |
| **CO2** | Apply C# programming constructs, OOP principles, collections, LINQ, interfaces, and exception handling. | Implemented clean OOP with interfaces (`INewsService`, `IFeedbackService`, `IAnalyticsService`, `ISentimentService`, `IDataSeeder`), strong encapsulation, domain inheritance (`ApplicationUser : IdentityUser`), enum types (`SentimentType`), robust LINQ transformations (`GroupBy`, `Select`, `OrderByDescending`), and defensive exception handling. | [INewsService.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Services/INewsService.cs)<br>[NewsService.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Services/NewsService.cs)<br>[SentimentService.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Services/SentimentService.cs)<br>[ApplicationUser.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Models/ApplicationUser.cs) |
| **CO3** | Develop web applications using ASP.NET Core MVC architecture, Controllers, Razor Views, ViewModels, and Validation. | Implemented complete MVC triad. Controllers (`Home`, `News`, `Citizen`, `Admin`, `Account`, `Reports`, `Analytics`), strongly-typed ViewModels (`NewsDetailViewModel`, `AdminDashboardViewModel`, `CitizenDashboardViewModel`, `RegisterViewModel`), Model Binding, Data Annotations (`[Required]`, `[Range]`, `[EmailAddress]`), Razor Tag Helpers, and Master Layouts (`_Layout.cshtml`, `_AdminLayout.cshtml`). | [NewsController.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Controllers/NewsController.cs)<br>[CitizenController.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Controllers/CitizenController.cs)<br>[NewsViewModels.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/ViewModels/NewsViewModels.cs)<br>[Details.cshtml](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Views/News/Details.cshtml) |
| **CO4** | Configure ASP.NET Core Middleware pipeline, Dependency Injection (DI) service lifetimes, and Authentication / Authorization. | Configured middleware in `Program.cs` (`UseRouting`, `UseAuthentication`, `UseAuthorization`, `UseStaticFiles`, `UseSwagger`). Registered services in DI container (`AddScoped<INewsService, NewsService>`). Configured ASP.NET Core Identity with PBKDF2 password hashing, secure cookie policies, and Role-Based Access Control (`[Authorize(Roles = "Admin")]`, `[Authorize]`). | [Program.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Program.cs)<br>[AccountController.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Controllers/AccountController.cs)<br>[AdminController.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Controllers/AdminController.cs) |
| **CO5** | Design and interact with Microsoft SQL Server databases using Entity Framework Core, Tables, Constraints, Views, and Stored Procedures. | Database model built on SQL Server 2022 via EF Core. Configured Primary Keys, Foreign Keys, `OnDelete` cascade and restrict behaviors, composite unique index (`UserId + NewsStoryId`), automated seeder (`DataSeeder.cs`), custom SQL Views (`vw_NewsFeedbackSummary`, `vw_RegionalFeedbackSummary`), and Stored Procedures (`sp_GetDashboardKPIs`, `sp_GetRegionalFeedbackAnalytics`). | [ApplicationDbContext.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Data/ApplicationDbContext.cs)<br>[DatabaseViewsAndProcedures.sql](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Data/DatabaseViewsAndProcedures.sql)<br>[DataSeeder.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Services/DataSeeder.cs) |
| **CO6** | Design, build, test, and document RESTful Web APIs returning JSON, supporting CRUD operations and OpenAPI / Swagger. | Developed full RESTful API suite under `/api/news`, `/api/feedback`, `/api/auth`, `/api/analytics`. Implemented proper HTTP methods (`GET`, `POST`, `PUT`, `DELETE`), status codes (`200 OK`, `201 Created`, `204 NoContent`, `400 BadRequest`, `401 Unauthorized`, `409 Conflict`), and integrated OpenAPI/Swagger UI. | [NewsApiController.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Controllers/Api/NewsApiController.cs)<br>[FeedbackApiController.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Controllers/Api/FeedbackApiController.cs)<br>[AuthApiController.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Controllers/Api/AuthApiController.cs)<br>[AnalyticsApiController.cs](file:///c:/Users/abhil/Desktop/360-Degree%20Government%20News%20Feedback%20Platform/GovernmentFeedbackPlatform/Controllers/Api/AnalyticsApiController.cs) |

---

## Detailed Code Walkthrough for Evaluation

### 1. CO1: CLI and Configuration
The project is built on .NET 10.0 and uses standard CLI commands:
- Build: `dotnet build --no-restore`
- Run: `dotnet run`
- Packages: `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.SqlServer`, `Swashbuckle.AspNetCore`.

### 2. CO2: OOP and Business Logic
- **Interfaces:** `INewsService`, `IFeedbackService`, `IAnalyticsService`, `ISentimentService`, `IDataSeeder` enforce dependency inversion.
- **LINQ Operations:** In `AnalyticsService.cs`, complex grouping, projection, and aggregations are performed cleanly:
```csharp
return feedbacks
    .GroupBy(f => f.NewsStory!.State!)
    .Select(g => new RegionalStat
    {
        State = g.Key,
        FeedbackCount = g.Count(),
        NewsCount = newsDict.TryGetValue(g.Key, out var nc) ? nc : 0,
        AverageRating = g.Count() > 0 ? Math.Round(g.Average(f => f.Rating), 1) : 0
    })
    .OrderByDescending(r => r.FeedbackCount)
    .ToList();
```

### 3. CO3: MVC Triad & Validation
- **Controller:** Accepts incoming requests, validates model state, binds ViewModels, passes data to Views.
- **Views:** Razor syntax with Tag Helpers, Master Layouts (`_Layout.cshtml`, `_AdminLayout.cshtml`), dynamic Chart.js scripts.
- **Validation:** Both client-side (unobtrusive jQuery validation) and server-side (`ModelState.IsValid`).

### 4. CO4: Dependency Injection & Security
Registered in `Program.cs`:
```csharp
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<ISentimentService, SentimentService>();
builder.Services.AddScoped<IDataSeeder, DataSeeder>();
```
Protected via `[Authorize(Roles = "Admin")]` on Administrative endpoints and `[Authorize]` on Citizen submission/profile routes.

### 5. CO5: Advanced SQL Server Operations
- Entity mappings in `ApplicationDbContext.cs` enforce a composite unique constraint:
```csharp
modelBuilder.Entity<Feedback>(e =>
{
    e.HasIndex(f => new { f.UserId, f.NewsStoryId }).IsUnique();
});
```
- SQL Views (`vw_NewsFeedbackSummary`, `vw_RegionalFeedbackSummary`) and Stored Procedures (`sp_GetDashboardKPIs`, `sp_GetRegionalFeedbackAnalytics`) execute directly on the database engine.

### 6. CO6: RESTful APIs
RESTful controllers in `Controllers/Api/`:
- `GET /api/news`: Paginated list of regional stories.
- `GET /api/feedback/stats/{newsId}`: Summary stats for feedback on a specific story.
- `POST /api/feedback`: Submit structured feedback.
- `POST /api/auth/login`: Authenticate and obtain role status.
- `GET /api/analytics/overview`: High-level platform KPIs.
