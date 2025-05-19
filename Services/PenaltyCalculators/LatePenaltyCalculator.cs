using LibraryManagement.Enums;
using LibraryManagement.Models;

namespace LibraryManagement.Services.PenaltyCalculators
{
    public class LatePenaltyCalculator : IPenaltyCalculator
    {
        private const decimal PenaltyPerDay = 2000;

        public decimal CalculatePenalty(BorrowedBook borrowedBook)
        {
            if (borrowedBook.Status == BorrowStatus.Overdue.ToString())
            {
                var lateDays = (borrowedBook.ReturnDate - borrowedBook.DueDate).Days;
                return lateDays * PenaltyPerDay;
            }
            return 0;
        }
    }
}
