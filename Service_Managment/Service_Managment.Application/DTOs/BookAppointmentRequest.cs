using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Managment.Application.DTOs
{
    public class BookAppointmentRequest
    {
        [Range(1, int.MaxValue)]
        public int WorkerId { get; set; }

        [Required, StringLength(1000, MinimumLength = 10)]
        public string ProblemDescription { get; set; } = string.Empty;

        public bool PartsRequired { get; set; }


        //public string OtpCode {  get; set; }
    }
}
