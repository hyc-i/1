using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondClassroomManager.Data;

namespace SecondClassroomManager.Controllers;

[Authorize(Roles = "Admin")]
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

    public IActionResult Export(string? keyword)
    {
        var summaries = _repository.GetStudentSummaries(keyword);
        var builder = new StringBuilder();
        builder.AppendLine("学号,姓名,学院,专业,班级,登记总数,待审核数,通过数量,累计学分,达标状态,距离目标学分");
        foreach (var item in summaries)
        {
            builder.AppendLine(string.Join(',', new[]
            {
                Csv(item.StudentNo),
                Csv(item.Name),
                Csv(item.College),
                Csv(item.Major),
                Csv(item.ClassName),
                item.TotalRecords.ToString(CultureInfo.InvariantCulture),
                item.PendingRecords.ToString(CultureInfo.InvariantCulture),
                item.ApprovedRecords.ToString(CultureInfo.InvariantCulture),
                item.TotalCredits.ToString("0.##", CultureInfo.InvariantCulture),
                Csv(item.CreditStatus),
                item.RemainingCredits.ToString("0.##", CultureInfo.InvariantCulture)
            }));
        }

        return CsvFile(builder, "student-summary.csv");
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

    private static FileContentResult CsvFile(StringBuilder builder, string fileName)
    {
        var preamble = Encoding.UTF8.GetPreamble();
        var content = Encoding.UTF8.GetBytes(builder.ToString());
        return new FileContentResult(preamble.Concat(content).ToArray(), "text/csv; charset=utf-8")
        {
            FileDownloadName = fileName
        };
    }

    private static string Csv(string value)
    {
        var safeValue = value.Length > 0 && "=+-@".Contains(value[0]) ? "'" + value : value;
        return $"\"{safeValue.Replace("\"", "\"\"")}\"";
    }
}
