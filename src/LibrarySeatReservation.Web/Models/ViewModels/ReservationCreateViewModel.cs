using System.ComponentModel.DataAnnotations;

namespace LibrarySeatReservation.Web.Models.ViewModels;

public class ReservationCreateViewModel
{
    public int SeatId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "请选择日期")]
    [DataType(DataType.Date)]
    public DateTime ReservationDate { get; set; }

    [Required(ErrorMessage = "请选择开始时间")]
    [DataType(DataType.Time)]
    public TimeSpan StartTime { get; set; }

    [Required(ErrorMessage = "请选择结束时间")]
    [DataType(DataType.Time)]
    public TimeSpan EndTime { get; set; }
}
