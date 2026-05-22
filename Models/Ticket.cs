namespace service_eventos_eventual.Models;

public class Ticket
{
    public int Id { get; set; }
    public string QrUuid { get; set; } = null!;
    public string QrCode { get; set; } = null!;

    public int PurchaseId { get; set; }
    public Purchase Purchase { get; set; } = null!;

    public int PerformanceSeatId { get; set; }
    public PerformanceSeat PerformanceSeat { get; set; } = null!;

    public int OwnerId { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public enum TicketStatus
{
    Pending = 0,
    Active,
    Cancelled,
    Used
}