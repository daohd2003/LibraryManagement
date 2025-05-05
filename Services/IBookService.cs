using LibraryManagement.DTOs;

namespace LibraryManagement.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDetailDto>> GetAllBooksAsync();
        Task<BookDetailDto?> GetBookByIdAsync(int id);
        Task<BookDetailDto> AddBookAsync(BookDto bookDto);
        Task UpdateBookAsync(int id, BookDto bookDto);
        Task DeleteBookAsync(int id);
    }
}
