using LibraryManagement.Data;
using LibraryManagement.DTOs.Response;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LibraryManagement.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(LibraryDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<User>> GetUsersWithBorrowedBooksAsync(int bookId)
        {
            return await _context.Users
                .Include(u => u.BorrowedBooks)
                    .ThenInclude(bb => bb.Book)
                .Where(u => u.BorrowedBooks.Any(bb => bb.BookId == bookId && bb.Status != "Returned"))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetOrCreateUserAsync(GooglePayload payload)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == payload.Email);

            if (user == null)
            {
                // Nếu người dùng chưa tồn tại, tạo mới
                user = new User
                {
                    Username = "user",
                    Email = payload.Email,
                    AvatarUrl = "https://static-00.iconduck.com/assets.00/avatar-default-symbolic-icon-479x512-n8sg74wg.png",
                    GoogleId = payload.Sub,
                    Role = "Member",
                    PasswordHash = "",
                    RefreshToken = "",
                    RefreshTokenExpiryTime = System.DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return user;
        }

    }
}
