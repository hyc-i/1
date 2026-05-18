using System.ComponentModel.DataAnnotations;

namespace SecondClassroomManager.Models;

public class ActivityRecord
{
    public int Id { get; set; }

    [Display(Name = "学生")]
    [Required(ErrorMessage = "请选择学生")]
    public int StudentId { get; set; }

    [Display(Name = "学号")]
    public string StudentNo { get; set; } = string.Empty;

    [Display(Name = "姓名")]
    public string StudentName { get; set; } = string.Empty;

    [Display(Name = "学院")]
    public string College { get; set; } = string.Empty;

    [Display(Name = "班级")]
    public string ClassName { get; set; } = string.Empty;

    [Display(Name = "活动类别")]
    [Required(ErrorMessage = "请选择活动类别")]
    [StringLength(40)]
    public string Category { get; set; } = string.Empty;

    [Display(Name = "活动名称")]
    [Required(ErrorMessage = "请输入活动名称")]
    [StringLength(120)]
    public string ActivityName { get; set; } = string.Empty;

    [Display(Name = "级别")]
    [StringLength(40)]
    public string Level { get; set; } = string.Empty;

    [Display(Name = "组织/颁发单位")]
    [StringLength(120)]
    public string Organizer { get; set; } = string.Empty;

    [Display(Name = "开始日期")]
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    [Display(Name = "结束日期")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    [Display(Name = "活动时长")]
    [Range(0, 999, ErrorMessage = "活动时长不能为负数")]
    public double Hours { get; set; }

    [Display(Name = "活动说明")]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "证明材料")]
    [StringLength(200)]
    public string Evidence { get; set; } = string.Empty;

    [Display(Name = "审核状态")]
    public string Status { get; set; } = ActivityOptions.PendingStatus;

    [Display(Name = "第二课堂学分")]
    [Range(0, 20, ErrorMessage = "学分范围为 0-20")]
    public double Credits { get; set; }

    [Display(Name = "审核意见")]
    [StringLength(500)]
    public string ReviewOpinion { get; set; } = string.Empty;

    [Display(Name = "提交时间")]
    public DateTime SubmittedAt { get; set; } = DateTime.Now;

    [Display(Name = "审核时间")]
    public DateTime? ReviewedAt { get; set; }
}
