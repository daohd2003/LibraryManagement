using LibraryManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private readonly SearchService _searchService;

        public SearchController(SearchService searchService)
        {
            _searchService = searchService;
        }

        // GET api/search/history
        [HttpGet("history")]
        public async Task<IActionResult> GetSearchHistory()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            var history = await _searchService.GetSearchHistoryAsync(
                userId != null ? int.Parse(userId) : null,
                clientIp
            );

            return Ok(history);
        }

        // GET api/search/suggestions?query=harry
        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required");

            var suggestions = await _searchService.GetSearchSuggestionsAsync(query);
            return Ok(suggestions);
        }

        // GET api/search/results?query=harry&page=1
        [HttpGet("results")]
        public async Task<IActionResult> SearchBooks(
            [FromQuery] string query,
            [FromQuery] int page = 1)
        {
            // Lưu lịch sử tìm kiếm
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            await _searchService.SaveSearchHistoryAsync(query,
                userId != null ? int.Parse(userId) : null, clientIp);

            // Thực hiện tìm kiếm
            var (results, totalCount) = await _searchService.SearchBooksAsync(query, page);

            return Ok(new
            {
                Results = results,
                TotalCount = totalCount,
                CurrentPage = page
            });
        }
    }
}
