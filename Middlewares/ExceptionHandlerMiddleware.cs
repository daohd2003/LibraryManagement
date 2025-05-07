using LibraryManagement.DTOs.Response;
using LibraryManagement.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;
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
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = "Internal server error";

            if (exception is DomainException httpEx)
            {
                statusCode = (int)httpEx.StatusCode;
                message = httpEx.Message;
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
