using LibraryManagement.Models;

namespace LibraryManagement.Repositories
{
    public interface IBorrowedBookRepository : IRepository<BorrowedBook>
    {
        Task<IEnumerable<BorrowedBook>> GetAllWithDetailsAsync();
        Task<BorrowedBook?> GetByIdWithDetailsAsync(int id);
        Task<bool> IsAlreadyBorrowedAndNotReturned(int userId, int bookId);
    }
}
