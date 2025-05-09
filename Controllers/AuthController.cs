﻿using LibraryManagement.DTOs.Request;
using LibraryManagement.DTOs.Response;
using LibraryManagement.Repositories;
using LibraryManagement.Services;
using LibraryManagement.Services.Authentication;
using LibraryManagement.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryManagement.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly GoogleAuthService _googleAuthService;

        public AuthController(IJwtService jwtService, IUserService userService, GoogleAuthService googleAuthService)
        {
            _jwtService = jwtService;
            _userService = userService;
            _googleAuthService = googleAuthService;
        }

        // POST: /api/v1/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var tokenResponse = await _jwtService.Authenticate(request.Email, request.Password);
                return Ok(tokenResponse);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto request)
        {
            try
            {
                // Xác thực ID token với Google
                var payload = await _googleAuthService.VerifyGoogleTokenAsync(request.IdToken);
                if (payload == null)
                {
                    return Unauthorized(new { message = "Invalid Google token" });
                }

                // Kiểm tra xem người dùng có trong hệ thống không, nếu không thì tạo mới
                var user = await _userService.GetOrCreateUserAsync(payload);

                // Tạo access token và refresh token
                var tokens = _jwtService.GenerateToken(user);
                var refreshTokens = _jwtService.GenerateRefreshToken();
                var expiryTime = _jwtService.GetRefreshTokenExpiryTime();

                user.RefreshTokenExpiryTime = expiryTime;
                user.RefreshToken = refreshTokens;

                await _userService.UpdateAsync(user);

                return Ok(new TokenResponseDto
                {
                    Token = tokens,
                    RefreshToken = refreshTokens,
                    RefreshTokenExpiryTime = expiryTime
                });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu không xác thực được ID Token
                return Unauthorized(new { message = "Invalid or expired Google ID token", error = ex.Message });
            }
        }

        // POST: /api/v1/auth/refresh-token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var token = TokenHelper.ExtractAccessToken(HttpContext);

            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Access token is missing or invalid" });

            var result = await _jwtService.RefreshTokenAsync(token, request.RefreshToken);

            if (result == null)
                return Unauthorized(new { message = "Invalid or expired refresh token" });

            return Ok(result);
        }
    }
}
