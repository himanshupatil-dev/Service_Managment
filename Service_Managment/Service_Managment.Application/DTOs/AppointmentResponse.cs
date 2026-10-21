using Service_Managment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Managment.Application.DTOs
{
    public class AppointmentResponse
    {
        public int AppointmentId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int WorkerId { get; set; }
        public string WorkerName { get; set; } = string.Empty;
        public string ProblemDescription { get; set; } = string.Empty;
        public bool PartsRequired { get; set; }
        public string? FailureReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? Rating { get; set; }
    }
}
