using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondClassroomManager.Data;

namespace SecondClassroomManager.Controllers;

[Authorize(Roles = "Student")]
public class StudentPortalController : Controller
{
    private readonly SecondClassroomRepository _repository;

    public StudentPortalController(SecondClassroomRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        var studentId = CurrentStudentId();
        if (studentId == 0)
        {
            return RedirectToAction("StudentLogin", "Account");
        }

        var student = _repository.GetStudent(studentId);
        if (student == null)
        {
            return NotFound();
        }

        ViewBag.Records = _repository.GetActivityRecords(studentId: studentId);
        return View(student);
    }

    private int CurrentStudentId()
    {
        return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
    }
}
