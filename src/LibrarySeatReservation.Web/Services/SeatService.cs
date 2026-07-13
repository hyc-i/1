using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.DataAccess;
using LibrarySeatReservation.Web.Models.ViewModels;

namespace LibrarySeatReservation.Web.Services;

public class SeatService : ISeatService
{
    private readonly AppDbContext _db;

    public SeatService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(int Total, int Available, int Occupied)> GetHomeStatsAsync()
    {
        var now = DateTime.UtcNow;
        var total = await _db.Seats.CountAsync(s => s.IsActive);

        var occupied = await _db.Reservations
            .CountAsync(r => r.Status == "预约中" && r.StartTime <= now && r.EndTime >= now);

        return (total, total - occupied, occupied);
    }

    public async Task<List<SeatItem>> GetAllSeatsAsync(string? area)
    {
        var now = DateTime.UtcNow;
        var query = _db.Seats.Where(s => s.IsActive);

        if (!string.IsNullOrEmpty(area))
            query = query.Where(s => s.Area == area);

        var seats = await query.OrderBy(s => s.SeatNumber).ToListAsync();

        var occupiedSeatIds = await _db.Reservations
            .Where(r => r.Status == "预约中" && r.StartTime <= now && r.EndTime > now)
            .Select(r => r.SeatId)
            .Distinct()
            .ToListAsync();

        return seats.Select(s => new SeatItem
        {
            Id = s.Id,
            SeatNumber = s.SeatNumber,
            Area = s.Area,
            Description = s.Description,
            Status = occupiedSeatIds.Contains(s.Id) ? "已预约" : "空闲"
        }).ToList();
    }

    public async Task<SeatDetailViewModel?> GetSeatDetailAsync(int seatId)
    {
        var seat = await _db.Seats.FirstOrDefaultAsync(s => s.Id == seatId && s.IsActive);
        if (seat == null) return null;

        var now = DateTime.UtcNow;
        var isOccupied = await _db.Reservations
            .AnyAsync(r => r.SeatId == seatId && r.Status == "预约中" && r.StartTime <= now && r.EndTime > now);

        return new SeatDetailViewModel
        {
            Id = seat.Id,
            SeatNumber = seat.SeatNumber,
            Area = seat.Area,
            Floor = seat.Floor,
            Description = seat.Description,
            IsAvailable = !isOccupied
        };
    }

    public async Task<List<string>> GetAreaOptionsAsync()
    {
        return await _db.Seats
            .Where(s => s.IsActive)
            .Select(s => s.Area)
            .Distinct()
            .OrderBy(a => a)
            .ToListAsync();
    }

    public async Task<List<SeatItem>> GetAllSeatsForAdminAsync()
    {
        var now = DateTime.UtcNow;
        var seats = await _db.Seats.OrderBy(s => s.SeatNumber).ToListAsync();

        var occupiedSeatIds = await _db.Reservations
            .Where(r => r.Status == "预约中" && r.StartTime <= now && r.EndTime > now)
            .Select(r => r.SeatId)
            .Distinct()
            .ToListAsync();

        return seats.Select(s => new SeatItem
        {
            Id = s.Id,
            SeatNumber = s.SeatNumber,
            Area = s.Area,
            Description = s.Description,
            Status = s.IsActive ? (occupiedSeatIds.Contains(s.Id) ? "已预约" : "空闲") : "已停用"
        }).ToList();
    }
}
