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
        Task<IEnumerable<Book>> GetPagedBooksAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Book>> SearchBooksPagedAsync(string keyword, int lastId, int pageSize);
    }
}
