using Microsoft.AspNetCore.Mvc;
using SeatBookingProj.Data;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = _context.Employees.SingleOrDefault(e => e.Email == email && e.Password == password);

        if (user != null)
        {
            HttpContext.Session.SetString("UserRole", user.Role);
            HttpContext.Session.SetInt32("UserId", user.Id);

            return user.Role == "Admin"
                ? RedirectToAction("Dashboard", "Admin")
                : RedirectToAction("Dashboard", "Employee");
        }

        ViewBag.Error = "Invalid credentials.";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}