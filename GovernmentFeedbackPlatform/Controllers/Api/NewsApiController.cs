using GovernmentFeedbackPlatform.Models;
using GovernmentFeedbackPlatform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovernmentFeedbackPlatform.Controllers.Api
{
    [ApiController]
    [Route("api/news")]
    [Produces("application/json")]
    public class NewsApiController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsApiController(INewsService newsService)
        {
            _newsService = newsService;
        }

        /// <summary>GET /api/news – returns paginated published news</summary>
        [HttpGet]
        public async Task<IActionResult> GetNews([FromQuery] string? search, [FromQuery] int? categoryId,
            [FromQuery] string? state, [FromQuery] string? language, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var news = await _newsService.GetPublishedNewsAsync(search, categoryId, state, language, page, pageSize);
            var total = await _newsService.GetPublishedNewsCountAsync(search, categoryId, state, language);
            return Ok(new { data = news, total, page, pageSize, totalPages = (int)Math.Ceiling(total / (double)pageSize) });
        }

        /// <summary>GET /api/news/{id}</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null) return NotFound(new { message = "News story not found." });
            return Ok(news);
        }

        /// <summary>POST /api/news – Admin only</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] NewsStory news)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _newsService.CreateNewsAsync(news);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>PUT /api/news/{id} – Admin only</summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] NewsStory news)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != news.Id) return BadRequest(new { message = "ID mismatch." });
            var existing = await _newsService.GetNewsByIdAsync(id);
            if (existing == null) return NotFound(new { message = "News story not found." });
            var updated = await _newsService.UpdateNewsAsync(news);
            return Ok(updated);
        }

        /// <summary>DELETE /api/news/{id} – Admin only</summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _newsService.GetNewsByIdAsync(id);
            if (existing == null) return NotFound(new { message = "News story not found." });
            await _newsService.DeleteNewsAsync(id);
            return NoContent();
        }
    }
}
