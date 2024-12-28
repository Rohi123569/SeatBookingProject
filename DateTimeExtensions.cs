using System;

namespace SeatBookingProj.Utilities
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt,DayOfWeek startDay = DayOfWeek.Monday)
        {
            int diff = (7 + (dt.DayOfWeek - startDay)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startDay = DayOfWeek.Monday)
        {
           
            return dt.StartOfWeek(startDay).AddDays(6);
        }
    }
}
