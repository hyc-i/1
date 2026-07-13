using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LibrarySeatReservation.Web.Models.ViewModels;
using LibrarySeatReservation.Web.Services;

namespace LibrarySeatReservation.Web.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;
    private readonly IReservationService _reservationService;
    private readonly ISeatService _seatService;

    public AdminController(
        IAdminService adminService,
        IReservationService reservationService,
        ISeatService seatService)
    {
        _adminService = adminService;
        _reservationService = reservationService;
        _seatService = seatService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Reservations");

        return View(new AdminLoginViewModel());
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var valid = await _adminService.ValidateLoginAsync(model.Username, model.Password);
        if (!valid)
        {
            ModelState.AddModelError(string.Empty, "用户名或密码错误");
            return View(model);
        }

        var claims = new[] { new System.Security.Claims.Claim("username", model.Username) };
        var identity = new System.Security.Claims.ClaimsIdentity(claims, "AdminCookie");
        var principal = new System.Security.Claims.ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("AdminCookie", principal);

        return RedirectToAction("Reservations");
    }

    public async Task<IActionResult> Reservations(string? status)
    {
        var items = await _reservationService.GetAllReservationsAsync(status);
        var vm = new AdminReservationsViewModel
        {
            Items = items,
            StatusFilter = new SelectList(new[] { "预约中", "已取消" }, status),
            CurrentStatus = status
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await _reservationService.CancelReservationAsAdminAsync(id);
        TempData[result.Success ? "Success" : "Error"] = result.Success ? "已取消" : result.ErrorMessage;
        return RedirectToAction("Reservations");
    }

    public async Task<IActionResult> Statistics()
    {
        var vm = await _adminService.GetStatisticsAsync();
        return View(vm);
    }

    public async Task<IActionResult> Seats()
    {
        var seats = await _seatService.GetAllSeatsForAdminAsync();
        var areas = await _seatService.GetAreaOptionsAsync();
        var vm = new AdminSeatsViewModel
        {
            Seats = seats,
            AreaOptions = new SelectList(areas)
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("AdminCookie");
        return RedirectToAction("Index", "Home");
    }
}
