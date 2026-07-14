using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySeatReservation.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservation_Seat_Time",
                table: "Reservations");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_Seat_Time",
                table: "Reservations",
                columns: new[] { "SeatId", "ReservationDate", "StartTime", "EndTime" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservation_Seat_Time",
                table: "Reservations");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_Seat_Time",
                table: "Reservations",
                columns: new[] { "SeatId", "ReservationDate", "StartTime", "EndTime" });
        }
    }
}
