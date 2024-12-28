namespace SeatBookingProj.Models
{
    public class SeatBookingRequest
    {
        public int SeatIds { get; set; }
        public DateTime BookingDate { get; set; }
        public string TimeSlot { get; set; }
    }
}
