﻿namespace LibraryManagement.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; } = false;

        public virtual User User { get; set; } = null!;
    }
}
