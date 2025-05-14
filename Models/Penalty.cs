namespace LibraryManagement.Models
{
    public class Penalty
    {
        public int Id { get; set; }
        public int BorrowedBookId { get; set; }
        public string ViolationType { get; set; } // "Late", "Damaged", "Lost"
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual BorrowedBook BorrowedBook { get; set; } = null!;
    }
}
