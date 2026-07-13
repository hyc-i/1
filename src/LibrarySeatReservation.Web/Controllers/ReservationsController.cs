using Microsoft.AspNetCore.Mvc;
using LibrarySeatReservation.Web.Models.ViewModels;
using LibrarySeatReservation.Web.Services;

namespace LibrarySeatReservation.Web.Controllers;

public class ReservationsController : Controller
{
    private readonly IReservationService _reservationService;
    private readonly ISeatService _seatService;
    private readonly IStudentContext _studentContext;

    public ReservationsController(
        IReservationService reservationService,
        ISeatService seatService,
        IStudentContext studentContext)
    {
        _reservationService = reservationService;
        _seatService = seatService;
        _studentContext = studentContext;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int seatId)
    {
        if (!_studentContext.IsLoggedIn)
            return RedirectToAction("Index", "Home");

        var seat = await _seatService.GetSeatDetailAsync(seatId);
        if (seat == null || !seat.IsAvailable)
        {
            TempData["Error"] = "该座位不可预约";
            return RedirectToAction("Index", "Seats");
        }

        var vm = new ReservationCreateViewModel
        {
            SeatId = seat.Id,
            SeatNumber = seat.SeatNumber,
            ReservationDate = DateTime.UtcNow.Date.AddDays(1),
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(10, 0, 0)
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReservationCreateViewModel model)
    {
        if (!_studentContext.IsLoggedIn)
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
            return View(model);

        var start = model.ReservationDate.Date + model.StartTime;
        var end = model.ReservationDate.Date + model.EndTime;

        var result = await _reservationService.CreateReservationAsync(
            model.SeatId,
            _studentContext.CurrentStudentName!,
            _studentContext.CurrentStudentIdentifier!,
            model.ReservationDate,
            start,
            end);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        TempData["Success"] = "预约成功";
        return RedirectToAction("My");
    }

    public async Task<IActionResult> My()
    {
        if (!_studentContext.IsLoggedIn)
            return RedirectToAction("Index", "Home");

        var items = await _reservationService.GetMyReservationsAsync(
            _studentContext.CurrentStudentIdentifier!);

        var vm = new ReservationListViewModel { Items = items };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        if (!_studentContext.IsLoggedIn)
            return RedirectToAction("Index", "Home");

        var result = await _reservationService.CancelMyReservationAsync(
            id, _studentContext.CurrentStudentIdentifier!);

        TempData[result.Success ? "Success" : "Error"] = result.Success ? "已取消" : result.ErrorMessage;
        return RedirectToAction("My");
    }
}
