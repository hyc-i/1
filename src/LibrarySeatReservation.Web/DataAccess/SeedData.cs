using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.DataAccess;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (await context.Seats.AnyAsync()) return;

        var seats = new List<Seat>();

        for (int i = 1; i <= 10; i++)
            seats.Add(new Seat { SeatNumber = $"C-{i:D2}", Area = "一楼大厅", Floor = "1F", Description = "一楼大厅座位", IsActive = true });

        for (int i = 1; i <= 15; i++)
            seats.Add(new Seat { SeatNumber = $"B-{i:D2}", Area = "二楼阅览区", Floor = "2F", Description = "二楼阅览区座位", IsActive = true });

        for (int i = 1; i <= 15; i++)
            seats.Add(new Seat { SeatNumber = $"A-{i:D2}", Area = "三楼自习区", Floor = "3F", Description = "三楼自习区座位", IsActive = true });

        context.Seats.AddRange(seats);

        var admin = new Admin
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456")
        };
        context.Admins.Add(admin);

        await context.SaveChangesAsync();
    }
}
