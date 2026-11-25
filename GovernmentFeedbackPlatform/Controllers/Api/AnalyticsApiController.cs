using GovernmentFeedbackPlatform.Services;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentFeedbackPlatform.Controllers.Api
{
    [ApiController]
    [Route("api/analytics")]
    [Produces("application/json")]
    public class AnalyticsApiController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsApiController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        /// <summary>GET /api/analytics/overview – Platform KPIs</summary>
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var stats = await _analyticsService.GetPlatformStatsAsync();
            return Ok(stats);
        }

        /// <summary>GET /api/analytics/sentiment – Sentiment breakdown with optional state and category filters</summary>
        [HttpGet("sentiment")]
        public async Task<IActionResult> GetSentiment([FromQuery] string? state, [FromQuery] int? categoryId,
            [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var (positive, neutral, negative) = await _analyticsService.GetSentimentSummaryAsync(state, categoryId, from, to);
            int total = positive + neutral + negative;
            return Ok(new
            {
                positive,
                neutral,
                negative,
                total,
                positivePercent = total > 0 ? Math.Round((double)positive / total * 100, 1) : 0,
                neutralPercent = total > 0 ? Math.Round((double)neutral / total * 100, 1) : 0,
                negativePercent = total > 0 ? Math.Round((double)negative / total * 100, 1) : 0
            });
        }

        /// <summary>GET /api/analytics/regional – State-by-state feedback statistics</summary>
        [HttpGet("regional")]
        public async Task<IActionResult> GetRegional([FromQuery] string? state, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var stats = await _analyticsService.GetRegionalStatsAsync(state, from, to);
            return Ok(stats);
        }

        /// <summary>GET /api/analytics/monthly – Monthly trends for past N months</summary>
        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthly([FromQuery] int months = 6)
        {
            if (months < 1 || months > 24) months = 6;
            var trends = await _analyticsService.GetMonthlyTrendsAsync(months);
            return Ok(trends);
        }
    }
}
