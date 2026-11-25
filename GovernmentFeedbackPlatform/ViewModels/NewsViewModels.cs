using GovernmentFeedbackPlatform.Models;

namespace GovernmentFeedbackPlatform.ViewModels
{
    public class NewsListViewModel
    {
        public List<NewsStory> NewsStories { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public string? State { get; set; }
        public string? Language { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; } = 12;
    }

    public class NewsDetailViewModel
    {
        public NewsStory? NewsStory { get; set; }
        public List<Feedback> RecentFeedbacks { get; set; } = new();
        public FeedbackStatsViewModel? Stats { get; set; }
        public FeedbackFormViewModel? FeedbackForm { get; set; }
        public bool UserAlreadySubmitted { get; set; }
        public Feedback? UserFeedback { get; set; }
    }

    public class FeedbackStatsViewModel
    {
        public int TotalFeedbacks { get; set; }
        public double AverageRating { get; set; }
        public int PositiveCount { get; set; }
        public int NeutralCount { get; set; }
        public int NegativeCount { get; set; }
        public double PositivePercentage => TotalFeedbacks > 0 ? Math.Round((double)PositiveCount / TotalFeedbacks * 100, 1) : 0;
        public double NeutralPercentage => TotalFeedbacks > 0 ? Math.Round((double)NeutralCount / TotalFeedbacks * 100, 1) : 0;
        public double NegativePercentage => TotalFeedbacks > 0 ? Math.Round((double)NegativeCount / TotalFeedbacks * 100, 1) : 0;
        public Dictionary<int, int> RatingDistribution { get; set; } = new();
    }

    public class FeedbackFormViewModel
    {
        public int NewsStoryId { get; set; }
        public string? NewsStoryTitle { get; set; }
        public int Rating { get; set; } = 3;
        public string UserSentiment { get; set; } = "Neutral";
        public string? Comment { get; set; }
        public string? Suggestions { get; set; }
        public string? FeedbackCategory { get; set; }
    }

    public class HomeViewModel
    {
        public List<NewsStory> LatestNews { get; set; } = new();
        public List<NewsStory> FeaturedNews { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public Dictionary<string, List<NewsStory>> RegionalNews { get; set; } = new();
        public PlatformStats Stats { get; set; } = new();
    }

    public class PlatformStats
    {
        public int TotalUsers { get; set; }
        public int TotalNews { get; set; }
        public int TotalNewsStories { get => TotalNews; set => TotalNews = value; }
        public int TotalFeedbacks { get; set; }
        public double AverageRating { get; set; }
        public int PositiveFeedbacks { get; set; }
        public int NeutralFeedbacks { get; set; }
        public int NegativeFeedbacks { get; set; }
        public int TotalStates { get; set; }
        public int TotalLanguages { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public PlatformStats Stats { get; set; } = new();
        public List<NewsStory> RecentNews { get; set; } = new();
        public List<Feedback> RecentFeedbacks { get; set; } = new();
        public List<CategoryFeedbackStat> CategoryStats { get; set; } = new();
        public List<RegionalStat> RegionalStats { get; set; } = new();
        public List<MonthlyTrend> MonthlyTrends { get; set; } = new();
    }

    public class CategoryFeedbackStat
    {
        public string CategoryName { get; set; } = string.Empty;
        public int FeedbackCount { get; set; }
        public double AverageRating { get; set; }
    }

    public class RegionalStat
    {
        public string State { get; set; } = string.Empty;
        public int FeedbackCount { get; set; }
        public int NewsCount { get; set; }
        public double AverageRating { get; set; }
    }

    public class MonthlyTrend
    {
        public string Month { get; set; } = string.Empty;
        public int FeedbackCount { get; set; }
        public int NewsCount { get; set; }
    }

    public class AnalyticsViewModel
    {
        public PlatformStats OverallStats { get; set; } = new();
        public List<CategoryFeedbackStat> CategoryStats { get; set; } = new();
        public List<RegionalStat> RegionalStats { get; set; } = new();
        public List<MonthlyTrend> MonthlyTrends { get; set; } = new();
        public int TotalPositive { get; set; }
        public int TotalNeutral { get; set; }
        public int TotalNegative { get; set; }
        public Dictionary<int, int> RatingDistribution { get; set; } = new();

        // Filter params
        public string? FilterState { get; set; }
        public int? FilterCategoryId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public List<Category> Categories { get; set; } = new();
    }

    public class FeedbackManagementViewModel
    {
        public List<Feedback> Feedbacks { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? FilterSentiment { get; set; }
        public bool? FilterModerated { get; set; }
        public int? FilterNewsId { get; set; }
    }

    public class CitizenDashboardViewModel
    {
        public ApplicationUser? User { get; set; }
        public List<Feedback> RecentFeedbacks { get; set; } = new();
        public int TotalFeedbacks { get; set; }
        public double AverageRating { get; set; }
        public int PositiveCount { get; set; }
        public int NeutralCount { get; set; }
        public int NegativeCount { get; set; }
        public List<NewsStory> RecentNews { get; set; } = new();
    }
}
