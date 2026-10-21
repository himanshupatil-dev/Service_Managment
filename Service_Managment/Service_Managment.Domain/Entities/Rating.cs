namespace Service_Managment.Domain.Entities
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int WorkerId { get; set; }
        public Worker Worker { get; set; }
        public int Score { get; set; }
        public string Review { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}