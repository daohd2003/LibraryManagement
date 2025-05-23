using LibraryManagement.Data;
using LibraryManagement.DTOs.Request;
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

        public async Task<bool> ProcessSepayWebhookAsync(SepayWebhookRequest request)
        {
            const string expectedBankAccount = "0347350184";

            if (request.BankAccount != expectedBankAccount)
            {
                _logger.LogWarning("Invalid bank account: {BankAccount}", request.BankAccount);
                return false;
            }

            // Lấy TransactionCode từ nội dung content
            var transactionCode = ExtractTransactionCode(request.Content);
            if (string.IsNullOrEmpty(transactionCode))
            {
                _logger.LogWarning("Transaction code not found in content: {Content}", request.Content);
                return false;
            }

            var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.TransactionCode == transactionCode);
            if (transaction == null)
            {
                _logger.LogWarning("Transaction not found: {TransactionCode}", transactionCode);
                return false;
            }

            if (transaction.Amount != request.Amount)
            {
                _logger.LogWarning("Amount mismatch for {TransactionCode}: expected {Expected}, received {Received}",
                    transactionCode, transaction.Amount, request.Amount);
                return false;
            }

            if (request.IsSuccess && transaction.Status != "PAID")
            {
                transaction.Status = "PAID";
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Transaction {TransactionCode} marked as PAID", transactionCode);

                // TODO: Gửi thông báo/email
            }

            return true;
        }

        private string ExtractTransactionCode(string content)
        {
            var match = System.Text.RegularExpressions.Regex.Match(content, @"TXN\d+");
            return match.Success ? match.Value : string.Empty;
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int transactionId)
        {
            return await _dbContext.Transactions.FindAsync(transactionId);
        }
    }
}
