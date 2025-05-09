using LibraryManagement.DTOs.Response;
using LibraryManagement.Models;
using System.Security.Claims;

namespace LibraryManagement.Services.Authentication
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token, bool validateLifetime = false);
        DateTime GetRefreshTokenExpiryTime();
        Task<TokenResponseDto> Authenticate(string email, string password);
        Task<TokenResponseDto?> RefreshTokenAsync(string accessToken, string refreshToken);
    }
}
