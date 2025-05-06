using LibraryManagement.Models;

namespace LibraryManagement.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<bool> ExistsByNameAsync(string name);
    }
}
