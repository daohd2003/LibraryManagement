using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services.Payments.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly LibraryDbContext _dbContext;
        private readonly ILogger<TransactionService> _logger;

        public TransactionService(LibraryDbContext dbContext, ILogger<TransactionService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId)
        {
            return await _dbContext.Transactions.Include(t => t.User).Where(t => t.UserId == userId).OrderByDescending(t => t.CreatedAt).AsNoTracking().ToListAsync();
        }

        public async Task<Transaction> SaveTransactionAsync(Transaction transaction)
        {
            try
            {
                _dbContext.Transactions.Add(transaction);
                await _dbContext.SaveChangesAsync();
                return transaction;
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Error saving transaction to database. Transaction details: {@Transaction}", transaction);
                throw;
            }
        }
    }
}
