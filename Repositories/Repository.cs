
using LibraryManagement.Data;
using LibraryManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {

        protected readonly LibraryDbContext _context;

        public Repository(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var affectedRows = 0;

            var entity = await _context.Set<T>().FindAsync(id);

            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                affectedRows = await _context.SaveChangesAsync();
            }
            return affectedRows > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id) != null;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            var keyNames = _context.Model
                .FindEntityType(typeof(T))?
                .FindPrimaryKey()?
                .Properties
                .Select(x => x.Name)
                .ToArray();

            if (keyNames == null || keyNames.Length == 0)
                throw new Exception("No primary key defined.");

            var keyValues = keyNames
                .Select(name => typeof(T).GetProperty(name)!.GetValue(entity))
                .ToArray();

            var exists = await _context.Set<T>().FindAsync(keyValues);
            if (exists == null) throw new Exception("Entity not found.");

            _context.Entry(exists).CurrentValues.SetValues(entity);
            var affectedRows = await _context.SaveChangesAsync();

            return affectedRows > 0;
        }
    }
}
