using LibraryManagement.DTOs.Request;
using LibraryManagement.Models;

namespace LibraryManagement.Services.Payments.Transactions
{
    public interface ITransactionService
    {
        Task<Transaction> SaveTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetUserTransactionsAsync(int userId);
        Task<Transaction?> GetTransactionByIdAsync(int transactionId);
        Task<bool> ProcessSepayWebhookAsync(SepayWebhookRequest request);
    }
}
