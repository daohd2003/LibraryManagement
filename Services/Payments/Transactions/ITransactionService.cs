using LibraryManagement.Models;

namespace LibraryManagement.Services.Payments.Transactions
{
    public interface ITransactionService
    {
        Task<Transaction> SaveTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId);
    }
}
