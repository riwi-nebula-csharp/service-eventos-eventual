using System.ComponentModel.DataAnnotations.Schema;

namespace service_eventos_eventual.Models;
[Table("tickets")]
public class Ticket
{
    [Column("id")]
    public int Id { get; set; }

    [Column("qr_uuid")]
    public Guid QrUuid { get; set; }

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

    // Campo que faltaba: email del dueño del ticket
    [Column("owner_email")]
    public string OwnerEmail { get; set; } = null!;
    
    [Column("price_at_purchase")]
    public decimal PriceAtPurchase { get; set; }

    [Column("status")]    
    public string StatusString { get; set; } = "Pending";

    [NotMapped]
    public TicketStatus Status
    {
        get => StatusString switch
        {
            "Pending"   => TicketStatus.Pending,
            "Active"    => TicketStatus.Active,
            "Cancelled" => TicketStatus.Cancelled,
            "Used"      => TicketStatus.Used,
            _           => TicketStatus.Pending
        };
        set => StatusString = value switch
        {
            TicketStatus.Pending    => "Pending",
            TicketStatus.Active     => "Active",
            TicketStatus.Cancelled  => "Cancelled",
            TicketStatus.Used       => "Used",
            _                       => "Pending"
        };
    }

    [Column("sold_by")]
    public int? SoldBy { get; set; }

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