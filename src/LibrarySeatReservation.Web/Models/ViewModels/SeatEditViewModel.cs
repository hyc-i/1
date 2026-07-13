using System.ComponentModel.DataAnnotations;

namespace LibrarySeatReservation.Web.Models.ViewModels;

public class SeatEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "请输入座位编号")]
    [Display(Name = "座位编号")]
    public string SeatNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "请选择区域")]
    [Display(Name = "区域")]
    public string Area { get; set; } = string.Empty;

    [Display(Name = "楼层")]
    public string? Floor { get; set; }

    [Display(Name = "描述")]
    public string? Description { get; set; }

    [Display(Name = "是否启用")]
    public bool IsActive { get; set; } = true;
}
