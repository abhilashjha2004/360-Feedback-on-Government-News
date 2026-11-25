using GovernmentFeedbackPlatform.Data;
using GovernmentFeedbackPlatform.Services;
using GovernmentFeedbackPlatform.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GovernmentFeedbackPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ApplicationDbContext _context;

        public AnalyticsController(IAnalyticsService analyticsService, ApplicationDbContext context)
        {
            _analyticsService = analyticsService;
            _context = context;
        }

        public async Task<IActionResult> Index(string? filterState, int? filterCategoryId, DateTime? dateFrom, DateTime? dateTo)
        {
            var stats = await _analyticsService.GetPlatformStatsAsync();
            var categoryStats = await _analyticsService.GetCategoryStatsAsync(filterCategoryId, filterState, dateFrom, dateTo);
            var regionalStats = await _analyticsService.GetRegionalStatsAsync(filterState, dateFrom, dateTo);
            var monthlyTrends = await _analyticsService.GetMonthlyTrendsAsync(12);
            var sentiment = await _analyticsService.GetSentimentSummaryAsync(filterState, filterCategoryId, dateFrom, dateTo);
            var ratingDist = await _analyticsService.GetRatingDistributionAsync(filterState, filterCategoryId, dateFrom, dateTo);
            var categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();

            var vm = new AnalyticsViewModel
            {
                OverallStats = stats,
                CategoryStats = categoryStats,
                RegionalStats = regionalStats,
                MonthlyTrends = monthlyTrends,
                TotalPositive = sentiment.positive,
                TotalNeutral = sentiment.neutral,
                TotalNegative = sentiment.negative,
                RatingDistribution = ratingDist,
                FilterState = filterState,
                FilterCategoryId = filterCategoryId,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Categories = categories
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> FilteredUpdate(string? filterState, int? filterCategoryId, DateTime? dateFrom, DateTime? dateTo)
        {
            var categoryStats = await _analyticsService.GetCategoryStatsAsync(filterCategoryId, filterState, dateFrom, dateTo);
            var regionalStats = await _analyticsService.GetRegionalStatsAsync(filterState, dateFrom, dateTo);
            var sentiment = await _analyticsService.GetSentimentSummaryAsync(filterState, filterCategoryId, dateFrom, dateTo);

            return Json(new
            {
                categoryStats,
                regionalStats,
                positive = sentiment.positive,
                neutral = sentiment.neutral,
                negative = sentiment.negative
            });
        }
    }
}
