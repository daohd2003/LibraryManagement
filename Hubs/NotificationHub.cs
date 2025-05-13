using LibraryManagement.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace LibraryManagement.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
            _logger.LogInformation("NotificationHub initialized");
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation($"UserID: {userId} connected with connectionId: {Context.ConnectionId}");

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                _logger.LogInformation($"User {userId} added to group {userId}.");
            }
            await base.OnConnectedAsync();
        }

        public async Task SendNotification(string userId, Notification notification)
        {
            _logger.LogInformation($"Sending notification to user {userId}: {notification.Title}");

            // Gửi thông báo đến user cụ thể
            await Clients.User(userId).SendAsync("ReceiveNotification", notification);
        }
    }
}
