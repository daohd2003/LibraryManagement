namespace LibraryManagement.Models
{
    public class BorrowedBook
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; } = "Borrowed"; // Borrowed, Returned, Overdue

        public virtual Book Book { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
