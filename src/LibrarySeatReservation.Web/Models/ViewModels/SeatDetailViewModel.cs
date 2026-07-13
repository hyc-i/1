namespace LibrarySeatReservation.Web.Models.ViewModels;

public class SeatDetailViewModel
{
    public int Id { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string? Floor { get; set; }
    public string? Description { get; set; }
    public bool IsAvailable { get; set; }
}
