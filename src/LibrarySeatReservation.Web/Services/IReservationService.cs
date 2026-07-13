using LibrarySeatReservation.Web.Models.ViewModels;

namespace LibrarySeatReservation.Web.Services;

public interface IReservationService
{
    Task<(bool Success, string ErrorMessage)> CreateReservationAsync(int seatId, string studentName, string studentIdentifier, DateTime date, DateTime startTime, DateTime endTime);
    Task<List<ReservationItem>> GetMyReservationsAsync(string studentIdentifier);
    Task<(bool Success, string ErrorMessage)> CancelMyReservationAsync(int reservationId, string studentIdentifier);
    Task<List<ReservationItem>> GetAllReservationsAsync(string? statusFilter);
    Task<(bool Success, string ErrorMessage)> CancelReservationAsAdminAsync(int reservationId);
}
