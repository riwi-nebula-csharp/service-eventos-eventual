using System.ComponentModel.DataAnnotations;

namespace service_eventos_eventual.DTOs;

public class CreatePaymentIntentDto
{
    [Required]
    public int PerformanceId { get; set; }

    [Required]
    public int TicketCount { get; set; }
}

public class PaymentIntentResponseDto
{
    public string ClientSecret     { get; set; } = null!;
    public string PaymentIntentId  { get; set; } = null!;
    public decimal Amount          { get; set; }
    public string Currency         { get; set; } = null!;
}