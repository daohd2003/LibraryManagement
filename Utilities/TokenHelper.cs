namespace LibraryManagement.Utilities
{
    public class TokenHelper
    {
        public static string? ExtractAccessToken(HttpContext httpContext)
        {
            var authHeader = httpContext.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            return authHeader.Substring("Bearer ".Length).Trim();
        }
    }
}
