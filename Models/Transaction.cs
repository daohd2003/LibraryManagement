namespace LibraryManagement.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; } = "PENDING"; // PENDING/PAID/EXPIRED
        public string TransactionCode { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}
