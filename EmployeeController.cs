using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeatBookingProj.Data;
using SeatBookingProj.Models;
using System.Linq;

public class EmployeeController : Controller
{
    private readonly ApplicationDbContext _context;

    public EmployeeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard(string location = null)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var seats = _context.Seats.Where(s => s.Location == location &&
                    s.Type == "Employee").OrderBy(s=>s.SeatNumber).ToList();
            
             

        ViewBag.BookingHistory = _context.Bookings
            .Where(b => b.EmployeeId == userId)
            .Include(b => b.Seat)
            .OrderByDescending(b => b.BookingDate)
            .ToList();

        
        // Static list of locations (can be replaced with dynamic data from the database)
        ViewBag.SelectedLocation = location;
        ViewBag.Locations = new List<string> { "CKC-Chennai", "Mepz-Chennai" };
        ViewBag.AvailableSeats = seats.Count(s => !s.IsBooked);
        ViewBag.BookedSeats = seats.Count(s => s.IsBooked);

        return View(seats);
    }



    [HttpPost]
    public IActionResult BookSeats([FromBody] List<SeatBookingRequest> requests)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
        {
            return Json(new { success = false, message = "You must be logged in to book seats." });
        }

        // Group requests by date to check for multiple bookings on the same date
        var groupedRequests = requests.GroupBy(r => r.BookingDate.Date);

        foreach (var group in groupedRequests)
        {
            if (group.Count() > 1)
            {
                return Json(new { success = false, message = "You cannot book multiple seats for the same date." });
            }
        }

        foreach (var request in requests)
        {
            // Log the received seat ID
            Console.WriteLine($"Received Seat ID: {request.SeatIds}");

            // Check if booking more than 5 seats in advance for any given week
            var startOfWeek = request.BookingDate.AddDays(-(int)request.BookingDate.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(6);
            var bookingsThisWeek = _context.Bookings
                .Where(b => b.EmployeeId == userId && b.BookingDate >= startOfWeek && b.BookingDate <= endOfWeek)
                .Count();

            if (bookingsThisWeek + requests.Count > 5)
            {
                return Json(new { success = false, message = "You cannot book more than 5 seats in a week." });
            }

            // Check if the employee has already booked a seat on the selected date
            var existingBooking = _context.Bookings
                .FirstOrDefault(b => b.EmployeeId == userId && b.BookingDate.Date == request.BookingDate.Date);

            if (existingBooking != null)
            {
                return Json(new { success = false, message = "You have already booked a seat on this date." });
            }

            // Validate each seat
            var seat = _context.Seats.FirstOrDefault(s => s.Id == request.SeatIds && !s.IsBooked);
            if (seat == null)
            {
                return Json(new { success = false, message = $"Seat with ID {request.SeatIds} is not available." });
            }

            seat.IsBooked = true;
            _context.Bookings.Add(new Booking
            {
                EmployeeId = userId.Value,
                SeatId = request.SeatIds,
                BookingDate = request.BookingDate,
                TimeSlot = request.TimeSlot // Save the time slot
            });
        }

        _context.SaveChanges();
        return Json(new { success = true });
    }

    [HttpGet]
    public IActionResult BookingHistory()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var bookings = _context.Bookings
                        .Where(b => b.EmployeeId == userId)
                        .Include(b => b.Seat)
                        .ToList();
        return View(bookings);
    }

    [HttpGet]
    public IActionResult GetBookingDetails(int seatId)
    {
        var booking = _context.Bookings.
            Include(b=>b.Employee).FirstOrDefault(b=>b.SeatId == seatId);

        if(booking != null)
        {
            return Json(new
            {
                employeeName = booking.Employee.Name,
                date = booking.BookingDate.ToShortDateString(),
                time = booking.BookingDate.ToShortTimeString()
            });
        }
        return Json(null);
    }


    [HttpGet]
    public IActionResult GetSeatStatuses()
    {
        var seats = _context.Seats.Select(seat => new
        {
            seat.Id,
            status = _context.Bookings.Any(b => b.SeatId == seat.Id)? "Booked" : "Available"
        }).ToList();
        return Json(seats);
    }
}