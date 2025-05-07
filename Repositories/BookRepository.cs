using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Dapper;

namespace LibraryManagement.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(LibraryDbContext context) : base(context) { }

        public override async Task<Book> AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            var result = await _context.Books
           .Include(b => b.BookCategories)
               .ThenInclude(bc => bc.Category)
           .FirstOrDefaultAsync(b => b.Id == book.Id);

            if (result == null)
            {
                throw new InvalidOperationException("Book not found after insert.");
            }

            return result;
        }

        /*        public async Task<bool> BookExistsAsync(int id)
                {
                    return await _context.Books.AnyAsync(book => book.Id == id);
                }*/

        /*public override async Task<bo> DeleteAsync(int id)
        {
            var book = await GetBookByIdAsync(id);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {id} not found.");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }*/

        public override async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
                .Include(b => b.BorrowedBooks)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            return await _context.Books
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
                .Where(b => b.BookCategories.Any(bc => bc.CategoryId.Equals(categoryId)))
                .ToListAsync();
        }

        public override async Task<bool> UpdateAsync(Book book)
        {
            var existingBook = await _context.Books.FindAsync(book.Id);

            if (existingBook == null)
                throw new KeyNotFoundException($"Book with ID {book.Id} not found.");

            _context.Entry(existingBook).CurrentValues.SetValues(book);
            var affectedRows = await _context.SaveChangesAsync();

            return affectedRows > 0;
        }

        public async Task<IEnumerable<Book>> GetPagedBooksAsync(int pageNumber, int pageSize)
        {
            var offset = (pageNumber - 1) * pageSize;
            /*var sql = @"
                SELECT b.*, bc.*, c.*
                FROM Books b
                LEFT JOIN BookCategories bc ON b.Id = bc.BookId
                LEFT JOIN Categories c ON bc.CategoryId = c.Id
                ORDER BY b.Id
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;
            ";

            using var connection = _context.Database.GetDbConnection();
            return await connection.QueryAsync<Book>(sql, new { Offset = offset, PageSize = pageSize });*/

            return await _context.Books
                .OrderBy(b => b.Id)
                .Skip(offset)
                .Take(pageSize)
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> SearchBooksPagedAsync(string keyword, int lastId, int pageSize)
        {
            return await _context.Books
                .Where(b => b.Id > lastId && b.Title.Contains(keyword))
                .OrderBy(b => b.Id)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
