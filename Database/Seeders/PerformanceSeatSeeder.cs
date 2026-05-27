using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.Database.Factories;

namespace service_eventos_eventual.Database.Seeders;

public class PerformanceSeatSeeder
{
    public static async Task SeedAsync(TeatroEventsDbContext context, int performanceId)
    {
        // Check if performance_seats already exist for this performance
        var alreadySeeded = await context.PerformanceSeats
            .AnyAsync(ps => ps.PerformanceId == performanceId);

        if (alreadySeeded) return;

        // Get all seat ids
        var seatIds = await context.Seats
            .AsNoTracking()
            .Select(s => s.Id)
            .ToListAsync();

        if (!seatIds.Any())
        {
            Console.WriteLine("No seats found, performance seats not created");
            return;
        }

        var performanceSeats = PerformanceSeatFactory.CreateForPerformance(performanceId, seatIds);

        await context.PerformanceSeats.AddRangeAsync(performanceSeats);
        await context.SaveChangesAsync();

        Console.WriteLine($"{performanceSeats.Count} performance seats created for performance {performanceId}");
    }
}