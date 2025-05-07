using LibraryManagement.DTOs.Response;
using LibraryManagement.Models;

namespace LibraryManagement.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDetailDto>> GetAllBooksAsync();
        Task<BookDetailDto?> GetBookByIdAsync(int id);
        Task<BookDetailDto> AddBookAsync(BookDto bookDto);
        Task UpdateBookAsync(int id, BookDto bookDto);
        Task DeleteBookAsync(int id);
        Task<IEnumerable<BookDetailDto>> GetPagedBooksAsync(int pageNumber, int pageSize);
        Task<IEnumerable<BookDetailDto>> SearchBooksPagedAsync(string keyword, int lastId, int pageSize);
    }
}
