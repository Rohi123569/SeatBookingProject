namespace SeatBookingProj.Models
{
    public class Seat
    {
        public int Id { get; set; }
        //public int LocationId { get; set; }
        //public string SeatId { get; set; }
        public string SeatNumber { get; set; }
        public string Type { get; set; } // "Employee" or "Guest"
        public bool IsBooked { get; set; }
        public string Location { get; set; }

      
    }
}
