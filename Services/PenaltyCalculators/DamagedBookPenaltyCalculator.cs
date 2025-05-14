using LibraryManagement.Models;

namespace LibraryManagement.Services.PenaltyCalculators
{
    public class DamagedBookPenaltyCalculator : IPenaltyCalculator
    {
        public decimal CalculatePenalty(BorrowedBook borrowedBook)
        {
            return borrowedBook.Book.Price * 0.3m;
        }
    }
}
