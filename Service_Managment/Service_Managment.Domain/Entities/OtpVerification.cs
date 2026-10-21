using System;
using System.ComponentModel.DataAnnotations;
using Service_Managment.Domain.Enum;

namespace Service_Managment.Domain.Entities
{
    public class OtpVerification
    {
        [Key]
        public int OtpId { get; set; }
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public string Code { get; set; }
        public bool IsVerified { get; set; }
        public int? VerifiedBy { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}