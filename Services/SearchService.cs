using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class SearchService
    {
        private readonly LibraryDbContext _context;

        public SearchService(LibraryDbContext context)
        {

            _context = context;
        }

        // 1. Lấy lịch sử tìm kiếm
        public async Task<List<string>> GetSearchHistoryAsync(int? userId, string clientIdentifier)
        {
            return await _context.SearchHistories
                .Where(sh =>
                    (userId != null && sh.UserId == userId) ||
                    (userId == null && sh.ClientIdentifier == clientIdentifier)
                )
                .OrderByDescending(sh => sh.SearchedAt)
                .Select(sh => sh.Query)
                .Distinct()
                .OrderBy(q => q)
                .Take(10)
                .ToListAsync();
        }

        // 2. Gợi ý 5 kết quả liên quan
        public async Task<List<Book>> GetSearchSuggestionsAsync(string query)
        {
            return await _context.Books
                .Where(b => b.Title.Contains(query) ||
                       b.Author.Contains(query) ||
                       b.ISBN.Contains(query))
                .OrderBy(b => b)
                .Take(5)
                .ToListAsync();
        }

        // 3. Tìm kiếm full kết quả
        public async Task<(List<Book> results, int totalCount)> SearchBooksAsync(
            string query,
            int page = 1,
            int pageSize = 10)
        {
            var queryable = _context.Books
                .Where(b => b.Title.Contains(query) ||
                       b.Author.Contains(query) ||
                       b.ISBN.Contains(query))
                .OrderBy(b => b.Title);

            int totalCount = await queryable.CountAsync();

            var results = await queryable
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (results, totalCount);
        }

        // Lưu lịch sử tìm kiếm
        public async Task SaveSearchHistoryAsync(string query, int? userId, string clientIdentifier)
        {
            _context.SearchHistories.Add(new SearchHistory
            {
                Query = query,
                UserId = userId,
                ClientIdentifier = userId == null ? clientIdentifier : null,
            });
            await _context.SaveChangesAsync();
        }
    }
}
