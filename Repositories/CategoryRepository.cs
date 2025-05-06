using LibraryManagement.Data;
using LibraryManagement.Infrastructure;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(LibraryDbContext context) : base(context) { }

        public async Task<bool> ExistsByNameAsync(string name)
        => await _context.Categories.AnyAsync(c => c.Name == name);
    }
}
