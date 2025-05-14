using LibraryManagement.Models;

namespace LibraryManagement.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailNotification email);
    }
}
