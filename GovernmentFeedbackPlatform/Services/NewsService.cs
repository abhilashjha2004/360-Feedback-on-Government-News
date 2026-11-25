using GovernmentFeedbackPlatform.Data;
using GovernmentFeedbackPlatform.Models;
using Microsoft.EntityFrameworkCore;

namespace GovernmentFeedbackPlatform.Services
{
    public class NewsService : INewsService
    {
        private readonly ApplicationDbContext _context;

        public NewsService(ApplicationDbContext context)
        {
            _context = context;
        }

        private IQueryable<NewsStory> PublishedQuery(string? search, int? categoryId, string? state, string? language)
        {
            var q = _context.NewsStories
                .Include(n => n.Category)
                .Where(n => n.IsPublished && !n.IsArchived);

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(n => n.Title.Contains(search) || (n.Summary != null && n.Summary.Contains(search)));
            if (categoryId.HasValue)
                q = q.Where(n => n.CategoryId == categoryId.Value);
            if (!string.IsNullOrWhiteSpace(state))
                q = q.Where(n => n.State == state);
            if (!string.IsNullOrWhiteSpace(language))
                q = q.Where(n => n.Language == language);

            return q;
        }

        public async Task<List<NewsStory>> GetPublishedNewsAsync(string? search, int? categoryId, string? state, string? language, int page, int pageSize)
        {
            return await PublishedQuery(search, categoryId, state, language)
                .OrderByDescending(n => n.PublishedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetPublishedNewsCountAsync(string? search, int? categoryId, string? state, string? language)
        {
            return await PublishedQuery(search, categoryId, state, language).CountAsync();
        }

        public async Task<NewsStory?> GetNewsByIdAsync(int id)
        {
            return await _context.NewsStories
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<List<NewsStory>> GetAllNewsAsync(int page, int pageSize, bool includeUnpublished = false)
        {
            var q = _context.NewsStories.Include(n => n.Category).AsQueryable();
            if (!includeUnpublished)
                q = q.Where(n => n.IsPublished);
            return await q.OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetAllNewsCountAsync(bool includeUnpublished = false)
        {
            var q = _context.NewsStories.AsQueryable();
            if (!includeUnpublished)
                q = q.Where(n => n.IsPublished);
            return await q.CountAsync();
        }

        public async Task<NewsStory> CreateNewsAsync(NewsStory news)
        {
            news.CreatedAt = DateTime.UtcNow;
            news.UpdatedAt = DateTime.UtcNow;
            _context.NewsStories.Add(news);
            await _context.SaveChangesAsync();
            return news;
        }

        public async Task<NewsStory> UpdateNewsAsync(NewsStory news)
        {
            news.UpdatedAt = DateTime.UtcNow;
            _context.NewsStories.Update(news);
            await _context.SaveChangesAsync();
            return news;
        }

        public async Task DeleteNewsAsync(int id)
        {
            var news = await _context.NewsStories.FindAsync(id);
            if (news != null)
            {
                _context.NewsStories.Remove(news);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<NewsStory>> GetLatestNewsAsync(int count)
        {
            return await _context.NewsStories
                .Include(n => n.Category)
                .Where(n => n.IsPublished && !n.IsArchived)
                .OrderByDescending(n => n.PublishedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<NewsStory>> GetNewsByCategoryAsync(int categoryId, int count)
        {
            return await _context.NewsStories
                .Include(n => n.Category)
                .Where(n => n.IsPublished && !n.IsArchived && n.CategoryId == categoryId)
                .OrderByDescending(n => n.PublishedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllStatesAsync()
        {
            return await _context.NewsStories
                .Where(n => n.State != null)
                .Select(n => n.State!)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();
        }

        public async Task<List<string>> GetAllLanguagesAsync()
        {
            return await _context.NewsStories
                .Where(n => n.Language != null)
                .Select(n => n.Language!)
                .Distinct()
                .OrderBy(l => l)
                .ToListAsync();
        }
    }
}
