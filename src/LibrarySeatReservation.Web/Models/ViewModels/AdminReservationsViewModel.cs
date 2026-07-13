using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibrarySeatReservation.Web.Models.ViewModels;

public class AdminReservationsViewModel
{
    public List<ReservationItem> Items { get; set; } = new();
    public SelectList? StatusFilter { get; set; }
    public string? CurrentStatus { get; set; }
}
