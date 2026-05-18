using Microsoft.AspNetCore.Mvc;
using SecondClassroomManager.Data;

namespace SecondClassroomManager.Controllers;

public class AdminController : Controller
{
    private readonly SecondClassroomRepository _repository;

    public AdminController(SecondClassroomRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index(string? keyword)
    {
        ViewBag.Keyword = keyword ?? string.Empty;
        return View(_repository.GetStudentSummaries(keyword));
    }

    public IActionResult StudentRecords(int id)
    {
        var student = _repository.GetStudent(id);
        if (student == null)
        {
            return NotFound();
        }

        ViewBag.Student = student;
        return View(_repository.GetActivityRecords(studentId: id));
    }
}
