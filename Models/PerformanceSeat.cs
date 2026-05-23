using System.ComponentModel.DataAnnotations.Schema;

namespace service_eventos_eventual.Models;
[Table("performance_seats")]
public class PerformanceSeat
{
    [Column("id")]
    public int Id { get; set; }
    [Column("seat_id")]
    public int SeatId { get; set; }
    public Seat Seat { get; set; } = null!;
    [Column("performance_id")]
    public int PerformanceId { get; set; }
    public Performance Performance { get; set; } = null!;
    [Column("status")]
    public PSStatus Status { get; set; }
    [Column("reserved_until")]
    public DateTime? ReservedUntil { get; set; }
    [Column("scanned_by")]
    public int? ScannedByUserId { get; set; }
    [Column("scanned_at")]
    public DateTime? ScannedAt { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }
    
    public Ticket? Ticket { get; set; }
}

public enum PSStatus
{
    Available = 0,
    Reserved,
    Occupied,
}