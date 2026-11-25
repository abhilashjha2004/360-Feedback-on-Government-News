using GovernmentFeedbackPlatform.Data;
using GovernmentFeedbackPlatform.Models;
using GovernmentFeedbackPlatform.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GovernmentFeedbackPlatform.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PlatformStats> GetPlatformStatsAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalNews = await _context.NewsStories.CountAsync(n => n.IsPublished);
            var feedbacks = await _context.Feedbacks.Where(f => !f.IsDeleted).ToListAsync();
            var states = await _context.NewsStories.Where(n => n.State != null).Select(n => n.State!).Distinct().CountAsync();
            var languages = await _context.NewsStories.Where(n => n.Language != null).Select(n => n.Language!).Distinct().CountAsync();

            int positive = feedbacks.Count(f => f.DetectedSentiment == SentimentType.Positive || f.UserSentiment == SentimentType.Positive);
            int negative = feedbacks.Count(f => f.DetectedSentiment == SentimentType.Negative || f.UserSentiment == SentimentType.Negative);
            int neutral = feedbacks.Count - positive - negative;
            if (neutral < 0) neutral = 0;

            return new PlatformStats
            {
                TotalUsers = totalUsers,
                TotalNews = totalNews,
                TotalFeedbacks = feedbacks.Count,
                AverageRating = feedbacks.Count > 0 ? Math.Round(feedbacks.Average(f => f.Rating), 1) : 0,
                PositiveFeedbacks = positive,
                NeutralFeedbacks = neutral,
                NegativeFeedbacks = negative,
                TotalStates = states,
                TotalLanguages = languages
            };
        }

        public async Task<List<CategoryFeedbackStat>> GetCategoryStatsAsync(int? categoryId, string? state, DateTime? from, DateTime? to)
        {
            var q = _context.Feedbacks
                .Include(f => f.NewsStory)
                    .ThenInclude(n => n!.Category)
                .Where(f => !f.IsDeleted)
                .AsQueryable();

            if (categoryId.HasValue)
                q = q.Where(f => f.NewsStory!.CategoryId == categoryId.Value);
            if (!string.IsNullOrWhiteSpace(state))
                q = q.Where(f => f.NewsStory!.State == state);
            if (from.HasValue)
                q = q.Where(f => f.CreatedAt >= from.Value);
            if (to.HasValue)
                q = q.Where(f => f.CreatedAt <= to.Value);

            var data = await q.ToListAsync();

            return data
                .GroupBy(f => f.NewsStory?.Category?.Name ?? "Unknown")
                .Select(g => new CategoryFeedbackStat
                {
                    CategoryName = g.Key,
                    FeedbackCount = g.Count(),
                    AverageRating = g.Count() > 0 ? Math.Round(g.Average(f => f.Rating), 1) : 0
                })
                .OrderByDescending(c => c.FeedbackCount)
                .ToList();
        }

        public async Task<List<RegionalStat>> GetRegionalStatsAsync(string? state, DateTime? from, DateTime? to)
        {
            var feedbackQ = _context.Feedbacks
                .Include(f => f.NewsStory)
                .Where(f => !f.IsDeleted && f.NewsStory != null && f.NewsStory.State != null)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(state))
                feedbackQ = feedbackQ.Where(f => f.NewsStory!.State == state);
            if (from.HasValue)
                feedbackQ = feedbackQ.Where(f => f.CreatedAt >= from.Value);
            if (to.HasValue)
                feedbackQ = feedbackQ.Where(f => f.CreatedAt <= to.Value);

            var feedbacks = await feedbackQ.ToListAsync();

            var newsQ = _context.NewsStories
                .Where(n => n.IsPublished && n.State != null)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(state))
                newsQ = newsQ.Where(n => n.State == state);

            var newsByState = await newsQ
                .GroupBy(n => n.State!)
                .Select(g => new { State = g.Key, Count = g.Count() })
                .ToListAsync();

            var newsDict = newsByState.ToDictionary(x => x.State, x => x.Count);

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
        }

        public async Task<List<MonthlyTrend>> GetMonthlyTrendsAsync(int months)
        {
            var from = DateTime.UtcNow.AddMonths(-months + 1);
            from = new DateTime(from.Year, from.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var feedbacks = await _context.Feedbacks
                .Where(f => !f.IsDeleted && f.CreatedAt >= from)
                .ToListAsync();

            var news = await _context.NewsStories
                .Where(n => n.IsPublished && n.PublishedDate >= from)
                .ToListAsync();

            var trends = new List<MonthlyTrend>();
            for (int i = 0; i < months; i++)
            {
                var month = from.AddMonths(i);
                var label = month.ToString("MMM yyyy");
                trends.Add(new MonthlyTrend
                {
                    Month = label,
                    FeedbackCount = feedbacks.Count(f => f.CreatedAt.Year == month.Year && f.CreatedAt.Month == month.Month),
                    NewsCount = news.Count(n => n.PublishedDate.Year == month.Year && n.PublishedDate.Month == month.Month)
                });
            }
            return trends;
        }

        public async Task<(int positive, int neutral, int negative)> GetSentimentSummaryAsync(string? state, int? categoryId, DateTime? from, DateTime? to)
        {
            var q = _context.Feedbacks
                .Include(f => f.NewsStory)
                .Where(f => !f.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(state))
                q = q.Where(f => f.NewsStory!.State == state);
            if (categoryId.HasValue)
                q = q.Where(f => f.NewsStory!.CategoryId == categoryId.Value);
            if (from.HasValue)
                q = q.Where(f => f.CreatedAt >= from.Value);
            if (to.HasValue)
                q = q.Where(f => f.CreatedAt <= to.Value);

            var feedbacks = await q.ToListAsync();

            int positive = feedbacks.Count(f => f.DetectedSentiment == SentimentType.Positive);
            int neutral = feedbacks.Count(f => f.DetectedSentiment == SentimentType.Neutral || f.DetectedSentiment == null);
            int negative = feedbacks.Count(f => f.DetectedSentiment == SentimentType.Negative);

            return (positive, neutral, negative);
        }

        public async Task<Dictionary<int, int>> GetRatingDistributionAsync(string? state, int? categoryId, DateTime? from, DateTime? to)
        {
            var q = _context.Feedbacks
                .Include(f => f.NewsStory)
                .Where(f => !f.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(state))
                q = q.Where(f => f.NewsStory!.State == state);
            if (categoryId.HasValue)
                q = q.Where(f => f.NewsStory!.CategoryId == categoryId.Value);
            if (from.HasValue)
                q = q.Where(f => f.CreatedAt >= from.Value);
            if (to.HasValue)
                q = q.Where(f => f.CreatedAt <= to.Value);

            var ratings = await q.Select(f => f.Rating).ToListAsync();
            var dist = new Dictionary<int, int>();
            for (int r = 1; r <= 5; r++)
            {
                dist[r] = ratings.Count(x => x == r);
            }
            return dist;
        }
    }
}
