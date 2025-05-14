using System.Net;
using System.Net.Mail;
using LibraryManagement.Models;
using LibraryManagement.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(EmailNotification email)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");

        var host = smtpSettings["Host"];
        var port = int.Parse(smtpSettings["Port"]!);
        var username = smtpSettings["Username"];
        var password = smtpSettings["Password"];
        var fromName = smtpSettings["FromName"];

        var smtpClient = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(username, fromName),
            Subject = email.EmailSubject,
            Body = email.EmailBody,
            IsBodyHtml = true
        };

        mailMessage.To.Add(email.EmailAddress);

        try
        {
            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation("Email sent to {Email}", email.EmailAddress);
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "SMTP Error sending to {Email}. Status: {Message}", email.EmailAddress, ex.Message);
            throw new ApplicationException("Failed to send email due to SMTP error", ex);
        }
    }
}