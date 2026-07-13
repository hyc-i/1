using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.DataAccess;
using LibrarySeatReservation.Web.Models.ViewModels;

namespace LibrarySeatReservation.Web.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;

    public AdminService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> ValidateLoginAsync(string username, string password)
    {
        var admin = await _db.Admins.FirstOrDefaultAsync(a => a.Username == username);
        if (admin == null) return false;

        return BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash);
    }

    public async Task<AdminStatisticsViewModel> GetStatisticsAsync()
    {
        var now = DateTime.UtcNow;
        var today = now.Date;

        var totalSeats = await _db.Seats.CountAsync(s => s.IsActive);

        var todayReservations = await _db.Reservations
            .CountAsync(r => r.ReservationDate == today && r.Status == "预约中");

        var occupiedToday = await _db.Reservations
            .Where(r => r.ReservationDate == today && r.Status == "预约中")
            .Select(r => r.SeatId)
            .Distinct()
            .CountAsync();

        var utilizationRate = totalSeats > 0 ? (double)occupiedToday / totalSeats * 100 : 0;

        var hotArea = await _db.Reservations
            .Where(r => r.Status == "预约中")
            .GroupBy(r => r.Seat!.Area)
            .Select(g => new { Area = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Select(x => x.Area)
            .FirstOrDefaultAsync();

        return new AdminStatisticsViewModel
        {
            TotalSeats = totalSeats,
            TodayReservations = todayReservations,
            UtilizationRate = Math.Round(utilizationRate, 1),
            HotArea = hotArea
        };
    }
}
