using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories
{
    public class PenaltyRepository : Repository<Penalty>, IPenaltyRepository
    {
        public PenaltyRepository(LibraryDbContext context) : base(context) { }

        public override async Task<Penalty> AddAsync(Penalty entity)
        {
            var borrowedBookExists = await _context.BorrowedBooks.AnyAsync(b => b.Id == entity.BorrowedBookId);
            if (!borrowedBookExists)
            {
                throw new Exception($"BorrowedBook with Id {entity.BorrowedBookId} does not exist.");
            }

            await _context.Penalties.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
