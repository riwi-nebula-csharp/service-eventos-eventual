namespace service_eventos_eventual.Models;

public class Purchase
{
    public int Id { get; set; }
    public int BuyerId { get; set; }
    public string BuyerEmail { get; set; } = null!;
    
    public int PerformanceId { get; set; }
    public Performance Performance { get; set; } = null!;
    
    public int TicketCount { get; set; }
    public double TotalPrice { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public PurchaseStatus Status { get; set; }
    public string? StripePaymentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
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