using GovernmentFeedbackPlatform.Data;
using GovernmentFeedbackPlatform.Services;
using GovernmentFeedbackPlatform.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GovernmentFeedbackPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly INewsService _newsService;
        private readonly IAnalyticsService _analyticsService;
        private readonly ApplicationDbContext _context;

        public HomeController(INewsService newsService, IAnalyticsService analyticsService, ApplicationDbContext context)
        {
            _newsService = newsService;
            _analyticsService = analyticsService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var latestNews = await _newsService.GetLatestNewsAsync(6);
            var featuredNews = await _newsService.GetLatestNewsAsync(3);
            var categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            var stats = await _analyticsService.GetPlatformStatsAsync();

            // Regional news: top 4 states
            var topStates = new[] { "Maharashtra", "Uttar Pradesh", "Tamil Nadu", "Delhi" };
            var regionalNews = new Dictionary<string, List<Models.NewsStory>>();
            foreach (var s in topStates)
            {
                var stateNews = await _newsService.GetPublishedNewsAsync(null, null, s, null, 1, 3);
                if (stateNews.Any()) regionalNews[s] = stateNews;
            }

            var vm = new HomeViewModel
            {
                LatestNews = latestNews,
                FeaturedNews = featuredNews,
                Categories = categories,
                RegionalNews = regionalNews,
                Stats = stats
            };

            return View(vm);
        }

        public IActionResult About() => View();

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
