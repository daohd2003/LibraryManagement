using LibraryManagement.DTOs.Response;
using LibraryManagement.Models;

namespace LibraryManagement.Services
{
    public interface IBorrowedBookService
    {
        Task<IEnumerable<BorrowedBookDto>> GetAllAsync();
        Task<BorrowedBookDetailDto?> GetByIdAsync(int id);
        Task<BorrowedBookDto> AddAsync(BorrowedBook book);
        Task<bool> UpdateAsync(BorrowedBook book);
        Task<bool> DeleteAsync(int id);
        Task<(bool Success, string Message)> BorrowBookAsync(int userId, int bookId);
    }
}
