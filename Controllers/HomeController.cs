using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SecondClassroomManager.Data;
using SecondClassroomManager.Models;

namespace SecondClassroomManager.Controllers;

public class HomeController : Controller
{
    private readonly SecondClassroomRepository _repository;

    public HomeController(SecondClassroomRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction("Index", "Account");
        }

        if (User.IsInRole("Student"))
        {
            return RedirectToAction("Index", "StudentPortal");
        }

        return View(_repository.GetDashboardStats());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
