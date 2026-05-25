using service_eventos_eventual.Models;

namespace service_eventos_eventual.Database.Factories;

public class PerformanceSeatFactory
{
    private static PerformanceSeat Create(int performanceId, int seatId)
    {
        return new PerformanceSeat
        {
            PerformanceId = performanceId,
            SeatId = seatId,
            Status = PSStatus.Available,
            ReservedUntil = null,
            ScannedByUserId = null,
            ScannedAt = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static List<PerformanceSeat> CreateForPerformance(int performanceId, List<int> seatIds)
    {
        return seatIds
            .Select(seatId => Create(performanceId, seatId))
            .ToList();
    }
}