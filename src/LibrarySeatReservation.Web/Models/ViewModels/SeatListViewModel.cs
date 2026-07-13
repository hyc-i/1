using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibrarySeatReservation.Web.Models.ViewModels;

public class SeatListViewModel
{
    public List<SeatItem> Seats { get; set; } = new();
    public SelectList? AreaOptions { get; set; }
    public string? CurrentFilter { get; set; }
}
