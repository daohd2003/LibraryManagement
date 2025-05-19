namespace LibraryManagement.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed
        public string TransactionCode { get; set; } = Guid.NewGuid().ToString("N");
        public DateTime CreatedAt { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}
