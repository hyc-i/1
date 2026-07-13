using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.DataAccess;
using LibrarySeatReservation.Web.Models.Entities;
using LibrarySeatReservation.Web.Models.ViewModels;

namespace LibrarySeatReservation.Web.Services;

public class ReservationService : IReservationService
{
    private readonly AppDbContext _db;

    public ReservationService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(bool Success, string ErrorMessage)> CreateReservationAsync(
        int seatId, string studentName, string studentIdentifier,
        DateTime date, DateTime startTime, DateTime endTime)
    {
        var seat = await _db.Seats.FirstOrDefaultAsync(s => s.Id == seatId && s.IsActive);
        if (seat == null)
            return (false, "座位不存在或已停用");

        if (endTime <= startTime)
            return (false, "结束时间必须晚于开始时间");

        if (date.Date <= DateTime.UtcNow.Date)
            return (false, "只能预约明天的座位");

        var conflict = await _db.Reservations.AnyAsync(r =>
            r.SeatId == seatId &&
            r.ReservationDate == date.Date &&
            r.Status == "预约中" &&
            r.StartTime < endTime &&
            startTime < r.EndTime);

        if (conflict)
            return (false, "该时段座位已被预约");

        var reservation = new Reservation
        {
            SeatId = seatId,
            StudentName = studentName,
            StudentIdentifier = studentIdentifier,
            ReservationDate = date.Date,
            StartTime = startTime.Kind == DateTimeKind.Utc ? startTime : DateTime.SpecifyKind(startTime, DateTimeKind.Utc),
            EndTime = endTime.Kind == DateTimeKind.Utc ? endTime : DateTime.SpecifyKind(endTime, DateTimeKind.Utc),
            Status = "预约中",
            CreatedAt = DateTime.UtcNow
        };

        _db.Reservations.Add(reservation);
        await _db.SaveChangesAsync();

        return (true, string.Empty);
    }

    public async Task<List<ReservationItem>> GetMyReservationsAsync(string studentIdentifier)
    {
        return await _db.Reservations
            .Where(r => r.StudentIdentifier == studentIdentifier)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReservationItem
            {
                Id = r.Id,
                SeatNumber = r.Seat.SeatNumber,
                Area = r.Seat.Area,
                ReservationDate = r.ReservationDate,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                CanCancel = r.Status == "预约中"
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string ErrorMessage)> CancelMyReservationAsync(
        int reservationId, string studentIdentifier)
    {
        var reservation = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId);
        if (reservation == null)
            return (false, "预约记录不存在");

        if (reservation.StudentIdentifier != studentIdentifier)
            return (false, "只能取消自己的预约");

        if (reservation.Status != "预约中")
            return (false, "只能取消状态为预约中的记录");

        reservation.Status = "已取消";
        reservation.CancelledAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return (true, string.Empty);
    }

    public async Task<List<ReservationItem>> GetAllReservationsAsync(string? statusFilter)
    {
        var query = _db.Reservations.AsQueryable();

        if (!string.IsNullOrEmpty(statusFilter))
            query = query.Where(r => r.Status == statusFilter);

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReservationItem
            {
                Id = r.Id,
                SeatNumber = r.Seat.SeatNumber,
                Area = r.Seat.Area,
                ReservationDate = r.ReservationDate,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                CanCancel = r.Status == "预约中"
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string ErrorMessage)> CancelReservationAsAdminAsync(int reservationId)
    {
        var reservation = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId);
        if (reservation == null)
            return (false, "预约记录不存在");

        if (reservation.Status != "预约中")
            return (false, "该记录已取消");

        reservation.Status = "已取消";
        reservation.CancelledAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return (true, string.Empty);
    }
}
