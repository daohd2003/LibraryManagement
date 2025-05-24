namespace LibraryManagement.Models
{
    public class SearchHistory
    {
        public int Id { get; set; }
        public string Query { get; set; } = string.Empty;
        public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
        public int? UserId { get; set; } 
        public string? ClientIdentifier { get; set; }

        public User? User { get; set; }
    }
}
