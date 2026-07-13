using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LibrarySeatReservation.Web.Models.ViewModels;
using LibrarySeatReservation.Web.Services;

namespace LibrarySeatReservation.Web.Controllers;

public class SeatsController : Controller
{
    private readonly ISeatService _seatService;
    private readonly IStudentContext _studentContext;

    public SeatsController(ISeatService seatService, IStudentContext studentContext)
    {
        _seatService = seatService;
        _studentContext = studentContext;
    }

    public async Task<IActionResult> Index(string? area)
    {
        if (!_studentContext.IsLoggedIn)
            return RedirectToAction("Index", "Home");

        var seats = await _seatService.GetAllSeatsAsync(area);
        var areas = await _seatService.GetAreaOptionsAsync();

        var vm = new SeatListViewModel
        {
            Seats = seats,
            AreaOptions = new SelectList(areas, area),
            CurrentFilter = area
        };
        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!_studentContext.IsLoggedIn)
            return RedirectToAction("Index", "Home");

        var seat = await _seatService.GetSeatDetailAsync(id);
        if (seat == null) return NotFound();

        return View(seat);
    }
}
