using System;
using System.Collections.Generic;
using Service_Managment.Domain.Enum;
using Service_Managment.Domain.Enum.Service_Managment.Domain.Enum;

namespace Service_Managment.Domain.Entities
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int WorkerId { get; set; }
        public Worker Worker { get; set; }              
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public Status Status { get; set; }
        public bool PartsRequired { get; set; }
        public string ProblemDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public string Otp { get; set; }
        public ICollection<OtpVerification> OtpVerifications { get; set; }
    = new List<OtpVerification>();
        public Complaint? Complaint { get; set; }
        public string FailureReason { get; set; } = string.Empty;

        public Rating? RatingDetails { get; set; }
        public string OtpHash { get; set; }
        public DateTime OtpExpiresAt { get; set; }
    }
}