using LibrarySeatReservation.Web.Models.ViewModels;

namespace LibrarySeatReservation.Web.Services;

public interface ISeatService
{
    Task<(int Total, int Available, int Occupied)> GetHomeStatsAsync();
    Task<List<SeatItem>> GetAllSeatsAsync(string? area);
    Task<SeatDetailViewModel?> GetSeatDetailAsync(int seatId);
    Task<List<string>> GetAreaOptionsAsync();
    Task<List<SeatItem>> GetAllSeatsForAdminAsync();
    Task<(bool Success, string ErrorMessage)> CreateSeatAsync(string seatNumber, string area, string? floor, string? description);
    Task<(bool Success, string ErrorMessage)> UpdateSeatAsync(int id, string seatNumber, string area, string? floor, string? description, bool isActive);
    Task<(bool Success, string ErrorMessage)> DeleteSeatAsync(int id);
    Task<SeatEditViewModel?> GetSeatForEditAsync(int id);
}
