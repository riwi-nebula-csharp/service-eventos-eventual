using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.Database.Factories;
using service_eventos_eventual.Models;

namespace service_eventos_eventual.Database.Seeders;

public class SeatSeeder
{
    public static async Task SeedAsync(TeatroEventsDbContext context)
    {
        if (await context.Seats.AnyAsync()) return;

        var seats = SeatFactory.CreateMany();

        await context.Seats.AddRangeAsync(seats);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ {seats.Count} seats seeded successfully");
    }
}