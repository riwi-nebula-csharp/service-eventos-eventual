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
    
    // Backing property - maps to DB
    [Column("status")]
    public string StatusString { get; set; } = "available";

    // NotMapped - used in code
    [NotMapped]
    public PsStatus Status
    {
        get => StatusString switch
        {
            "available" => PsStatus.Available,
            "reserved"  => PsStatus.Reserved,
            "occupied"  => PsStatus.Occupied,
            _           => PsStatus.Available // Fallback
        };
        set => StatusString = value switch
        {
            PsStatus.Available => "available",
            PsStatus.Reserved  => "reserved",
            PsStatus.Occupied  => "occupied",
            _                  => "available" // Fallback
        };
    }
    
    
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

    public Ticket? Ticket { get; set; }
}

public enum PsStatus
{
    Available,
    Reserved,
    Occupied
}