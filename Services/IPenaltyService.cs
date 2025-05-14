using LibraryManagement.Models;

namespace LibraryManagement.Services
{
    public interface IPenaltyService
    {
        Task<Penalty> CreatePenaltyAsync(int borrowedBookId, string violationType);
    }
}
