using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(LibraryDbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserWithBorrowedBooksAsync(int id)
        {
            return await _context.Users
                .Include(u => u.BorrowedBooks)
                    .ThenInclude(bb => bb.Book)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
