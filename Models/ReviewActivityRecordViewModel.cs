using System.ComponentModel.DataAnnotations;

namespace SecondClassroomManager.Models;

public class ReviewActivityRecordViewModel
{
    public int Id { get; set; }
    public ActivityRecord? Record { get; set; }

    [Display(Name = "审核状态")]
    [Required(ErrorMessage = "请选择审核状态")]
    public string Status { get; set; } = ActivityOptions.ApprovedStatus;

    [Display(Name = "第二课堂学分")]
    [Range(0, 20, ErrorMessage = "学分范围为 0-20")]
    public double Credits { get; set; }

    [Display(Name = "审核意见")]
    [StringLength(500)]
    public string ReviewOpinion { get; set; } = string.Empty;
}
