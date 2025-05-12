namespace LibraryManagement.DTOs.Request
{
    public class RegisterRequest
    {
        public string FullName { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}
