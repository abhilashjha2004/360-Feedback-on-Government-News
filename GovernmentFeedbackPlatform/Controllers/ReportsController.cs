using System.Text;
using GovernmentFeedbackPlatform.Data;
using GovernmentFeedbackPlatform.Models;
using GovernmentFeedbackPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GovernmentFeedbackPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ApplicationDbContext _context;

        public ReportsController(IAnalyticsService analyticsService, ApplicationDbContext context)
        {
            _analyticsService = analyticsService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _analyticsService.GetPlatformStatsAsync();
            var categoryStats = await _analyticsService.GetCategoryStatsAsync(null, null, null, null);
            var regionalStats = await _analyticsService.GetRegionalStatsAsync(null, null, null);
            var monthly = await _analyticsService.GetMonthlyTrendsAsync(12);
            var sentiment = await _analyticsService.GetSentimentSummaryAsync(null, null, null, null);

            // Real Top 5 Most Discussed News
            var topNews = await _context.NewsStories
                .Include(n => n.Category)
                .Include(n => n.Feedbacks)
                .Where(n => n.IsPublished)
                .Select(n => new
                {
                    n.Id,
                    n.Title,
                    Category = n.Category != null ? n.Category.Name : "General",
                    State = n.State ?? "National",
                    FeedbacksCount = n.Feedbacks.Count(f => !f.IsDeleted),
                    AvgRating = n.Feedbacks.Where(f => !f.IsDeleted).Any() ? Math.Round(n.Feedbacks.Where(f => !f.IsDeleted).Average(f => f.Rating), 1) : 0
                })
                .OrderByDescending(x => x.FeedbacksCount)
                .Take(5)
                .ToListAsync();

            ViewBag.Stats = stats;
            ViewBag.CategoryStats = categoryStats;
            ViewBag.RegionalStats = regionalStats;
            ViewBag.Monthly = monthly;
            ViewBag.Positive = sentiment.positive;
            ViewBag.Neutral = sentiment.neutral;
            ViewBag.Negative = sentiment.negative;
            ViewBag.TopNews = topNews;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ExportNewsCsv()
        {
            var news = await _context.NewsStories
                .Include(n => n.Category)
                .Include(n => n.Feedbacks)
                .OrderByDescending(n => n.PublishedDate)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Id,Title,Category,State,Language,MediaSource,PublishedDate,IsPublished,FeedbackCount,AverageRating");

            foreach (var n in news)
            {
                var fbCount = n.Feedbacks.Count(f => !f.IsDeleted);
                var avgRating = fbCount > 0 ? Math.Round(n.Feedbacks.Where(f => !f.IsDeleted).Average(f => f.Rating), 1) : 0;
                var safeTitle = $"\"{n.Title.Replace("\"", "\"\"")}\"";
                var cat = n.Category?.Name ?? "General";
                var state = n.State ?? "National";
                var lang = n.Language ?? "English";
                var media = n.MediaSource ?? "Regional Media";
                var date = n.PublishedDate.ToString("yyyy-MM-dd");

                sb.AppendLine($"{n.Id},{safeTitle},{cat},{state},{lang},{media},{date},{n.IsPublished},{fbCount},{avgRating}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"govfeedback_news_report_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        [HttpGet]
        public async Task<IActionResult> ExportFeedbackCsv()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.NewsStory)
                .Include(f => f.User)
                .Where(f => !f.IsDeleted)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Id,NewsStoryTitle,CitizenState,Rating,UserSentiment,DetectedSentiment,Category,Comment,Suggestions,SubmittedDate");

            foreach (var f in feedbacks)
            {
                var title = $"\"{(f.NewsStory?.Title ?? "—").Replace("\"", "\"\"")}\"";
                var state = f.User?.State ?? "Unknown";
                var comment = $"\"{(f.Comment ?? "").Replace("\"", "\"\"")}\"";
                var suggestions = $"\"{(f.Suggestions ?? "").Replace("\"", "\"\"")}\"";
                var cat = f.FeedbackCategory ?? "General";
                var date = f.CreatedAt.ToString("yyyy-MM-dd HH:mm");

                sb.AppendLine($"{f.Id},{title},{state},{f.Rating},{f.UserSentiment},{f.DetectedSentiment},{cat},{comment},{suggestions},{date}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"govfeedback_feedback_report_{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        [HttpGet]
        public async Task<IActionResult> ExportUsersCsv()
        {
            var users = await _context.Users
                .Include(u => u.Feedbacks)
                .OrderBy(u => u.FullName)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("Id,FullName,Email,State,Language,AgeGroup,Occupation,FeedbacksGiven,JoinedDate");

            foreach (var u in users)
            {
                var name = $"\"{u.FullName.Replace("\"", "\"\"")}\"";
                var state = u.State ?? "National";
                var lang = u.Language ?? "English";
                var age = u.AgeGroup ?? "—";
                var occ = $"\"{(u.Occupation ?? "Citizen").Replace("\"", "\"\"")}\"";
                var count = u.Feedbacks.Count(f => !f.IsDeleted);
                var date = u.CreatedAt.ToString("yyyy-MM-dd");

                sb.AppendLine($"{u.Id},{name},{u.Email},{state},{lang},{age},{occ},{count},{date}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"govfeedback_users_report_{DateTime.UtcNow:yyyyMMdd}.csv");
        }
    }
}
