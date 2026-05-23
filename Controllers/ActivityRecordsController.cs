using System.Globalization;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondClassroomManager.Data;
using SecondClassroomManager.Models;

namespace SecondClassroomManager.Controllers;

[Authorize(Roles = "Admin,Student")]
public class ActivityRecordsController : Controller
{
    private readonly SecondClassroomRepository _repository;

    public ActivityRecordsController(SecondClassroomRepository repository)
    {
        _repository = repository;
    }

    private bool IsStudentOnly() => User.IsInRole("Student") && !User.IsInRole("Admin");

    [Authorize(Roles = "Admin")]
    public IActionResult Index(string? keyword, string? status, string? category)
    {
        ViewBag.Keyword = keyword ?? string.Empty;
        ViewBag.Status = status ?? string.Empty;
        ViewBag.Category = category ?? string.Empty;
        return View(_repository.GetActivityRecords(keyword, status, category));
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Export(string? keyword, string? status, string? category)
    {
        var records = _repository.GetActivityRecords(keyword, status, category);
        var builder = new StringBuilder();
        builder.AppendLine("学号,姓名,类别,活动名称,级别,组织单位,开始日期,结束日期,时长,状态,学分,审核意见,提交时间,审核时间");
        foreach (var item in records)
        {
            builder.AppendLine(string.Join(',', new[]
            {
                Csv(item.StudentNo),
                Csv(item.StudentName),
                Csv(item.Category),
                Csv(item.ActivityName),
                Csv(item.Level),
                Csv(item.Organizer),
                Csv(item.StartDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty),
                Csv(item.EndDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty),
                item.Hours.ToString("0.##", CultureInfo.InvariantCulture),
                Csv(item.Status),
                item.Credits.ToString("0.##", CultureInfo.InvariantCulture),
                Csv(item.ReviewOpinion),
                Csv(item.SubmittedAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)),
                Csv(item.ReviewedAt?.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) ?? string.Empty)
            }));
        }

