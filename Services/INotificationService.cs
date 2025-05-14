namespace LibraryManagement.Services
{
    public interface INotificationService
    {
        Task SendDueBookNotificationsAsync();
    }
}
