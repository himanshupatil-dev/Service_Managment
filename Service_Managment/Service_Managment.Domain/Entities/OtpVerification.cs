using System.ComponentModel.DataAnnotations;

namespace Service_Managment.Domain.Entities
{
    public class OtpVerification
    {
        [Key]
        public int OtpId { get; set; }
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;
        public string OtpHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public int FailedAttempts { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}