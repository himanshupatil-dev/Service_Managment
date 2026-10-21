using Service_Managment.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Managment.Domain.Entities
{
    public class Worker
    {
        public int WorkerId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public WorkerCategory Category { get; set; }
        public string Qualification { get; set; } = string.Empty;
        public string License { get; set; } = string.Empty;
        public int Experience { get; set; }
        public double Rating { get; set; } = 0;
        public PricingModel PricingModel { get; set; }
        public decimal Price { get; set; }
        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
        public bool IsBlocked { get; set; } = false;
        public bool IsAvailable { get; set; } = true;
        public int CoinBalance { get; set; } = 0;
    }
}
