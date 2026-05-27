using System.ComponentModel.DataAnnotations;

namespace service_eventos_eventual.DTOs;

public class PurchaseResponseDto
{
    public int Id { get; set; }
    public int BuyerId { get; set; }
    public string BuyerEmail { get; set; } = null!;
    public int PerformanceId { get; set; }
    public DateOnly PerformanceDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public string PlayName { get; set; } = null!;
    public int TicketCount { get; set; }
    public double TotalPrice { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? StripePaymentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PurchaseRequestDto
{
    [Required]
    public int PerformanceId { get; set; }

    [Required]
    public int TicketCount { get; set; }

    [Required]
    [EmailAddress]
    public string BuyerEmail { get; set; } = null!;

    [Required]
    public string PaymentMethod { get; set; } = "online";

    public string? StripePaymentId { get; set; }
}

public class PurchaseUpdateStatusDto
{
    [Required]
    public string Status { get; set; } = null!; // "pending" | "completed" | "failed" | "refunded"
}