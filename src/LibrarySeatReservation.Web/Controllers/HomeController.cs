using Microsoft.AspNetCore.Mvc;
using LibrarySeatReservation.Web.Models.ViewModels;
using LibrarySeatReservation.Web.Services;

namespace LibrarySeatReservation.Web.Controllers;

public class HomeController : Controller
{
    private readonly ISeatService _seatService;
    private readonly IStudentContext _studentContext;

    private static readonly Dictionary<string, string> Students = new()
    {
        ["学生A"] = "student_a",
        ["学生B"] = "student_b",
        ["学生C"] = "student_c"
    };

    public HomeController(ISeatService seatService, IStudentContext studentContext)
    {
        _seatService = seatService;
        _studentContext = studentContext;
    }

    public async Task<IActionResult> Index()
    {
        var stats = await _seatService.GetHomeStatsAsync();
        var vm = new HomeIndexViewModel
        {
            TotalSeats = stats.Total,
            AvailableSeats = stats.Available,
            OccupiedSeats = stats.Occupied,
            IsLoggedIn = _studentContext.IsLoggedIn,
            CurrentStudentName = _studentContext.CurrentStudentName
        };
        return View(vm);
    }

    [HttpPost]
    public IActionResult Login(string name)
    {
        if (name != null && Students.TryGetValue(name, out var identifier))
            _studentContext.Login(name, identifier);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Logout()
    {
        _studentContext.Logout();
        return RedirectToAction(nameof(Index));
    }
}
