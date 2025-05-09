using LibraryManagement.DTOs.Response;
using LibraryManagement.Models;

namespace LibraryManagement.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetUsersWithBorrowedBooksAsync(int id);
        Task<User> AddAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<User> GetOrCreateUserAsync(GooglePayload payload);
    }
}
