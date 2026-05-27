using System.ComponentModel.DataAnnotations.Schema;
using service_eventos_eventual.Models;

namespace service_eventos_eventual.Models;

[Table("performances")]
public class Performance
{
    [Column("id")]
    public int Id { get; set; }

    [Column("play_id")]
    public int PlayId { get; set; }
    public Play Play { get; set; } = null!;

    [Column("performance_date")]
    public DateOnly PerformanceDate { get; set; }

    [Column("start_time")]
    public TimeOnly StartTime { get; set; }

    [Column("end_time")]
    public TimeOnly EndTime { get; set; }

    [Column("ticket_price")]
    public double TicketPrice { get; set; }

    [Column("sale_start_date")]
    public DateTime SalesStartDate { get; set; }

    [Column("sale_end_date")]
    public DateTime SalesEndDate { get; set; }

    // Backing property - maps to DB
    [Column("status")]
    public string StatusString { get; set; } = "scheduled";

    // NotMapped - used in code
    [NotMapped]
    public PerformanceStatus Status
    {
        get => StatusString switch
        {
            "scheduled"  => PerformanceStatus.Scheduled,
            "on_sale"    => PerformanceStatus.OnSale,
            "sold_out"   => PerformanceStatus.SoldOut,
            "finished"   => PerformanceStatus.Finished,
            "cancelled"  => PerformanceStatus.Cancelled,
            _            => PerformanceStatus.Scheduled // Fallback
        };
        set => StatusString = value switch
        {
            PerformanceStatus.Scheduled => "scheduled",
            PerformanceStatus.OnSale    => "on_sale",
            PerformanceStatus.SoldOut   => "sold_out",
            PerformanceStatus.Finished  => "finished",
            PerformanceStatus.Cancelled => "cancelled",
            _                           => "scheduled" // Fallback
        };
    }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    public ICollection<PerformanceSeat> PerformanceSeats { get; set; } = new List<PerformanceSeat>();
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}

public enum PerformanceStatus
{
    Scheduled,
    OnSale,
    SoldOut,
    Finished,
    Cancelled
}