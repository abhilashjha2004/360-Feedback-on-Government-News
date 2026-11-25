using GovernmentFeedbackPlatform.Data;
using GovernmentFeedbackPlatform.Models;
using GovernmentFeedbackPlatform.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GovernmentFeedbackPlatform.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly ApplicationDbContext _context;
        private readonly ISentimentService _sentimentService;

        public FeedbackService(ApplicationDbContext context, ISentimentService sentimentService)
        {
            _context = context;
            _sentimentService = sentimentService;
        }

        public async Task<Feedback?> GetUserFeedbackAsync(string userId, int newsId)
        {
            return await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.NewsStory)
                .FirstOrDefaultAsync(f => f.UserId == userId && f.NewsStoryId == newsId && !f.IsDeleted);
        }

        public async Task<Feedback> SubmitFeedbackAsync(Feedback feedback)
        {
            // Auto-detect sentiment from comment/suggestions
            var text = $"{feedback.Comment} {feedback.Suggestions}";
            feedback.DetectedSentiment = _sentimentService.AnalyzeSentiment(text);
            feedback.CreatedAt = DateTime.UtcNow;
            feedback.UpdatedAt = DateTime.UtcNow;
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();
            return feedback;
        }

        public async Task<Feedback> UpdateFeedbackAsync(Feedback feedback)
        {
            var text = $"{feedback.Comment} {feedback.Suggestions}";
            feedback.DetectedSentiment = _sentimentService.AnalyzeSentiment(text);
            feedback.UpdatedAt = DateTime.UtcNow;
            _context.Feedbacks.Update(feedback);
            await _context.SaveChangesAsync();
            return feedback;
        }

        public async Task DeleteFeedbackAsync(int id, string userId)
        {
            var feedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);
            if (feedback != null)
            {
                feedback.IsDeleted = true;
                feedback.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Feedback>> GetFeedbacksForNewsAsync(int newsId, int count)
        {
            return await _context.Feedbacks
                .Include(f => f.User)
                .Where(f => f.NewsStoryId == newsId && !f.IsDeleted && f.IsApproved)
                .OrderByDescending(f => f.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<FeedbackStatsViewModel> GetFeedbackStatsAsync(int newsId)
        {
            var feedbacks = await _context.Feedbacks
                .Where(f => f.NewsStoryId == newsId && !f.IsDeleted && f.IsApproved)
                .ToListAsync();

            var stats = new FeedbackStatsViewModel
            {
                TotalFeedbacks = feedbacks.Count,
                AverageRating = feedbacks.Count > 0 ? Math.Round(feedbacks.Average(f => f.Rating), 1) : 0,
                PositiveCount = feedbacks.Count(f => f.DetectedSentiment == SentimentType.Positive || f.UserSentiment == SentimentType.Positive),
                NeutralCount = feedbacks.Count(f => f.DetectedSentiment == SentimentType.Neutral || f.UserSentiment == SentimentType.Neutral),
                NegativeCount = feedbacks.Count(f => f.DetectedSentiment == SentimentType.Negative || f.UserSentiment == SentimentType.Negative),
                RatingDistribution = new Dictionary<int, int>()
            };

            for (int r = 1; r <= 5; r++)
                stats.RatingDistribution[r] = feedbacks.Count(f => f.Rating == r);

            return stats;
        }

        public async Task<List<Feedback>> GetUserFeedbacksAsync(string userId)
        {
            return await _context.Feedbacks
                .Include(f => f.NewsStory)
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Feedback>> GetAllFeedbacksAsync(int page, int pageSize, string? sentiment, bool? moderated)
        {
            var q = _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.NewsStory)
                .Where(f => !f.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(sentiment) && Enum.TryParse<SentimentType>(sentiment, out var sent))
                q = q.Where(f => f.DetectedSentiment == sent);
            if (moderated.HasValue)
                q = q.Where(f => f.IsModerated == moderated.Value);

            return await q.OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetAllFeedbacksCountAsync(string? sentiment, bool? moderated)
        {
            var q = _context.Feedbacks.Where(f => !f.IsDeleted).AsQueryable();

            if (!string.IsNullOrWhiteSpace(sentiment) && Enum.TryParse<SentimentType>(sentiment, out var sent))
                q = q.Where(f => f.DetectedSentiment == sent);
            if (moderated.HasValue)
                q = q.Where(f => f.IsModerated == moderated.Value);

            return await q.CountAsync();
        }

        public async Task ModerateFeedbackAsync(int id, bool approve, string note)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                feedback.IsModerated = true;
                feedback.IsApproved = approve;
                feedback.ModerationNote = note;
                feedback.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
