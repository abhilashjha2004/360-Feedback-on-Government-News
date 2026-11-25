using GovernmentFeedbackPlatform.Data;
using GovernmentFeedbackPlatform.Models;
using GovernmentFeedbackPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GovernmentFeedbackPlatform.Controllers.Api
{
    [ApiController]
    [Route("api/feedback")]
    [Produces("application/json")]
    public class FeedbackApiController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ApplicationDbContext _context;

        public FeedbackApiController(IFeedbackService feedbackService, ApplicationDbContext context)
        {
            _feedbackService = feedbackService;
            _context = context;
        }

        /// <summary>GET /api/feedback?newsId={id}</summary>
        [HttpGet]
        public async Task<IActionResult> GetFeedbacks([FromQuery] int newsId, [FromQuery] int count = 10)
        {
            var feedbacks = await _feedbackService.GetFeedbacksForNewsAsync(newsId, count);
            return Ok(feedbacks);
        }

        /// <summary>GET /api/feedback/stats/{newsId}</summary>
        [HttpGet("stats/{newsId}")]
        public async Task<IActionResult> GetStats(int newsId)
        {
            var stats = await _feedbackService.GetFeedbackStatsAsync(newsId);
            return Ok(stats);
        }

        /// <summary>POST /api/feedback – Requires authentication</summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Submit([FromBody] Feedback feedback)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = await _context.Users
                .Where(u => u.UserName == User.Identity!.Name)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (userId == null) return Unauthorized();

            var existing = await _feedbackService.GetUserFeedbackAsync(userId, feedback.NewsStoryId);
            if (existing != null) return Conflict(new { message = "Feedback already submitted for this news." });

            feedback.UserId = userId;
            var created = await _feedbackService.SubmitFeedbackAsync(feedback);
            return CreatedAtAction(nameof(GetFeedbacks), new { newsId = created.NewsStoryId }, created);
        }

        /// <summary>PUT /api/feedback/{id} – Requires authentication (own feedback only)</summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] Feedback feedback)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != feedback.Id) return BadRequest(new { message = "ID mismatch." });

            var userId = await _context.Users
                .Where(u => u.UserName == User.Identity!.Name)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            var existing = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId && !f.IsDeleted);
            if (existing == null) return NotFound(new { message = "Feedback not found or access denied." });

            existing.Rating = feedback.Rating;
            existing.UserSentiment = feedback.UserSentiment;
            existing.Comment = feedback.Comment;
            existing.Suggestions = feedback.Suggestions;
            existing.FeedbackCategory = feedback.FeedbackCategory;

            var updated = await _feedbackService.UpdateFeedbackAsync(existing);
            return Ok(updated);
        }

        /// <summary>DELETE /api/feedback/{id} – Requires authentication (own feedback only)</summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = await _context.Users
                .Where(u => u.UserName == User.Identity!.Name)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (userId == null) return Unauthorized();

            var feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId && !f.IsDeleted);
            if (feedback == null) return NotFound(new { message = "Feedback not found or access denied." });

            await _feedbackService.DeleteFeedbackAsync(id, userId);
            return NoContent();
        }
    }
}
