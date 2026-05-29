using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;

namespace service_eventos_eventual.Services.BackgroundServices;

public class PerformanceStatusUpdaterService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PerformanceStatusUpdaterService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

    public PerformanceStatusUpdaterService(
        IServiceScopeFactory scopeFactory,
        ILogger<PerformanceStatusUpdaterService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await UpdatePerformanceStatuses();
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task UpdatePerformanceStatuses()
    {
        using var scope   = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TeatroEventsDbContext>();

        var now = DateTime.UtcNow;
        int count = 0;

        // ── scheduled → on_sale ────────────────────────────────────────────
        // Cuando la fecha de inicio de ventas ya pasó
        var toOpenSale = await context.Performances
            .Where(p =>
                p.DeletedAt       == null        &&
                p.StatusString    == "scheduled" &&
                p.SalesStartDate  <= now)
            .ToListAsync();

        foreach (var p in toOpenSale)
        {
            p.StatusString = "on_sale";
            p.UpdatedAt    = now;
            count++;
        }

        // ── on_sale → finished ─────────────────────────────────────────────
        // Cuando la fecha/hora del evento ya terminó
        // Combinamos PerformanceDate (DateOnly) + EndTime (TimeOnly) en un DateTime UTC
        var toFinish = await context.Performances
            .Where(p =>
                p.DeletedAt    == null    &&
                p.StatusString == "on_sale")
            .ToListAsync();

        foreach (var p in toFinish)
        {
            // Reconstruimos el DateTime de fin de función
            var endDateTime = p.PerformanceDate.ToDateTime(p.EndTime);
            if (endDateTime < now)
            {
                p.StatusString = "finished";
                p.UpdatedAt    = now;
                count++;
            }
        }

        // sold_out → finished también (si la función ya pasó aunque esté agotada)
        var soldOutToFinish = await context.Performances
            .Where(p =>
                p.DeletedAt    == null      &&
                p.StatusString == "sold_out")
            .ToListAsync();

        foreach (var p in soldOutToFinish)
        {
            var endDateTime = p.PerformanceDate.ToDateTime(p.EndTime);
            if (endDateTime < now)
            {
                p.StatusString = "finished";
                p.UpdatedAt    = now;
                count++;
            }
        }

        if (count > 0)
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("{Count} performance(s) status updated", count);
        }
    }
}
