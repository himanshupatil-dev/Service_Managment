using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Managment.Application.DTOs
{
    public class RegisterUserRequest
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string UserName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string UserEmail { get; set; } = string.Empty;

        [Required, RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit mobile number")]
        public string MobileNo { get; set; } = string.Empty;

        [Required, StringLength(250, MinimumLength = 5)]
        public string Address { get; set; } = string.Empty;

        [Range(-90, 90)]
        public double Latitude { get; set; }

        [Range(-180, 180)]
        public double Longitude { get; set; }

        [Required, StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}
