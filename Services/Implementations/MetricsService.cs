using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Services.Implementations;

public class MetricsService : IMetricsService
{
    private readonly TeatroEventsDbContext _context;

    public MetricsService(TeatroEventsDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<MetricsResponseDto>> GetMetricsAsync(DateTime from, DateTime to)
    {
        var response = new ServiceResponse<MetricsResponseDto>();
        try
        {
            // Normalizamos el rango: from desde las 00:00:00, to hasta las 23:59:59
            var fromUtc = DateTime.SpecifyKind(from.Date, DateTimeKind.Utc);
            var toUtc   = DateTime.SpecifyKind(to.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);

            // ─── 1. Total de tickets vendidos en el período ───────────────
            var totalSold = await _context.Tickets
                .Where(t => t.DeletedAt == null &&
                            t.CreatedAt >= fromUtc &&
                            t.CreatedAt <= toUtc)
                .CountAsync();

            // ─── 2. Tickets agrupados por semana ──────────────────────────
            // Traemos los tickets al cliente y agrupamos en memoria
            // porque MySQL no tiene una función directa de número de semana
            // compatible con todos los drivers de EF Core
            var ticketsInPeriod = await _context.Tickets
                .Where(t => t.DeletedAt == null &&
                            t.CreatedAt >= fromUtc &&
                            t.CreatedAt <= toUtc)
                .Select(t => t.CreatedAt)
                .ToListAsync();

            var byWeek = ticketsInPeriod
                .GroupBy(date =>
                {
                    // Obtener el lunes de la semana de cada ticket
                    var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
                    return date.AddDays(-diff).Date;
                })
                .Select(g => new WeeklyDto
                {
                    Week  = g.Key.ToString("yyyy-MM-dd"),
                    Count = g.Count()
                })
                .OrderBy(w => w.Week)
                .ToList();

            // ─── 3. Métricas de compras ───────────────────────────────────
            var purchases = await _context.Purchases
                .Where(p => p.DeletedAt  == null &&
                            p.StatusString == "completed" &&
                            p.CreatedAt  >= fromUtc &&
                            p.CreatedAt  <= toUtc)
                .ToListAsync();

            var purchaseMetrics = new PurchaseMetricsDto
            {
                Total          = purchases.Count,
                TotalRevenue   = purchases.Sum(p => (decimal)p.TotalPrice),
                OnlineCount    = purchases.Count(p => p.PaymentMethodString == "online"),
                BoxOfficeCount = purchases.Count(p => p.PaymentMethodString == "box_office")
            };

            // ─── 4. Ocupación por función ─────────────────────────────────
            var occupancy = await _context.PerformanceSeats
                .AsNoTracking()
                .Include(ps => ps.Performance).ThenInclude(p => p.Play)
                .Where(ps => ps.Performance.DeletedAt == null)
                .GroupBy(ps => new
                {
                    ps.PerformanceId,
                    PlayName        = ps.Performance.Play.Name,
                    PerformanceDate = ps.Performance.PerformanceDate,
                    StartTime       = ps.Performance.StartTime
                })
                .Select(g => new OccupancyDto
                {
                    PerformanceId   = g.Key.PerformanceId,
                    PlayName        = g.Key.PlayName,
                    PerformanceDate = g.Key.PerformanceDate,
                    StartTime       = g.Key.StartTime,
                    TotalSeats      = g.Count(),
                    OccupiedSeats   = g.Count(ps => ps.StatusString == "occupied"),
                    OccupancyRate   = g.Count() == 0
                        ? 0
                        : Math.Round((double)g.Count(ps => ps.StatusString == "occupied") / g.Count() * 100, 2)
                })
                .OrderByDescending(o => o.OccupancyRate)
                .ToListAsync();

            // ─── 5. Top obras por tickets vendidos ────────────────────────
            var topPlays = await _context.Tickets
                .AsNoTracking()
                .Include(t => t.PerformanceSeat)
                    .ThenInclude(ps => ps.Performance)
                        .ThenInclude(p => p.Play)
                .Where(t => t.DeletedAt == null &&
                            t.CreatedAt >= fromUtc &&
                            t.CreatedAt <= toUtc)
                .GroupBy(t => t.PerformanceSeat.Performance.Play.Name)
                .Select(g => new TopPlayDto
                {
                    PlayName    = g.Key,
                    TicketsSold = g.Count()
                })
                .OrderByDescending(t => t.TicketsSold)
                .Take(5)
                .ToListAsync();

            // ─── Armar respuesta ──────────────────────────────────────────
            response.Data = new MetricsResponseDto
            {
                Period = new PeriodDto
                {
                    From = fromUtc,
                    To   = toUtc
                },
                Tickets = new TicketMetricsDto
                {
                    TotalSold = totalSold,
                    ByWeek    = byWeek
                },
                Purchases = purchaseMetrics,
                Occupancy = occupancy,
                TopPlays  = topPlays
            };

            response.Success = true;
            response.Message = "Metrics retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving metrics: {ex.Message}";
        }
        return response;
    }
}