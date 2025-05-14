using Google;
using LibraryManagement.Data;
using LibraryManagement.Hubs;
using LibraryManagement.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly LibraryDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IHubContext<NotificationHub> hubContext, LibraryDbContext context, IEmailService emailService, ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task SendDueBookNotificationsAsync()
        {
            try
            {
                var today = DateTime.Today;

                var dueSoonBooks = await _context.BorrowedBooks
                    .Include(b => b.Book)
                    .Include(b => b.User)
                    .Where(b => b.DueDate >= today && b.DueDate <= today.AddDays(3))
                    .ToListAsync();

                var notifications = new List<Notification>();

                foreach (var book in dueSoonBooks)
                {
                    var notification = new Notification
                    {
                        UserId = book.UserId,
                        Title = "Sắp đến hạn trả sách",
                        Message = $"Sách \"{book.Book.Title}\" sẽ đến hạn vào {book.DueDate:dd/MM/yyyy}.",
                        SentAt = DateTime.Now
                    };

                    notifications.Add(notification);

                    var emailNoti = new EmailNotification
                    {
                        UserId = book.UserId,
                        EmailAddress = book.User.Email,
                        EmailSubject = "Thông báo: Sách đến hạn trả",
                        EmailBody = $@"
                        <p>Xin chào {book.User.Username},</p>
                        <p>Sách <strong>{book.Book.Title}</strong> bạn đang mượn sẽ đến hạn vào <em>{book.DueDate:dd/MM/yyyy}</em>.</p>
                        <p>Vui lòng trả sách đúng hạn để tránh phí trễ hạn.</p>
                        <p>Thân mến,<br/>Thư viện của bạn</p>"
                    };

                    await _emailService.SendEmailAsync(emailNoti);

                }

                _context.Notifications.AddRange(notifications);
                await _context.SaveChangesAsync();

                foreach (var notification in notifications)
                {
                    await _hubContext.Clients.Group(notification.UserId.ToString())
                        .SendAsync("ReceiveNotification", notification);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendDueBookNotificationsAsync");
                throw;
            }
        }

    }
}
