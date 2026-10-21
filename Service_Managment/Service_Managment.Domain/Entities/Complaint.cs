using System;
using Service_Managment.Domain.Enum;

namespace Service_Managment.Domain.Entities
{
    public class Complaint
    {
        public int ComplaintId { get; set; }
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
        public ComplaintStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}