using LibraryManagement.Models;
using LibraryManagement.Repositories;
using LibraryManagement.Services.Payments.Transactions;
using LibraryManagement.Services.PenaltyCalculators;

namespace LibraryManagement.Services
{
    public class PenaltyService : IPenaltyService
    {
        private readonly IPenaltyRepository _penaltyRepo;
        private readonly IPenaltyCalculatorFactory _factory;
        private readonly IBorrowedBookRepository _borrowedBookRepository;
        private readonly ILogger<PenaltyService> _logger;
        private readonly ITransactionService _transactionService;

        public PenaltyService(IPenaltyRepository penaltyRepo, IPenaltyCalculatorFactory factory, IBorrowedBookRepository borrowedBookRepository, ILogger<PenaltyService> logger, ITransactionService transactionService)
        {
            _penaltyRepo = penaltyRepo;
            _factory = factory;
            _borrowedBookRepository = borrowedBookRepository;
            _logger = logger;
            _transactionService = transactionService;
        }

        public async Task<Penalty> CreatePenaltyAsync(int borrowedBookId, string violationType)
        {
            var record = await _borrowedBookRepository.GetByIdWithDetailsAsync(borrowedBookId);

            if (record == null) throw new Exception("Record not found");

            var calculator = _factory.GetCalculator(violationType);
            var amount = calculator.CalculatePenalty(record);

            var penalty = new Penalty
            {
                BorrowedBookId = borrowedBookId,
                ViolationType = violationType,
                Amount = amount,
                Description = $"Phạt tự động ({violationType})",
                CreatedAt = DateTime.Now
            };

            var transaction = new Transaction
            {
                UserId = record.UserId,
                Amount = amount,
                PaymentMethod = "SEPAY",
                TransactionCode = $"PENALTY{DateTime.UtcNow.Ticks}",
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            await _transactionService.SaveTransactionAsync(transaction);

            penalty.TransactionId = transaction.Id;

            return await _penaltyRepo.AddAsync(penalty);
        }
    }
}
