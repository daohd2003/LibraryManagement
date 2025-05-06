using LibraryManagement.Data;
using LibraryManagement.Infrastructure;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LibraryManagement.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryDbContext _libraryDbContext;

        public BookRepository(LibraryDbContext libraryDbContext)
        {
            _libraryDbContext = libraryDbContext;
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            _libraryDbContext.Books.Add(book);
            await _libraryDbContext.SaveChangesAsync();
            var result = await _libraryDbContext.Books
           .Include(b => b.BookCategories)
               .ThenInclude(bc => bc.Category)
           .FirstOrDefaultAsync(b => b.Id == book.Id);

            if (result == null)
            {
                throw new InvalidOperationException("Book not found after insert.");
            }

            return result;
        }

        public async Task<bool> BookExistsAsync(int id)
        {
            return await _libraryDbContext.Books.AnyAsync(book => book.Id == id);
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await GetBookByIdAsync(id);
            if (book == null)
                throw new KeyNotFoundException($"Book with ID {id} not found.");

            _libraryDbContext.Books.Remove(book);
            await _libraryDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _libraryDbContext.Books
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _libraryDbContext.Books
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            return await _libraryDbContext.Books
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
                .Where(b => b.BookCategories.Any(bc => bc.CategoryId.Equals(categoryId)))
                .ToListAsync();
        }

        public async Task UpdateBookAsync(Book book)
        {
            var existingBook = await _libraryDbContext.Books.FindAsync(book.Id);

            if (existingBook == null)
                throw new KeyNotFoundException($"Book with ID {book.Id} not found.");

            _libraryDbContext.Entry(existingBook).CurrentValues.SetValues(book);
            await _libraryDbContext.SaveChangesAsync();
        }
    }
}
