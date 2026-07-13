namespace LibrarySeatReservation.Web.Models.Entities;

public class Reservation
{
    public int Id { get; set; }
    public int SeatId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentIdentifier { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "预约中";
    public DateTime CreatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public Seat Seat { get; set; } = null!;
}
