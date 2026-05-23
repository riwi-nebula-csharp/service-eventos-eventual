using System.ComponentModel.DataAnnotations.Schema;

namespace service_eventos_eventual.Models;
[Table("performance_seats")]
public class PerformanceSeat
{
    public int Id { get; set; }
    public int SeatId { get; set; }
    public Seat Seat { get; set; } = null!;
    
    public int PerformanceId { get; set; }
    public Performance Performance { get; set; } = null!;
    public PSStatus Status { get; set; }
    
    public DateTime? ReservedUntil { get; set; }
    public int? ScannedByUserId { get; set; }
    public DateTime? ScannedAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public Ticket? Ticket { get; set; }
}

public enum PSStatus
{
    Available = 0,
    Reserved,
    Occupied,
}