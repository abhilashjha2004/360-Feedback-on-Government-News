using GovernmentFeedbackPlatform.Data;
using GovernmentFeedbackPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GovernmentFeedbackPlatform.Services
{
    public class DataSeeder : IDataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedAdminUserAsync();
            await SeedUsersAsync();
            await SeedNewsAsync();
            await SeedFeedbacksAsync();
            await SeedDatabaseObjectsAsync();
        }

        private async Task SeedDatabaseObjectsAsync()
        {
            try
            {
                // 1. View: vw_NewsFeedbackSummary
                await _context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('vw_NewsFeedbackSummary', 'V') IS NOT NULL DROP VIEW vw_NewsFeedbackSummary;
                ");
                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE VIEW vw_NewsFeedbackSummary AS
                    SELECT 
                        n.Id AS NewsId,
                        n.Title,
                        c.Name AS CategoryName,
                        n.State,
                        n.Language,
                        n.PublishedDate,
                        COUNT(f.Id) AS TotalFeedbacks,
                        ISNULL(ROUND(AVG(CAST(f.Rating AS FLOAT)), 2), 0) AS AverageRating,
                        SUM(CASE WHEN f.UserSentiment = 1 OR f.DetectedSentiment = 1 THEN 1 ELSE 0 END) AS PositiveCount,
                        SUM(CASE WHEN f.UserSentiment = 2 OR f.DetectedSentiment = 2 THEN 1 ELSE 0 END) AS NeutralCount,
                        SUM(CASE WHEN f.UserSentiment = 3 OR f.DetectedSentiment = 3 THEN 1 ELSE 0 END) AS NegativeCount
                    FROM NewsStories n
                    LEFT JOIN Categories c ON n.CategoryId = c.Id
                    LEFT JOIN Feedbacks f ON n.Id = f.NewsStoryId AND f.IsDeleted = 0
                    WHERE n.IsPublished = 1
                    GROUP BY n.Id, n.Title, c.Name, n.State, n.Language, n.PublishedDate;
                ");

                // 2. View: vw_RegionalFeedbackSummary
                await _context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('vw_RegionalFeedbackSummary', 'V') IS NOT NULL DROP VIEW vw_RegionalFeedbackSummary;
                ");
                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE VIEW vw_RegionalFeedbackSummary AS
                    SELECT 
                        ISNULL(n.State, 'National') AS State,
                        COUNT(DISTINCT n.Id) AS NewsCount,
                        COUNT(f.Id) AS FeedbackCount,
                        ISNULL(ROUND(AVG(CAST(f.Rating AS FLOAT)), 2), 0) AS AverageRating
                    FROM NewsStories n
                    LEFT JOIN Feedbacks f ON n.Id = f.NewsStoryId AND f.IsDeleted = 0
                    WHERE n.IsPublished = 1
                    GROUP BY n.State;
                ");

                // 3. Stored Procedure: sp_GetDashboardKPIs
                await _context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('sp_GetDashboardKPIs', 'P') IS NOT NULL DROP PROCEDURE sp_GetDashboardKPIs;
                ");
                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE PROCEDURE sp_GetDashboardKPIs
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        SELECT 
                            (SELECT COUNT(*) FROM AspNetUsers) AS TotalUsers,
                            (SELECT COUNT(*) FROM NewsStories WHERE IsPublished = 1) AS TotalNews,
                            (SELECT COUNT(*) FROM Feedbacks WHERE IsDeleted = 0) AS TotalFeedbacks,
                            (SELECT ISNULL(ROUND(AVG(CAST(Rating AS FLOAT)), 1), 0) FROM Feedbacks WHERE IsDeleted = 0) AS AverageRating,
                            (SELECT COUNT(DISTINCT State) FROM NewsStories WHERE State IS NOT NULL AND IsPublished = 1) AS TotalStates,
                            (SELECT COUNT(DISTINCT Language) FROM NewsStories WHERE Language IS NOT NULL AND IsPublished = 1) AS TotalLanguages;
                    END;
                ");

                // 4. Stored Procedure: sp_GetRegionalFeedbackAnalytics
                await _context.Database.ExecuteSqlRawAsync(@"
                    IF OBJECT_ID('sp_GetRegionalFeedbackAnalytics', 'P') IS NOT NULL DROP PROCEDURE sp_GetRegionalFeedbackAnalytics;
                ");
                await _context.Database.ExecuteSqlRawAsync(@"
                    CREATE PROCEDURE sp_GetRegionalFeedbackAnalytics
                        @State NVARCHAR(100) = NULL
                    AS
                    BEGIN
                        SET NOCOUNT ON;
                        SELECT 
                            n.State,
                            COUNT(DISTINCT n.Id) AS NewsCount,
                            COUNT(f.Id) AS FeedbackCount,
                            ISNULL(ROUND(AVG(CAST(f.Rating AS FLOAT)), 2), 0) AS AverageRating
                        FROM NewsStories n
                        LEFT JOIN Feedbacks f ON n.Id = f.NewsStoryId AND f.IsDeleted = 0
                        WHERE n.IsPublished = 1
                          AND (@State IS NULL OR n.State = @State)
                          AND n.State IS NOT NULL
                        GROUP BY n.State
                        ORDER BY FeedbackCount DESC;
                    END;
                ");
            }
            catch
            {
                // Silently ignore if raw DDL is restricted in current connection mode
            }
        }

        private async Task SeedRolesAsync()
        {
            string[] roles = { "Admin", "Citizen" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedAdminUserAsync()
        {
            if (await _userManager.FindByEmailAsync("admin@govfeedback.in") != null) return;

            var admin = new ApplicationUser
            {
                UserName = "admin@govfeedback.in",
                Email = "admin@govfeedback.in",
                FullName = "Platform Administrator",
                State = "Delhi",
                Language = "English",
                EmailConfirmed = true,
                IsActive = true
            };
            var result = await _userManager.CreateAsync(admin, "Admin@123456");
            if (result.Succeeded)
                await _userManager.AddToRoleAsync(admin, "Admin");
        }

        private async Task SeedUsersAsync()
        {
            if (await _context.Users.CountAsync() >= 5) return;

            var users = new[]
            {
                new { Name="Priya Sharma", Email="priya.sharma@example.com", State="Maharashtra", Lang="Hindi" },
                new { Name="Rahul Verma", Email="rahul.verma@example.com", State="Uttar Pradesh", Lang="Hindi" },
                new { Name="Anitha Reddy", Email="anitha.reddy@example.com", State="Andhra Pradesh", Lang="Telugu" },
                new { Name="Suresh Kumar", Email="suresh.kumar@example.com", State="Tamil Nadu", Lang="Tamil" },
                new { Name="Meena Pillai", Email="meena.pillai@example.com", State="Kerala", Lang="Malayalam" },
                new { Name="Amit Singh", Email="amit.singh@example.com", State="Punjab", Lang="Punjabi" },
                new { Name="Kavya Nair", Email="kavya.nair@example.com", State="Karnataka", Lang="Kannada" },
                new { Name="Deepak Joshi", Email="deepak.joshi@example.com", State="Rajasthan", Lang="Hindi" },
                new { Name="Sunita Das", Email="sunita.das@example.com", State="West Bengal", Lang="Bengali" },
                new { Name="Ravi Patel", Email="ravi.patel@example.com", State="Gujarat", Lang="Gujarati" },
                new { Name="Fatima Khan", Email="fatima.khan@example.com", State="Telangana", Lang="Urdu" },
                new { Name="Nikhil Rao", Email="nikhil.rao@example.com", State="Maharashtra", Lang="Marathi" },
                new { Name="Pooja Gupta", Email="pooja.gupta@example.com", State="Madhya Pradesh", Lang="Hindi" },
                new { Name="Arjun Mehta", Email="arjun.mehta@example.com", State="Delhi", Lang="English" },
                new { Name="Lakshmi Iyer", Email="lakshmi.iyer@example.com", State="Tamil Nadu", Lang="Tamil" },
                new { Name="Vinod Tiwari", Email="vinod.tiwari@example.com", State="Bihar", Lang="Hindi" },
                new { Name="Swati Bhatt", Email="swati.bhatt@example.com", State="Uttarakhand", Lang="Hindi" },
                new { Name="Gaurav Desai", Email="gaurav.desai@example.com", State="Gujarat", Lang="Gujarati" },
                new { Name="Reena Chaudhary", Email="reena.chaudhary@example.com", State="Haryana", Lang="Hindi" }
            };

            foreach (var u in users)
            {
                if (await _userManager.FindByEmailAsync(u.Email) != null) continue;
                var user = new ApplicationUser
                {
                    UserName = u.Email,
                    Email = u.Email,
                    FullName = u.Name,
                    State = u.State,
                    Language = u.Lang,
                    EmailConfirmed = true,
                    IsActive = true
                };
                var result = await _userManager.CreateAsync(user, "Citizen@123456");
                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(user, "Citizen");
            }
        }

        private async Task SeedNewsAsync()
        {
            if (await _context.NewsStories.AnyAsync()) return;

            var adminUser = await _userManager.FindByEmailAsync("admin@govfeedback.in");
            var adminId = adminUser?.Id;

            var now = DateTime.UtcNow;

            var stories = new List<NewsStory>
            {
                new NewsStory { Title="National Highway Expansion: 500 km New Roads Approved in Phase IV", Content="The Ministry of Road Transport and Highways has approved construction of 500 km of new national highways under Phase IV of Bharatmala Pariyojana, connecting remote districts to major urban centres.", Summary="500 km of highways approved under Bharatmala Phase IV, boosting connectivity.", CategoryId=1, State="Maharashtra", Language="English", IsPublished=true, PublishedDate=now.AddDays(-1), CreatedByUserId=adminId, Tags="infrastructure,highway,connectivity" },
                new NewsStory { Title="Ayushman Bharat Covers 10 Crore Families: PM Health Scheme Milestone", Content="The Pradhan Mantri Jan Arogya Yojana (PM-JAY) has successfully enrolled 10 crore families, providing free health coverage up to Rs 5 lakh per family per year across government and empanelled private hospitals.", Summary="PM-JAY achieves 10 crore family enrollment milestone.", CategoryId=2, State="Delhi", Language="English", IsPublished=true, PublishedDate=now.AddDays(-2), CreatedByUserId=adminId, Tags="healthcare,health,ayushman" },
                new NewsStory { Title="PM SHRI Schools Scheme: 14,500 Schools Upgraded Across India", Content="Under the PM SHRI (PM Schools for Rising India) scheme, 14,500 schools have been upgraded with modern infrastructure, digital labs, and skill development centres in all 28 states and 8 union territories.", Summary="14,500 schools modernised under PM SHRI scheme.", CategoryId=3, State=null, Language="English", IsPublished=true, PublishedDate=now.AddDays(-3), CreatedByUserId=adminId, Tags="education,school,development" },
                new NewsStory { Title="PM Kisan Samman: 18th Instalment of Rs 2000 Released to 9 Crore Farmers", Content="The 18th instalment under PM-Kisan Samman Nidhi scheme has been released directly to the bank accounts of over 9 crore farmers, transferring Rs 18,000 crore via Direct Benefit Transfer.", Summary="PM-Kisan 18th instalment released to 9 crore farmers.", CategoryId=4, State="Uttar Pradesh", Language="Hindi", IsPublished=true, PublishedDate=now.AddDays(-4), CreatedByUserId=adminId, Tags="agriculture,farmer,welfare" },
                new NewsStory { Title="Union Budget 2025-26: Capital Expenditure Set at Record Rs 11.11 Lakh Crore", Content="Finance Minister presented the Union Budget 2025-26 with capital expenditure allocation of Rs 11.11 lakh crore, targeting infrastructure growth, digital economy and green energy transition.", Summary="Record capex of Rs 11.11 lakh crore in Union Budget 2025-26.", CategoryId=5, State=null, Language="English", IsPublished=true, PublishedDate=now.AddDays(-5), CreatedByUserId=adminId, Tags="economy,budget,finance" },
                new NewsStory { Title="Agnipath Scheme: First Batch of Agniveers Complete Training", Content="The first batch of 46,000 Agniveers has successfully completed their training across the Indian Army, Navy and Air Force, marking a significant milestone in defence recruitment reform under Agnipath.", Summary="46,000 Agniveers complete training under Agnipath scheme.", CategoryId=6, State=null, Language="English", IsPublished=true, PublishedDate=now.AddDays(-6), CreatedByUserId=adminId, Tags="defence,agniveer,army" },
                new NewsStory { Title="National Clean Air Programme: 95 Cities Show Improvement in Air Quality", Content="The National Clean Air Programme (NCAP) reports that 95 of the 131 non-attainment cities have shown measurable reduction in PM2.5 and PM10 levels, with Delhi recording 15% improvement year-on-year.", Summary="95 cities show air quality improvement under NCAP.", CategoryId=7, State="Delhi", Language="English", IsPublished=true, PublishedDate=now.AddDays(-7), CreatedByUserId=adminId, Tags="environment,air,pollution" },
                new NewsStory { Title="Jan Dhan Yojana: 53 Crore Bank Accounts Opened, Deposits Cross Rs 2.3 Lakh Crore", Content="Pradhan Mantri Jan Dhan Yojana has facilitated financial inclusion with 53 crore bank accounts, total deposits exceeding Rs 2.3 lakh crore and over 36 crore RuPay debit cards issued.", Summary="PMJDY reaches 53 crore accounts with Rs 2.3 lakh crore deposits.", CategoryId=8, State=null, Language="English", IsPublished=true, PublishedDate=now.AddDays(-8), CreatedByUserId=adminId, Tags="welfare,banking,inclusion" },
                new NewsStory { Title="Digital India: 5G Rollout Crosses 7 Lakh Base Stations Nationwide", Content="India's 5G network expansion has surpassed 7 lakh base stations across 700+ cities, with Jio and Airtel leading the rollout. The government targets 100% coverage of tier-2 and tier-3 cities by 2026.", Summary="5G crosses 7 lakh base stations across 700+ Indian cities.", CategoryId=9, State=null, Language="English", IsPublished=true, PublishedDate=now.AddDays(-9), CreatedByUserId=adminId, Tags="technology,5G,digital" },
                new NewsStory { Title="India-ASEAN Free Trade Agreement Revised: Tariff Cuts on 85% of Goods", Content="India and ASEAN member states have agreed on revised terms for the Free Trade Agreement, reducing tariffs on 85% of goods, expected to boost bilateral trade to USD 200 billion by 2027.", Summary="India-ASEAN FTA revised with 85% tariff reduction on goods.", CategoryId=10, State=null, Language="English", IsPublished=true, PublishedDate=now.AddDays(-10), CreatedByUserId=adminId, Tags="foreignpolicy,trade,ASEAN" },
                new NewsStory { Title="Tamil Nadu Metro Rail Phase 2: Rs 63,246 Crore Project Approved by Cabinet", Content="The Union Cabinet has approved Phase 2 of Chennai Metro Rail at a cost of Rs 63,246 crore, covering 118.9 km across three corridors and expected to serve 11 lakh passengers daily.", Summary="Chennai Metro Phase 2 worth Rs 63,246 crore approved.", CategoryId=1, State="Tamil Nadu", Language="Tamil", IsPublished=true, PublishedDate=now.AddDays(-11), CreatedByUserId=adminId, Tags="infrastructure,metro,transport" },
                new NewsStory { Title="National Medical Devices Policy 2025: India Targets Self-Reliance in Healthcare Equipment", Content="The new National Medical Devices Policy 2025 aims to make India self-reliant in medical device manufacturing, reduce import dependency from 70% to 30% by 2030, and promote exports worth USD 10 billion.", Summary="India targets medical device self-reliance under new policy.", CategoryId=2, State=null, Language="English", IsPublished=true, PublishedDate=now.AddDays(-12), CreatedByUserId=adminId, Tags="healthcare,medtech,manufacturing" },
                new NewsStory { Title="National Education Policy: 50% Gross Enrolment Ratio in Higher Education by 2035", Content="Implementation report on NEP 2020 reveals significant progress with Gross Enrolment Ratio in higher education reaching 28.4%, with a roadmap to achieve 50% by 2035 through new IITs, IIMs and central universities.", Summary="NEP 2020 implementation shows GER progress toward 50% target.", CategoryId=3, State=null, Language="English", IsPublished=true, PublishedDate=now.AddDays(-13), CreatedByUserId=adminId, Tags="education,NEP,highereducation" },
                new NewsStory { Title="Punjab Farmers Protest Suspended: Government Accepts MSP Committee Recommendations", Content="Farmers in Punjab have suspended their ongoing protest after the government accepted key recommendations of the MSP committee, including guaranteed procurement and enhanced crop insurance under PMFBY.", Summary="Punjab farmers protest ends as government accepts MSP recommendations.", CategoryId=4, State="Punjab", Language="Punjabi", IsPublished=true, PublishedDate=now.AddDays(-14), CreatedByUserId=adminId, Tags="agriculture,MSP,protest" },
                new NewsStory { Title="GST Revenue Hits Record Rs 2.37 Lakh Crore in March 2025", Content="Gross GST collection crossed Rs 2.37 lakh crore in March 2025, the highest since the tax's introduction in 2017, reflecting robust economic activity and improved compliance through faceless assessment and AI-based audit tools.", Summary="GST collection hits all-time high of Rs 2.37 lakh crore in March 2025.", CategoryId=5, State=null, Language="English", IsPublished=true, PublishedDate=now.AddDays(-15), CreatedByUserId=adminId, Tags="economy,GST,revenue" },
            };

            await _context.NewsStories.AddRangeAsync(stories);
            await _context.SaveChangesAsync();
        }

        private async Task SeedFeedbacksAsync()
        {
            if (await _context.Feedbacks.AnyAsync()) return;

            var users = await _context.Users
                .Where(u => u.Email != "admin@govfeedback.in")
                .ToListAsync();

            var news = await _context.NewsStories.ToListAsync();

            if (users.Count == 0 || news.Count == 0) return;

            var rnd = new Random(42);
            var sentiments = Enum.GetValues<SentimentType>();
            var comments = new[]
            {
                "This is excellent progress by the government. The development initiative is very helpful for citizens.",
                "Great achievement! This scheme shows the government's commitment to welfare and improvement.",
                "The implementation seems delayed and inadequate. Many areas are still not covered.",
                "Disappointing progress. The benefits have not reached the poor and mismanaged funds are a concern.",
                "Overall a positive step, though more transparent execution would be beneficial.",
                "Very effective policy. I have seen progress in my state and it has been quite helpful.",
                "The scheme is good but needs better monitoring and efficient distribution.",
                "This is a waste of taxpayer money. The corrupt officials are not accountable.",
                "Improvement is visible but slow. The government needs to be more efficient.",
                "Excellent initiative! This will benefit millions and lead to great development across states.",
                "Failed to reach villages. Poor planning and inadequate budget allocation.",
                "A transparent and effective approach. The achievement is commendable.",
                "Progress is positive. This is a good step toward national development and welfare.",
                "The policy is controversial and negative for farmers in border states.",
                "Beneficial for urban areas but rural communities still face inadequate access."
            };

            var feedbacks = new List<Feedback>();
            var added = new HashSet<(string, int)>();

            for (int i = 0; i < 55; i++)
            {
                var user = users[rnd.Next(users.Count)];
                var story = news[rnd.Next(news.Count)];
                var key = (user.Id, story.Id);
                if (added.Contains(key)) continue;
                added.Add(key);

                var sent = sentiments[rnd.Next(sentiments.Length)];
                var comment = comments[rnd.Next(comments.Length)];
                feedbacks.Add(new Feedback
                {
                    UserId = user.Id,
                    NewsStoryId = story.Id,
                    Rating = rnd.Next(1, 6),
                    UserSentiment = sent,
                    DetectedSentiment = sent,
                    Comment = comment,
                    Suggestions = "More transparency and efficient implementation needed.",
                    FeedbackCategory = "General",
                    IsModerated = rnd.Next(2) == 0,
                    IsApproved = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-rnd.Next(0, 30)),
                    UpdatedAt = DateTime.UtcNow.AddDays(-rnd.Next(0, 10))
                });
            }

            await _context.Feedbacks.AddRangeAsync(feedbacks);
            await _context.SaveChangesAsync();
        }
    }
}
