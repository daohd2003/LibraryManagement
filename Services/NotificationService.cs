using Google;
using LibraryManagement.Data;
using LibraryManagement.Hubs;
using LibraryManagement.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Services
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly LibraryDbContext _context;

        public NotificationService(IHubContext<NotificationHub> hubContext, LibraryDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        public async Task SendDueBookNotificationsAsync()
        {
            var today = DateTime.Today;

            var dueSoonBooks = await _context.BorrowedBooks
                .Include(b => b.Book)
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
            }

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();

            foreach (var notification in notifications)
            {
                await _hubContext.Clients.User(notification.UserId.ToString())
                    .SendAsync("ReceiveNotification", notification);
            }
        }

    }
}
