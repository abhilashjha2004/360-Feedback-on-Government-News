using GovernmentFeedbackPlatform.Models;

namespace GovernmentFeedbackPlatform.Services
{
    public interface INewsService
    {
        Task<List<NewsStory>> GetPublishedNewsAsync(string? search, int? categoryId, string? state, string? language, int page, int pageSize);
        Task<int> GetPublishedNewsCountAsync(string? search, int? categoryId, string? state, string? language);
        Task<NewsStory?> GetNewsByIdAsync(int id);
        Task<List<NewsStory>> GetAllNewsAsync(int page, int pageSize, bool includeUnpublished = false);
        Task<int> GetAllNewsCountAsync(bool includeUnpublished = false);
        Task<NewsStory> CreateNewsAsync(NewsStory news);
        Task<NewsStory> UpdateNewsAsync(NewsStory news);
        Task DeleteNewsAsync(int id);
        Task<List<NewsStory>> GetLatestNewsAsync(int count);
        Task<List<NewsStory>> GetNewsByCategoryAsync(int categoryId, int count);
        Task<List<string>> GetAllStatesAsync();
        Task<List<string>> GetAllLanguagesAsync();
    }
}
