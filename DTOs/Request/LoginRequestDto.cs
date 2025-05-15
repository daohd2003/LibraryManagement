using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.DTOs.Request
{
    public class LoginRequestDto
    {
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        public string Email { get; set; } = String.Empty;
        [MinLength(6)]
        public string Password { get; set; } = String.Empty;
    }
}
