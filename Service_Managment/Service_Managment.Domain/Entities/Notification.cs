using System;
using Service_Managment.Domain.Enum;

namespace Service_Managment.Domain.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public NotificationChannel Channel { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
    }
}