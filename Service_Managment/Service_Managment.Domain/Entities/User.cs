using Service_Managment.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Managment.Domain.Entities
{
    public class User
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; } 

        public string MobileNo { get; set; }

        public string OTP { get; set; } = string.Empty;

        public string Address { get; set; }

        public DateTime OTPExpiresAt { get; set; }

        public bool IsMobileVerified { get; set; }

        public string PasswordHash { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public DateTime CreatedAt { get; set; }

        public Admin AdminProfile  { get; set; }

        public Worker WorkerProfile { get; set; }

        public UserRole Role { get; set; }


    }
}
