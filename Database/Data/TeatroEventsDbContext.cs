using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Models;

namespace service_eventos_eventual.Database.Data;

public class TeatroEventsDbContext : DbContext
{
    public TeatroEventsDbContext(DbContextOptions<TeatroEventsDbContext> options) : base(options)
    {
    }

    public DbSet<Play>            Plays            { get; set; }
    public DbSet<Performance>     Performances     { get; set; }
    public DbSet<Seat>            Seats            { get; set; }
    public DbSet<PerformanceSeat> PerformanceSeats { get; set; }
    public DbSet<Purchase>        Purchases        { get; set; }
    public DbSet<Favorite>        Favorites        { get; set; }
    public DbSet<Ticket>          Tickets          { get; set; }
    public DbSet<Pqrs>            Pqrs             { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ticket tiene FK a PerformanceSeat (1-a-1)
        // Sin esta configuración EF intenta crear una FK circular con cascade
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.PerformanceSeat)
            .WithOne(ps => ps.Ticket)
            .HasForeignKey<Ticket>(t => t.PerformanceSeatId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ticket → Purchase (muchos tickets por compra)
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Purchase)
            .WithMany(p => p.Tickets)
            .HasForeignKey(t => t.PurchaseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}