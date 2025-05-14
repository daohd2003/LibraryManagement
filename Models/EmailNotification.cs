namespace LibraryManagement.Models
{
    public class EmailNotification : Notification
    {
        public string EmailAddress { get; set; } = String.Empty;
        public string EmailSubject { get; set; } = String.Empty;
        public string EmailBody { get; set; } = String.Empty;
    }
}
