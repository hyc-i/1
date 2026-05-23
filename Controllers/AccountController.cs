using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondClassroomManager.Data;
using SecondClassroomManager.Models;

namespace SecondClassroomManager.Controllers;

public class AccountController : Controller
{
    private const string AdminUsername = "admin";
    private const string AdminPassword = "123456";
    private const string DemoStudentNo = "2024010101";
    private const string DemoStudentPhone = "13800000001";
    private readonly SecondClassroomRepository _repository;

    public AccountController(SecondClassroomRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "Home");
        }

        if (User.IsInRole("Student"))
        {
            return RedirectToAction("Index", "StudentPortal");
        }

        return View();
    }

    public IActionResult StudentLogin(string? returnUrl = null)
    {
        return View(new LoginViewModel
        {
            Username = DemoStudentNo,
            Password = DemoStudentPhone,
            ReturnUrl = returnUrl ?? string.Empty
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StudentLogin(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var student = _repository.GetStudentByStudentNo(model.Username);
        if (student == null || !string.Equals(student.Phone.Trim(), model.Password.Trim(), StringComparison.Ordinal))
        {
            ModelState.AddModelError(string.Empty, "学号或手机号不正确");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, student.Id.ToString()),
            new(ClaimTypes.Name, student.Name),
            new(ClaimTypes.Role, "Student"),
            new("StudentNo", student.StudentNo)
        };

        await SignInAsync(claims);
        return RedirectToLocal(model.ReturnUrl, "StudentPortal", "Index");
    }

    public IActionResult AdminLogin(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl ?? string.Empty });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdminLogin(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!string.Equals(model.Username.Trim(), AdminUsername, StringComparison.OrdinalIgnoreCase)
            || !string.Equals(model.Password.Trim(), AdminPassword, StringComparison.Ordinal))
        {
            ModelState.AddModelError(string.Empty, "管理员账号或密码不正确");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "admin"),
            new(ClaimTypes.Name, "管理员"),
            new(ClaimTypes.Role, "Admin")
        };

        await SignInAsync(claims);
        return RedirectToLocal(model.ReturnUrl, "Home", "Index");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Denied()
    {
        return View();
    }

    private async Task SignInAsync(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
    }

    private IActionResult RedirectToLocal(string? returnUrl, string controller, string action)
    {
        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToAction(action, controller);
    }
}
