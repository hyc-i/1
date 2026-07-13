using LibrarySeatReservation.Web.Models.ViewModels;

namespace LibrarySeatReservation.Web.Services;

public interface ISeatService
{
    Task<(int Total, int Available, int Occupied)> GetHomeStatsAsync();
    Task<List<SeatItem>> GetAllSeatsAsync(string? area);
    Task<SeatDetailViewModel?> GetSeatDetailAsync(int seatId);
    Task<List<string>> GetAreaOptionsAsync();
    Task<List<SeatItem>> GetAllSeatsForAdminAsync();
}
