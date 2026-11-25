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
    [Authorize]
    public class CitizenController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly INewsService _newsService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CitizenController(
            IFeedbackService feedbackService,
            INewsService newsService,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _feedbackService = feedbackService;
            _newsService = newsService;
            _context = context;
            _userManager = userManager;
        }

        private async Task<string?> GetCurrentUserIdAsync()
        {
            var name = User.Identity?.Name;
            if (string.IsNullOrEmpty(name)) return null;
            return await _context.Users.Where(u => u.UserName == name).Select(u => u.Id).FirstOrDefaultAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var feedbacks = await _feedbackService.GetUserFeedbacksAsync(user.Id);
            var recentNews = await _newsService.GetLatestNewsAsync(6);

            var positive = feedbacks.Count(f => f.DetectedSentiment == SentimentType.Positive || f.UserSentiment == SentimentType.Positive);
            var negative = feedbacks.Count(f => f.DetectedSentiment == SentimentType.Negative || f.UserSentiment == SentimentType.Negative);
            var neutral = feedbacks.Count - positive - negative;
            if (neutral < 0) neutral = 0;

            var vm = new CitizenDashboardViewModel
            {
                User = user,
                RecentFeedbacks = feedbacks.Take(5).ToList(),
                TotalFeedbacks = feedbacks.Count,
                AverageRating = feedbacks.Count > 0 ? Math.Round(feedbacks.Average(f => f.Rating), 1) : 0,
                PositiveCount = positive,
                NeutralCount = neutral,
                NegativeCount = negative,
                RecentNews = recentNews
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null) return Unauthorized();

            var feedbacks = await _feedbackService.GetUserFeedbacksAsync(userId);
            return View(feedbacks);
        }

        [HttpGet]
        public async Task<IActionResult> EditFeedback(int id)
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null) return Unauthorized();

            var feedback = await _context.Feedbacks
                .Include(f => f.NewsStory)
                    .ThenInclude(n => n!.Category)
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId && !f.IsDeleted);

            if (feedback == null) return NotFound();

            return View(feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFeedback(int id, int rating, string userSentiment, string? comment, string? suggestions, string? feedbackCategory)
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null) return Unauthorized();

            var feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId && !f.IsDeleted);
            if (feedback == null) return NotFound();

            if (!Enum.TryParse<SentimentType>(userSentiment, out var sentiment))
                sentiment = SentimentType.Neutral;

            feedback.Rating = rating;
            feedback.UserSentiment = sentiment;
            feedback.Comment = comment;
            feedback.Suggestions = suggestions;
            feedback.FeedbackCategory = feedbackCategory;

            await _feedbackService.UpdateFeedbackAsync(feedback);
            TempData["Success"] = "Your feedback has been successfully updated.";
            return RedirectToAction(nameof(History));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null) return Unauthorized();

            await _feedbackService.DeleteFeedbackAsync(id, userId);
            TempData["Success"] = "Your feedback has been deleted.";
            return RedirectToAction(nameof(History));
        }
    }
}
