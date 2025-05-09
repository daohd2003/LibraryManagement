namespace LibraryManagement.DTOs.Request
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = String.Empty;
        public string Issuer { get; set; } = String.Empty;
        public string Audience { get; set; } = String.Empty;
        public double ExpiryMinutes { get; set; }
        public double RefreshTokenExpiryDays { get; set; }
    }
}
