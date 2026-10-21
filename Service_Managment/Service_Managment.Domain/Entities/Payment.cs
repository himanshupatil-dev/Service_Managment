using System;
using Service_Managment.Domain.Enum;

namespace Service_Managment.Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        
        public PaymentStatus Status { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}