namespace LibrarySeatReservation.Web.Models.ViewModels;

public class SeatItem
{
    public int Id { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "空闲";
}
