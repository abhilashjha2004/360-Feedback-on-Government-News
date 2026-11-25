using GovernmentFeedbackPlatform.Data;
using GovernmentFeedbackPlatform.Services;
using GovernmentFeedbackPlatform.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GovernmentFeedbackPlatform.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;
        private readonly IFeedbackService _feedbackService;
        private readonly ApplicationDbContext _context;

        public NewsController(INewsService newsService, IFeedbackService feedbackService, ApplicationDbContext context)
        {
            _newsService = newsService;
            _feedbackService = feedbackService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int? categoryId, string? state, string? language, int page = 1)
        {
            const int pageSize = 12;
            var total = await _newsService.GetPublishedNewsCountAsync(search, categoryId, state, language);
            var news = await _newsService.GetPublishedNewsAsync(search, categoryId, state, language, page, pageSize);
            var categories = await _context.Categories.Where(c => c.IsActive).ToListAsync();

            var vm = new NewsListViewModel
            {
                NewsStories = news,
                Categories = categories,
                SearchTerm = search,
                CategoryId = categoryId,
                State = state,
                Language = language,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(total / (double)pageSize),
                TotalCount = total,
                PageSize = pageSize
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null) return NotFound();

            var stats = await _feedbackService.GetFeedbackStatsAsync(id);
            var recentFeedbacks = await _feedbackService.GetFeedbacksForNewsAsync(id, 10);

            Models.Feedback? userFeedback = null;
            bool alreadySubmitted = false;
            if (User.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(User.Identity.Name))
            {
                var userId = await _context.Users.Where(u => u.UserName == User.Identity.Name).Select(u => u.Id).FirstOrDefaultAsync();
                if (!string.IsNullOrEmpty(userId))
                {
                    userFeedback = await _feedbackService.GetUserFeedbackAsync(userId, id);
                    alreadySubmitted = userFeedback != null;
                }
            }

            var vm = new NewsDetailViewModel
            {
                NewsStory = news,
                RecentFeedbacks = recentFeedbacks,
                Stats = stats,
                FeedbackForm = new FeedbackFormViewModel { NewsStoryId = id, NewsStoryTitle = news.Title },
                UserAlreadySubmitted = alreadySubmitted,
                UserFeedback = userFeedback
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q)) return RedirectToAction(nameof(Index));
            return RedirectToAction(nameof(Index), new { search = q });
        }
    }
}
