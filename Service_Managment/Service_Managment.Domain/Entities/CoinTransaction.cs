using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service_Managment.Domain.Entities
{
    internal class CoinTransaction
    {
        public int CoinTransactionId { get; set; }

        public int WorkerId { get; set; }

        public Worker Worker { get; set; }

        public Appointment Appointment { get; set; }
        public int AppointmentId { get; set; }

        public string reason { get; set; }

        public DateTime createdAt { get; set; }
    }
}
