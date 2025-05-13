using Microsoft.AspNetCore.SignalR;

namespace LibraryManagement.Hubs
{
    public class LibraryHub : Hub
    {
        public async Task Notify(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
