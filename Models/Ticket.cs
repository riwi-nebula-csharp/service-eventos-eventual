using System.ComponentModel.DataAnnotations.Schema;

namespace service_eventos_eventual.Models;
[Table("tickets")]
public class Ticket
{
    [Column("id")]
    public int Id { get; set; }
    [Column("qr_uuid")]
    public string QrUuid { get; set; } = null!;
    [Column("qr_url")]
    public string QrCode { get; set; } = null!;
    [Column("purchase_id")]
    public int PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;
    [Column("performance_seat_id")]
    public int PerformanceSeatId { get; set; }
    public PerformanceSeat PerformanceSeat { get; set; } = null!;
    [Column("owner_id")]
    public int OwnerId { get; set; }
    [Column("status")]
    public TicketStatus Status { get; set; }
    [Column("sold_by")]
    public string SoldBy { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }
}

public enum TicketStatus
{
    Pending = 0,
    Active,
    Cancelled,
    Used
}