namespace LibraryManagement.Models
{
    public class BlacklistedToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiredAt { get; set; }
    }
}
