using Service_Managment.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Service_Managment.Application.DTOs
{
    public class RegisterWorkerRequest
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string WorkerName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string WorkerEmail { get; set; } = string.Empty;

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

        [EnumDataType(typeof(WorkerCategory))]
        public WorkerCategory Category { get; set; }

        [Required, StringLength(200)]
        public string Qualification { get; set; } = string.Empty;

        [StringLength(100)]   // optional, per your requirements
        public string License { get; set; } = string.Empty;

        [Range(0, 60)]
        public int Experience { get; set; }

        [EnumDataType(typeof(PricingModel))]
        public PricingModel PricingModel { get; set; }

        [Range(typeof(decimal), "1", "1000000")]
        public decimal Price { get; set; }
    }
}