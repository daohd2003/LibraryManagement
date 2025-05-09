namespace LibraryManagement.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Member";
        public string AvatarUrl {  get; set; } = string.Empty;

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public string GoogleId { get; set; } = string.Empty;

        public ICollection<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();    
    }
}
