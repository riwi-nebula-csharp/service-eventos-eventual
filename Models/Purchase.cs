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
    [Column("total_amount")]
    public double TotalPrice { get; set; }
    [Column("payment_method")]
    public PaymentMethod PaymentMethod { get; set; }
    [Column("status")]
    public PurchaseStatus Status { get; set; }
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