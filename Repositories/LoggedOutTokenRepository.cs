using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories
{
    public class LoggedOutTokenRepository : Repository<BlacklistedToken>, ILoggedOutTokenRepository
    {
        public LoggedOutTokenRepository(LibraryDbContext context) : base(context)
        {
        }

        public async Task AddAsync(string token, DateTime expirationDate)
        {
            var loggedOutToken = new BlacklistedToken { Token = token, ExpiredAt = expirationDate };

            await _context.BlacklistedTokens.AddAsync(loggedOutToken);

            await _context.SaveChangesAsync();

        }

        public async Task<bool> IsTokenLoggedOutAsync(string token)
        {
            return await _context.BlacklistedTokens.AnyAsync(bt => bt.Token == token && bt.ExpiredAt > DateTime.UtcNow);
        }
    }
}
