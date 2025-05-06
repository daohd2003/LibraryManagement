using LibraryManagement.Models;

namespace LibraryManagement.Repositories
{
    public interface IBookRepository : IRepository<Book>
    {
        /*Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
        Task<bool> BookExistsAsync(int id);
        */
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
    }
}
