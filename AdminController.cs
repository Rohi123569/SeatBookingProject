using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeatBookingProj.Data;
using SeatBookingProj.Models;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        var bookings = _context.Bookings
                        .Include(b => b.Employee)
                        .Include(b => b.Seat)
                        .ToList();
        return View(bookings);
        // return Json(new { success = true, data = bookings });

        //// Check if the user is an admin
        //if (User.IsInRole("Admin"))
        //{
        //    ViewBag.CanUpload = true; // Allow the upload option for admins
        //}
        //else
        //{
        //    ViewBag.CanUpload = false;
        //}

        //// Retrieve seat booking details or any other data needed for the dashboard
        //var seatBookings = _context.Seats.ToList(); // Example data fetching
        //return View(seatBookings);
    }



    [HttpGet]
    public IActionResult GetAllBookedSeats()
    {
        var bookedSeats = _context.Bookings
            .Select(b => new
            {
                BookingId = b.Id,
                EmployeeName = _context.Employees.FirstOrDefault(e => e.Id == b.EmployeeId).Name,
                SeatId = b.SeatId,
                BookingDate = b.BookingDate
            })
            .OrderBy(b => b.BookingId)
            .ToList();

        return Json(new { success = true, data = bookedSeats });
    }

    [HttpPost]
    public IActionResult DeleteBooking(int bookingId)
    {
        try
        {
            // Log the bookingId received from the frontend
            Console.WriteLine($"DeleteBooking called with bookingId: {bookingId}");

            // Query the database for the booking
            var booking = _context.Bookings
                .FirstOrDefault(b => b.Id == bookingId);

            if (booking == null)
            {
                Console.WriteLine($"Booking with ID {bookingId} not found.");
                return Json(new { success = false, error = "Booking not found." });
            }

            // Log the booking details
            Console.WriteLine($"Found booking: {booking.Id}, SeatId: {booking.SeatId}, EmployeeId: {booking.EmployeeId}");

            // Update the seat's IsBooked status
            var seat = _context.Seats.FirstOrDefault(s => s.Id == booking.SeatId);
            if (seat != null)
            {
                seat.IsBooked = false;
            }

            // Remove the booking
            _context.Bookings.Remove(booking);
            _context.SaveChanges();

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteBooking: {ex.Message}");
            return Json(new { success = false, error = "An internal error occurred. Please try again later." });
        }
    }


    // Assuming this is part of your Admin Controller

    [HttpPost]
    public IActionResult DeleteAllBookings()
    {
        var allBookings = _context.Bookings.Include(b=>b.Seat).ToList();

        if (allBookings.Any())
        {
            foreach (var booking in allBookings)
            {
                booking.Seat.IsBooked = false; // Mark all booked seats as available     
            }
            _context.Bookings.RemoveRange(allBookings);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        return Json(new { success = false, error = "No bookings to delete." });
    }


    //public IActionResult SeatBookingDetails()
    //{
    //    // Check if the user is an admin
    //    if (User.IsInRole("Admin"))
    //    {
    //        ViewBag.CanUpload = true; // Allow the upload option for admins
    //    }
    //    else
    //    {
    //        ViewBag.CanUpload = false;
    //    }

    //    // Retrieve seat booking details or any other data needed for the dashboard
    //    var seatBookings = _context.Seats.ToList(); // Example data fetching
    //    return View(seatBookings);
    //}




    public IActionResult UploadEmployees()
    {
        return View();
    }

    [HttpPost]
    public IActionResult UploadEmployees(IFormFile file)
    {
        //if (!User.IsInRole("Admin"))
        //{
        //    return RedirectToAction("AccessDenied", "Account");
        //}

        if (file != null && file.Length > 0)
        {
            using (var stream = file.OpenReadStream())
            {
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        var employee = new Employee
                        {
                            Name = row.Cell(1).GetValue<string>(),
                            Email = row.Cell(2).GetValue<string>(),
                            Password = row.Cell(3).GetValue<string>(),
                            Role = "Employee"
                        };
                        _context.Employees.Add(employee);
                    }
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Dashboard");
        }
        return View();
    }

    public IActionResult ManageSeats()
    {
        var seats = _context.Seats.ToList();
        return View(seats);
    }

    [HttpPost]
    public IActionResult AssignGuestSeat(int seatId, int employeeId)
    {
        var seat = _context.Seats.Find(seatId);
        if (seat != null && seat.Type == "Guest" && !seat.IsBooked)
        {
            seat.IsBooked = true;
            _context.Bookings.Add(new Booking
            {
                SeatId = seatId,
                EmployeeId = employeeId,
                BookingDate = DateTime.Now,
                IsTatkal = false
            });

            _context.SaveChanges();
        }

        return RedirectToAction("ManageSeats");
    }
}