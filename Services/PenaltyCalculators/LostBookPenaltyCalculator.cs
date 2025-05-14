using LibraryManagement.Models;

namespace LibraryManagement.Services.PenaltyCalculators
{
    public class LostBookPenaltyCalculator : IPenaltyCalculator
    {
        private const decimal HandlingFee = 10000;

        public decimal CalculatePenalty(BorrowedBook borrowedBook)
        {
            return HandlingFee + borrowedBook.Book.Price;
        }
    }
}
