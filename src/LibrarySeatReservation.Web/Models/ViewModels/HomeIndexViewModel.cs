namespace LibrarySeatReservation.Web.Models.ViewModels;

public class HomeIndexViewModel
{
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public int OccupiedSeats { get; set; }
    public bool IsLoggedIn { get; set; }
    public string? CurrentStudentName { get; set; }
}
