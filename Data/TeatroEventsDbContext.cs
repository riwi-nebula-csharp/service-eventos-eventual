using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Models;

namespace service_eventos_eventual.Data;

public class TeatroEventsDbContext : DbContext
{
    public TeatroEventsDbContext(DbContextOptions<TeatroEventsDbContext> options) : base(options)
    {
    }
    public DbSet<Play> Plays { get; set; }
    public DbSet<Performance> Performances { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<PerformanceSeat> PerformanceSeats { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Pqrs> Pqrs { get; set; }
}