using LibraryManagement.Models;

namespace LibraryManagement.Repositories
{
    public interface ILoggedOutTokenRepository : IRepository<BlacklistedToken>
    {
        Task AddAsync(string token, DateTime expirationDate);
        Task<bool> IsTokenLoggedOutAsync(string token);
    }
}
