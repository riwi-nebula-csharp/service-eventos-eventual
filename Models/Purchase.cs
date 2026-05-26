using System.ComponentModel.DataAnnotations.Schema;

namespace service_eventos_eventual.Models;

[Table("purchases")]
public class Purchase
{
    [Column("id")]
    public int Id { get; set; }

    [Column("buyer_id")]
    public int BuyerId { get; set; }

    [Column("buyer_email")]
    public string BuyerEmail { get; set; } = null!;

    [Column("performance_id")]
    public int PerformanceId { get; set; }
    public Performance Performance { get; set; } = null!;

    [Column("ticket_count")]
    public int TicketCount { get; set; }

    [Column("total_price")]
    public double TotalPrice { get; set; }

    // PaymentMethod backing property
    [Column("payment_method")]
    public string PaymentMethodString { get; set; } = "online";

    [NotMapped]
    public PaymentMethod PaymentMethod
    {
        get => PaymentMethodString switch
        {
            "online"      => PaymentMethod.Online,
            "box_office"  => PaymentMethod.BoxOffice,
            _             => PaymentMethod.Online
        };
        set => PaymentMethodString = value switch
        {
            PaymentMethod.Online     => "online",
            PaymentMethod.BoxOffice  => "box_office",
            _                        => "online"
        };
    }

    // Status backing property
    [Column("status")]
    public string StatusString { get; set; } = "pending";

    [NotMapped]
    public PurchaseStatus Status
    {
        get => StatusString switch
        {
            "pending"   => PurchaseStatus.Pending,
            "completed" => PurchaseStatus.Completed,
            "failed"    => PurchaseStatus.Failed,
            "refunded"  => PurchaseStatus.Refunded,
            _           => PurchaseStatus.Pending
        };
        set => StatusString = value switch
        {
            PurchaseStatus.Pending   => "pending",
            PurchaseStatus.Completed => "completed",
            PurchaseStatus.Failed    => "failed",
            PurchaseStatus.Refunded  => "refunded",
            _                        => "pending"
        };
    }

    [Column("stripe_payment_id")]
    public string? StripePaymentId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

public enum PaymentMethod
{
    Online,
    BoxOffice
}

public enum PurchaseStatus
{
    Pending,
    Completed,
    Failed,
    Refunded
}