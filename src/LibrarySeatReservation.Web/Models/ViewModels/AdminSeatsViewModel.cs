using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibrarySeatReservation.Web.Models.ViewModels;

public class AdminSeatsViewModel
{
    public List<SeatItem> Seats { get; set; } = new();
    public SelectList? AreaOptions { get; set; }
    public SeatEditViewModel NewSeat { get; set; } = new();
}
