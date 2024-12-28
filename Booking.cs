
namespace SeatBookingProj.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        //public int SeatId { get; set; }

        public int SeatId { get; set; }
        public Seat Seat { get; set; }
        public DateTime BookingDate { get; set; }
        public string TimeSlot { get; set; }
        public bool IsTatkal { get; set; }
    }
}
