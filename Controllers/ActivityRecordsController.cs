using Microsoft.AspNetCore.Mvc;
using SecondClassroomManager.Data;
using SecondClassroomManager.Models;

namespace SecondClassroomManager.Controllers;

public class ActivityRecordsController : Controller
{
    private readonly SecondClassroomRepository _repository;

    public ActivityRecordsController(SecondClassroomRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index(string? keyword, string? status, string? category)
    {
        ViewBag.Keyword = keyword ?? string.Empty;
        ViewBag.Status = status ?? string.Empty;
        ViewBag.Category = category ?? string.Empty;
        return View(_repository.GetActivityRecords(keyword, status, category));
    }

    public IActionResult Details(int id)
    {
        var record = _repository.GetActivityRecord(id);
        return record == null ? NotFound() : View(record);
    }

    public IActionResult Create(int? studentId)
    {
        PopulateSelections();
        return View(new ActivityRecord
        {
            StudentId = studentId ?? 0,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ActivityRecord record)
    {
        ValidateRecord(record);
        if (!ModelState.IsValid)
        {
            PopulateSelections();
            return View(record);
        }

        _repository.CreateActivityRecord(record);
        TempData["Success"] = "第二课堂登记记录已提交，等待审核";
        return RedirectToAction(nameof(Index), new { status = ActivityOptions.PendingStatus });
    }

    public IActionResult Edit(int id)
    {
        var record = _repository.GetActivityRecord(id);
        if (record == null)
        {
            return NotFound();
        }

        PopulateSelections();
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

        ValidateRecord(record);
        if (!ModelState.IsValid)
        {
            PopulateSelections();
            return View(record);
        }

        _repository.UpdateActivityRecord(record);
        TempData["Success"] = "第二课堂登记记录已更新";
        return RedirectToAction(nameof(Details), new { id });
    }

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
        ViewBag.Students = _repository.GetStudents();
        ViewBag.Categories = ActivityOptions.Categories;
        ViewBag.Levels = ActivityOptions.Levels;
    }

    private void ValidateRecord(ActivityRecord record)
    {
        if (record.StudentId <= 0 || _repository.GetStudent(record.StudentId) == null)
        {
            ModelState.AddModelError(nameof(ActivityRecord.StudentId), "请选择有效学生");
        }

        if (record.StartDate.HasValue && record.EndDate.HasValue && record.EndDate < record.StartDate)
        {
            ModelState.AddModelError(nameof(ActivityRecord.EndDate), "结束日期不能早于开始日期");
        }
    }
}
