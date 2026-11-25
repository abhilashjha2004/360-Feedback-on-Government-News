using GovernmentFeedbackPlatform.Data;
using GovernmentFeedbackPlatform.Models;
using GovernmentFeedbackPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GovernmentFeedbackPlatform.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ApplicationDbContext _context;

        public FeedbackController(IFeedbackService feedbackService, ApplicationDbContext context)
        {
            _feedbackService = feedbackService;
            _context = context;
        }

        private async Task<string?> GetCurrentUserIdAsync()
        {
            var name = User.Identity?.Name;
            if (name == null) return null;
            return await _context.Users.Where(u => u.UserName == name).Select(u => u.Id).FirstOrDefaultAsync();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int newsStoryId, int rating, string userSentiment, string? comment, string? suggestions, string? feedbackCategory)
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null) return Unauthorized();

            var existing = await _feedbackService.GetUserFeedbackAsync(userId, newsStoryId);
            if (existing != null)
            {
                TempData["Error"] = "You have already submitted feedback for this news story.";
                return RedirectToAction("Details", "News", new { id = newsStoryId });
            }

            if (!Enum.TryParse<SentimentType>(userSentiment, out var sentiment))
                sentiment = SentimentType.Neutral;

            var feedback = new Feedback
            {
                UserId = userId,
                NewsStoryId = newsStoryId,
                Rating = rating,
                UserSentiment = sentiment,
                Comment = comment,
                Suggestions = suggestions,
                FeedbackCategory = feedbackCategory
            };

            await _feedbackService.SubmitFeedbackAsync(feedback);
            TempData["Success"] = "Feedback submitted successfully!";
            return RedirectToAction("Details", "News", new { id = newsStoryId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null) return Unauthorized();

            var feedback = await _context.Feedbacks
                .Include(f => f.NewsStory)
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId && !f.IsDeleted);
            if (feedback == null) return NotFound();

            return View(feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int rating, string userSentiment, string? comment, string? suggestions, string? feedbackCategory)
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
            TempData["Success"] = "Feedback updated.";
            return RedirectToAction("Details", "News", new { id = feedback.NewsStoryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null) return Unauthorized();

            var feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
            var newsId = feedback?.NewsStoryId ?? 0;

            await _feedbackService.DeleteFeedbackAsync(id, userId);
            TempData["Success"] = "Feedback deleted.";
            return newsId > 0 ? RedirectToAction("Details", "News", new { id = newsId }) : RedirectToAction("History");
        }

        [HttpGet]
        public async Task<IActionResult> History()
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null) return Unauthorized();

            var feedbacks = await _feedbackService.GetUserFeedbacksAsync(userId);
            return View(feedbacks);
        }
    }
}
