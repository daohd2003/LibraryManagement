namespace LibraryManagement.DTOs.Request
{
    public class PenaltyRequestDto
    {
        public int BorrowedBookId { get; set; }
        public string ViolationType { get; set; } = string.Empty; // late, damaged, lost
    }
}
