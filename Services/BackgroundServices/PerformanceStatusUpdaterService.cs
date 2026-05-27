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
        _logger = logger;
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
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TeatroEventsDbContext>();

        var now = DateTime.UtcNow;

        var performances = await context.Performances
            .Where(p =>
                p.DeletedAt == null &&
                p.StatusString == "scheduled" &&
                p.SalesStartDate <= now)
            .ToListAsync();

        if (!performances.Any()) return;

        foreach (var performance in performances)
        {
            performance.StatusString = "on_sale";
            performance.UpdatedAt = now;
        }

        await context.SaveChangesAsync();
        _logger.LogInformation("{Count} performance(s) updated to on_sale", performances.Count);
    }
}