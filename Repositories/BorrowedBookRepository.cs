using LibraryManagement.Data;
using LibraryManagement.Enum;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories
{
    public class BorrowedBookRepository : Repository<BorrowedBook>, IBorrowedBookRepository
    {
        public BorrowedBookRepository(LibraryDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BorrowedBook>> GetAllWithDetailsAsync()
        {
            return await _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<BorrowedBook?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(bb => bb.Id == id);
        }

        public async Task<bool> IsAlreadyBorrowedAndNotReturned(int userId, int bookId)
        {
            return await _context.BorrowedBooks.AnyAsync(bb =>
                bb.UserId == userId &&
                bb.BookId == bookId &&
                bb.Status != "Returned");
        }

        public async Task<BorrowedBook?> GetByUserIdAndBookIdAsync(int userId, int bookId)
        {
            return await _context.BorrowedBooks
                .Include(bb => bb.Book)
                .Include(bb => bb.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(bb => bb.UserId == userId && bb.BookId == bookId);
        }

        public async Task<int> UpdateOverdueStatusAsync()
        {
            var overdueBooks = await _context.BorrowedBooks
                .Where(bb => bb.Status == BorrowStatus.Borrowed.ToString() && bb.DueDate < DateTime.Now)
                .ToListAsync();

            foreach (var bb in overdueBooks)
            {
                bb.Status = BorrowStatus.Overdue.ToString();
            }

            return await _context.SaveChangesAsync();
        }
    }
}

