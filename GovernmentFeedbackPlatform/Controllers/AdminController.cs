using GovernmentFeedbackPlatform.Data;
using GovernmentFeedbackPlatform.Models;
using GovernmentFeedbackPlatform.Services;
using GovernmentFeedbackPlatform.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GovernmentFeedbackPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly INewsService _newsService;
        private readonly IFeedbackService _feedbackService;
        private readonly IAnalyticsService _analyticsService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(INewsService newsService, IFeedbackService feedbackService,
            IAnalyticsService analyticsService, ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _newsService = newsService;
            _feedbackService = feedbackService;
            _analyticsService = analyticsService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var stats = await _analyticsService.GetPlatformStatsAsync();
            var recentNews = await _newsService.GetLatestNewsAsync(5);
            var recentFeedbacks = await _feedbackService.GetAllFeedbacksAsync(1, 5, null, null);
            var categoryStats = await _analyticsService.GetCategoryStatsAsync(null, null, null, null);
            var regionalStats = await _analyticsService.GetRegionalStatsAsync(null, null, null);
            var monthlyTrends = await _analyticsService.GetMonthlyTrendsAsync(6);

            var vm = new AdminDashboardViewModel
            {
                Stats = stats,
                RecentNews = recentNews,
                RecentFeedbacks = recentFeedbacks,
                CategoryStats = categoryStats,
                RegionalStats = regionalStats,
                MonthlyTrends = monthlyTrends
            };
            return View(vm);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public async Task<IActionResult> ManageNews(int page = 1, bool? published = null)
        {
            const int pageSize = 15;
            var includeUnpublished = published == null || published == false;
            var total = await _newsService.GetAllNewsCountAsync(includeUnpublished: true);
            var news = await _newsService.GetAllNewsAsync(page, pageSize, includeUnpublished: true);

            if (published.HasValue)
                news = news.Where(n => n.IsPublished == published.Value).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Published = published;
            return View(news);
        }

        [HttpGet]
        public async Task<IActionResult> CreateNews()
        {
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(new NewsStory());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNews(NewsStory news)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
                return View(news);
            }

            var userId = _context.Users.Where(u => u.UserName == User.Identity!.Name).Select(u => u.Id).FirstOrDefault();
            news.CreatedByUserId = userId;
            if (news.IsPublished && news.PublishedDate == default)
                news.PublishedDate = DateTime.UtcNow;

            await _newsService.CreateNewsAsync(news);
            TempData["Success"] = "News story created.";
            return RedirectToAction(nameof(ManageNews));
        }

        [HttpGet]
        public async Task<IActionResult> EditNews(int id)
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null) return NotFound();
            ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNews(NewsStory news)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();
                return View(news);
            }
            await _newsService.UpdateNewsAsync(news);
            TempData["Success"] = "News story updated.";
            return RedirectToAction(nameof(ManageNews));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNews(int id)
        {
            await _newsService.DeleteNewsAsync(id);
            TempData["Success"] = "News story deleted.";
            return RedirectToAction(nameof(ManageNews));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null) return NotFound();
            news.IsPublished = !news.IsPublished;
            if (news.IsPublished && news.PublishedDate == default)
                news.PublishedDate = DateTime.UtcNow;
            await _newsService.UpdateNewsAsync(news);
            TempData["Success"] = $"News story {(news.IsPublished ? "published" : "unpublished")}.";
            return RedirectToAction(nameof(ManageNews));
        }

        public async Task<IActionResult> ManageFeedback(int page = 1, string? sentiment = null, bool? moderated = null)
        {
            const int pageSize = 20;
            var total = await _feedbackService.GetAllFeedbacksCountAsync(sentiment, moderated);
            var feedbacks = await _feedbackService.GetAllFeedbacksAsync(page, pageSize, sentiment, moderated);

            var vm = new FeedbackManagementViewModel
            {
                Feedbacks = feedbacks,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                FilterSentiment = sentiment,
                FilterModerated = moderated
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModerateFeedback(int id, bool approve, string note)
        {
            await _feedbackService.ModerateFeedbackAsync(id, approve, note);
            TempData["Success"] = $"Feedback {(approve ? "approved" : "rejected")}.";
            return RedirectToAction(nameof(ManageFeedback));
        }

        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories.Include(c => c.NewsStories).ToListAsync();
            return View(categories);
        }
    }
}
