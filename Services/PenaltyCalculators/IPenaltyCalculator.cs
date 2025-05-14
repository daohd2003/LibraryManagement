using LibraryManagement.Models;

namespace LibraryManagement.Services.PenaltyCalculators
{
    public interface IPenaltyCalculator
    {
        decimal CalculatePenalty(BorrowedBook borrowedBook);
    }
}
