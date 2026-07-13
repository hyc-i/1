using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Admin> Admins => Set<Admin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Seat>(e =>
        {
            e.ToTable("Seats");
            e.HasKey(s => s.Id);
            e.Property(s => s.SeatNumber).HasMaxLength(20).IsRequired();
            e.Property(s => s.Area).HasMaxLength(50).IsRequired();
            e.Property(s => s.Floor).HasMaxLength(20);
            e.Property(s => s.Description).HasMaxLength(200);
            e.HasIndex(s => s.SeatNumber).IsUnique().HasDatabaseName("IX_Seats_SeatNumber");
        });

        modelBuilder.Entity<Reservation>(e =>
        {
            e.ToTable("Reservations");
            e.HasKey(r => r.Id);
            e.Property(r => r.StudentName).HasMaxLength(50).IsRequired();
            e.Property(r => r.StudentIdentifier).HasMaxLength(50).IsRequired();
            e.Property(r => r.ReservationDate).HasColumnType("date");
            e.Property(r => r.StartTime).HasColumnType("datetime2(0)");
            e.Property(r => r.EndTime).HasColumnType("datetime2(0)");
            e.Property(r => r.Status).HasMaxLength(20).IsRequired();
            e.Property(r => r.CreatedAt).HasColumnType("datetime2(0)").HasDefaultValueSql("GETUTCDATE()");
            e.Property(r => r.CancelledAt).HasColumnType("datetime2(0)");

            e.HasOne(r => r.Seat)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(r => new { r.SeatId, r.ReservationDate, r.StartTime, r.EndTime })
                .HasDatabaseName("IX_Reservation_Seat_Time");
            e.HasIndex(r => new { r.StudentIdentifier, r.ReservationDate })
                .HasDatabaseName("IX_Reservation_Student");
            e.HasIndex(r => new { r.Status, r.ReservationDate })
                .HasDatabaseName("IX_Reservation_Status");
            e.HasIndex(r => r.ReservationDate)
                .HasDatabaseName("IX_Reservation_Date");
        });

        modelBuilder.Entity<Admin>(e =>
        {
            e.ToTable("Admins");
            e.HasKey(a => a.Id);
            e.Property(a => a.Username).HasMaxLength(50).IsRequired();
            e.Property(a => a.PasswordHash).HasMaxLength(200).IsRequired();
            e.HasIndex(a => a.Username).IsUnique().HasDatabaseName("IX_Admins_Username");
        });
    }
}
