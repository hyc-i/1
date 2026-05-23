using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecondClassroomManager.Data;
using SecondClassroomManager.Models;

namespace SecondClassroomManager.Controllers;

[Authorize(Roles = "Admin")]
public class StudentsController : Controller
{
    private readonly SecondClassroomRepository _repository;

    public StudentsController(SecondClassroomRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index(string? keyword)
    {
        ViewBag.Keyword = keyword ?? string.Empty;
        return View(_repository.GetStudents(keyword));
    }

    public IActionResult Details(int id)
    {
        var student = _repository.GetStudent(id);
        if (student == null)
        {
            return NotFound();
        }

        ViewBag.Records = _repository.GetActivityRecords(studentId: id);
        return View(student);
    }

    public IActionResult Create()
    {
        return View(new Student());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Student student)
    {
        if (_repository.StudentNoExists(student.StudentNo))
        {
            ModelState.AddModelError(nameof(Student.StudentNo), "该学号已存在");
        }

        if (!ModelState.IsValid)
        {
            return View(student);
        }

        _repository.CreateStudent(student);
        TempData["Success"] = "学生档案已创建";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Edit(int id)
    {
        var student = _repository.GetStudent(id);
        return student == null ? NotFound() : View(student);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Student student)
    {
        if (id != student.Id)
        {
            return BadRequest();
        }

        if (_repository.StudentNoExists(student.StudentNo, student.Id))
        {
            ModelState.AddModelError(nameof(Student.StudentNo), "该学号已存在");
        }

        if (!ModelState.IsValid)
        {
            return View(student);
        }

        _repository.UpdateStudent(student);
        TempData["Success"] = "学生档案已更新";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        _repository.DeleteStudent(id);
        TempData["Success"] = "学生档案及关联第二课堂记录已删除";
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Import()
    {
        return View(new StudentImportResult());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Import(IFormFile? file)
    {
        var result = new StudentImportResult();
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("file", "请选择要导入的 CSV 或制表符文本文件");
            return View(result);
        }

        using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        var lineNumber = 0;
        while (!reader.EndOfStream)
        {
            lineNumber++;
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var fields = SplitImportLine(line).Select(x => x.Trim().Trim('"').Trim()).ToArray();
            if (IsHeader(fields))
            {
                continue;
            }

            if (fields.Length < 2 || string.IsNullOrWhiteSpace(fields[0]) || string.IsNullOrWhiteSpace(fields[1]))
            {
                result.Skipped++;
                result.Messages.Add($"第 {lineNumber} 行缺少学号或姓名，已跳过");
                continue;
            }

            var student = new Student
            {
                StudentNo = fields[0].TrimStart('﻿'),
                Name = fields[1],
                Gender = GetField(fields, 2),
                College = GetField(fields, 3),
                Major = GetField(fields, 4),
                ClassName = GetField(fields, 5),
                Phone = GetField(fields, 6),
                Email = GetField(fields, 7)
            };

            if (_repository.UpsertStudentByNo(student))
            {
                result.Inserted++;
            }
            else
            {
                result.Updated++;
            }
        }

        TempData["Success"] = $"导入完成：新增 {result.Inserted} 人，更新 {result.Updated} 人，跳过 {result.Skipped} 行";
        return View(result);
    }

    private static string[] SplitImportLine(string line)
    {
        return line.Contains('\t') ? line.Split('\t') : line.Split(',');
    }

    private static bool IsHeader(string[] fields)
    {
        if (fields.Length == 0)
        {
            return false;
        }

        var first = fields[0].TrimStart('﻿');
        return first.Contains("学号", StringComparison.OrdinalIgnoreCase)
            || first.Equals("StudentNo", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetField(string[] fields, int index)
    {
        return fields.Length > index ? fields[index] : string.Empty;
    }
}
