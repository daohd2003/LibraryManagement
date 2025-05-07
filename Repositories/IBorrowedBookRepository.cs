using LibraryManagement.Models;
using System.Net;

namespace LibraryManagement.Repositories
{
    public interface IBorrowedBookRepository : IRepository<BorrowedBook>
    {
        Task<IEnumerable<BorrowedBook>> GetAllWithDetailsAsync();
        Task<BorrowedBook?> GetByIdWithDetailsAsync(int id);
        Task<bool> IsAlreadyBorrowedAndNotReturned(int userId, int bookId);
        Task<BorrowedBook?> GetByUserIdAndBookIdAsync(int userId, int bookId);
        Task<int> UpdateOverdueStatusAsync();
    }
}
