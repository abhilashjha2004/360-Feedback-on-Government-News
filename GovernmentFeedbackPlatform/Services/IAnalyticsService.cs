using GovernmentFeedbackPlatform.ViewModels;

namespace GovernmentFeedbackPlatform.Services
{
    public interface IAnalyticsService
    {
        Task<PlatformStats> GetPlatformStatsAsync();
        Task<List<CategoryFeedbackStat>> GetCategoryStatsAsync(int? categoryId, string? state, DateTime? from, DateTime? to);
        Task<List<RegionalStat>> GetRegionalStatsAsync(string? state, DateTime? from, DateTime? to);
        Task<List<MonthlyTrend>> GetMonthlyTrendsAsync(int months);
        Task<(int positive, int neutral, int negative)> GetSentimentSummaryAsync(string? state, int? categoryId, DateTime? from, DateTime? to);
        Task<Dictionary<int, int>> GetRatingDistributionAsync(string? state, int? categoryId, DateTime? from, DateTime? to);
    }
}
