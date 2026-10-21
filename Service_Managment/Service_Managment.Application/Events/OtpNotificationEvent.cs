using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Managment.Application.Events
{
    public class OtpNotificationEvent
    {
        public int AppointmentId { get; set; }
        public int UserId { get; set; }
        public string Otp { get; set; } = string.Empty;
    }
}
