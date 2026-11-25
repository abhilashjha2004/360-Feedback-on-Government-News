using GovernmentFeedbackPlatform.Models;
using GovernmentFeedbackPlatform.ViewModels;

namespace GovernmentFeedbackPlatform.Services
{
    public interface IFeedbackService
    {
        Task<Feedback?> GetUserFeedbackAsync(string userId, int newsId);
        Task<Feedback> SubmitFeedbackAsync(Feedback feedback);
        Task<Feedback> UpdateFeedbackAsync(Feedback feedback);
        Task DeleteFeedbackAsync(int id, string userId);
        Task<List<Feedback>> GetFeedbacksForNewsAsync(int newsId, int count);
        Task<FeedbackStatsViewModel> GetFeedbackStatsAsync(int newsId);
        Task<List<Feedback>> GetUserFeedbacksAsync(string userId);
        Task<List<Feedback>> GetAllFeedbacksAsync(int page, int pageSize, string? sentiment, bool? moderated);
        Task<int> GetAllFeedbacksCountAsync(string? sentiment, bool? moderated);
        Task ModerateFeedbackAsync(int id, bool approve, string note);
    }
}
