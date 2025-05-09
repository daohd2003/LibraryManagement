using LibraryManagement.DTOs.Response;
using LibraryManagement.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Net;
using System.Security.Authentication;
using System.Text.Json;

namespace LibraryManagement.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);

                // Handle non-exception error status codes
                if (!context.Response.HasStarted) // Critical check
                {
                    if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                    {
                        await WriteErrorAsync(context, StatusCodes.Status401Unauthorized,
                            "Token không hợp lệ hoặc thiếu token");
                    }
                    else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                    {
                        await WriteErrorAsync(context, StatusCodes.Status403Forbidden,
                            "Không có quyền truy cập tài nguyên này");
                    }
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task WriteErrorAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var errorDetails = new ErrorDetails
            {
                StatusCode = statusCode,
                Message = message
            };

            return context.Response.WriteAsync(errorDetails.ToString());
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = "Internal server error";

            switch (exception)
            {
                case DomainException httpEx:
                    statusCode = (int)httpEx.StatusCode;
                    message = httpEx.Message;
                    break;

                case SecurityTokenExpiredException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = "Token đã hết hạn";
                    break;

                case SecurityTokenValidationException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = "Token không hợp lệ";
                    break;

                case AuthenticationException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    message = "Xác thực thất bại";
                    break;

                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status403Forbidden;
                    message = "Không có quyền truy cập";
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var errorDetails = new ErrorDetails
            {
                StatusCode = statusCode,
                Message = message
            };

            return context.Response.WriteAsync(errorDetails.ToString());
        }
    }
}