        return CsvFile(builder, "activity-records.csv");
    }

    public IActionResult Details(int id)
    {
        var record = _repository.GetActivityRecord(id);
        if (record == null)
        {
            return NotFound();
        }

        if (!CanAccessRecord(record))
        {
            return Forbid();
        }

        ViewBag.ReviewLogs = _repository.GetReviewLogs(id);
        return View(record);
    }

    public IActionResult Create(int? studentId)
    {
        var isStudentOnly = IsStudentOnly();
        PopulateSelections();
        if (isStudentOnly && ViewBag.CurrentStudent == null)
        {
            return Forbid();
        }

        ViewBag.CanEditCredits = true;
        var record = new ActivityRecord
        {
            StudentId = isStudentOnly ? CurrentStudentId() : studentId ?? 0,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today,
            Hours = 1,
            Credits = 1
        };

        return View(record);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ActivityRecord record)
    {
        var isStudentOnly = IsStudentOnly();
        if (isStudentOnly)
        {
            record.StudentId = CurrentStudentId();
        }

        ValidateRecord(record, requirePositiveCredits: true);
        if (!ModelState.IsValid)
        {
            PopulateSelections();
            ViewBag.CanEditCredits = true;
            return View(record);
        }

        _repository.CreateActivityRecord(record);
        TempData["Success"] = "第二课堂登记记录已提交，等待审核";
        return isStudentOnly
            ? RedirectToAction("Index", "StudentPortal")
            : RedirectToAction(nameof(Index), new { status = ActivityOptions.PendingStatus });
    }

    public IActionResult Edit(int id)
    {
        var record = _repository.GetActivityRecord(id);
        if (record == null)
        {
            return NotFound();
        }

        if (!CanEditRecord(record))
        {
            return Forbid();
        }

        PopulateSelections();
        ViewBag.CanEditCredits = IsStudentOnly() && record.Status == ActivityOptions.PendingStatus;
        return View(record);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ActivityRecord record)
    {
        if (id != record.Id)
        {
            return BadRequest();
        }

        var existing = _repository.GetActivityRecord(id);
        if (existing == null)
        {
            return NotFound();
        }

        if (!CanEditRecord(existing))
        {
            return Forbid();
        }

        var isStudentOnly = IsStudentOnly();
        if (isStudentOnly)
        {
            record.StudentId = existing.StudentId;
        }
        else
        {
            record.Credits = existing.Credits;
        }

        record.Status = existing.Status;
        record.ReviewOpinion = existing.ReviewOpinion;
        record.ReviewedAt = existing.ReviewedAt;

        ValidateRecord(record, requirePositiveCredits: isStudentOnly);
        if (!ModelState.IsValid)
        {
            PopulateSelections();
            ViewBag.CanEditCredits = isStudentOnly && existing.Status == ActivityOptions.PendingStatus;
            return View(record);
        }

        _repository.UpdateActivityRecord(record);
        TempData["Success"] = "第二课堂登记记录已更新";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Review(int id)
    {
        var record = _repository.GetActivityRecord(id);
        if (record == null)
        {
            return NotFound();
        }

        return View(new ReviewActivityRecordViewModel
        {
            Id = id,
            Record = record,
            Status = record.Status == ActivityOptions.PendingStatus ? ActivityOptions.ApprovedStatus : record.Status,
            Credits = record.Credits,
            ReviewOpinion = record.ReviewOpinion
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Review(int id, ReviewActivityRecordViewModel model)
    {
        var record = _repository.GetActivityRecord(id);
        if (record == null)
        {
            return NotFound();
        }

        model.Id = id;
        model.Record = record;
        if (!ActivityOptions.Statuses.Contains(model.Status))
        {
            ModelState.AddModelError(nameof(ReviewActivityRecordViewModel.Status), "审核状态无效");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _repository.ReviewActivityRecord(id, model.Status, model.Credits, model.ReviewOpinion);
        TempData["Success"] = "审核结果已保存";
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        _repository.DeleteActivityRecord(id);
        TempData["Success"] = "第二课堂登记记录已删除";
        return RedirectToAction(nameof(Index));
    }

    private void PopulateSelections()
    {
        if (IsStudentOnly())
        {
            ViewBag.CurrentStudent = _repository.GetStudent(CurrentStudentId());
            ViewBag.Students = Array.Empty<Student>();
        }
        else
        {
            ViewBag.Students = _repository.GetStudents();
        }

        ViewBag.Categories = ActivityOptions.Categories;
        ViewBag.Levels = ActivityOptions.Levels;
    }

    private void ValidateRecord(ActivityRecord record, bool requirePositiveCredits)
    {
        if (record.StudentId <= 0 || _repository.GetStudent(record.StudentId) == null)
        {
            ModelState.AddModelError(nameof(ActivityRecord.StudentId), "请选择有效学生");
        }

        if (record.Hours <= 0)
        {
            ModelState.AddModelError(nameof(ActivityRecord.Hours), "请输入大于 0 的活动时长");
        }

        if (requirePositiveCredits && record.Credits <= 0)
        {
            ModelState.AddModelError(nameof(ActivityRecord.Credits), "请输入申请学分");
        }

        if (record.StartDate.HasValue && record.EndDate.HasValue && record.EndDate < record.StartDate)
        {
            ModelState.AddModelError(nameof(ActivityRecord.EndDate), "结束日期不能早于开始日期");
        }
    }

    private bool CanAccessRecord(ActivityRecord record)
    {
        return User.IsInRole("Admin") || IsStudentOnly() && record.StudentId == CurrentStudentId();
    }

    private bool CanEditRecord(ActivityRecord record)
    {
        if (User.IsInRole("Admin"))
        {
            return true;
        }

        return IsStudentOnly()
            && record.StudentId == CurrentStudentId()
            && record.Status == ActivityOptions.PendingStatus;
    }

    private int CurrentStudentId()
    {
        return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
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
