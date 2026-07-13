namespace LibrarySeatReservation.Web.Models.ViewModels;

public class AdminStatisticsViewModel
{
    public int TotalSeats { get; set; }
    public int TodayReservations { get; set; }
    public double UtilizationRate { get; set; }
    public string? HotArea { get; set; }
}
