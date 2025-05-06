using LibraryManagement.Models;

namespace LibraryManagement.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserWithBorrowedBooksAsync(int id);
    }
}
