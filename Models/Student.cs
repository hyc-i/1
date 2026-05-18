using System.ComponentModel.DataAnnotations;

namespace SecondClassroomManager.Models;

public class Student
{
    public int Id { get; set; }

    [Display(Name = "学号")]
    [Required(ErrorMessage = "请输入学号")]
    [StringLength(30)]
    public string StudentNo { get; set; } = string.Empty;

    [Display(Name = "姓名")]
    [Required(ErrorMessage = "请输入姓名")]
    [StringLength(40)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "性别")]
    [StringLength(10)]
    public string Gender { get; set; } = string.Empty;

    [Display(Name = "学院")]
    [StringLength(80)]
    public string College { get; set; } = string.Empty;

    [Display(Name = "专业")]
    [StringLength(80)]
    public string Major { get; set; } = string.Empty;

    [Display(Name = "班级")]
    [StringLength(60)]
    public string ClassName { get; set; } = string.Empty;

    [Display(Name = "联系电话")]
    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Display(Name = "电子邮箱")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "建档时间")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
