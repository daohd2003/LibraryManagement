using LibraryManagement.DTOs.Response;
using LibraryManagement.Models;

namespace LibraryManagement.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> GetUsersWithBorrowedBooksAsync(int bookId);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> GetOrCreateUserAsync(GooglePayload payload);
    }
}
