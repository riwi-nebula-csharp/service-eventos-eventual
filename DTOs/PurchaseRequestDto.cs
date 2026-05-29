using System.ComponentModel.DataAnnotations;

namespace service_eventos_eventual.DTOs;

public class PurchaseRequestDto
{
    [Required]
    public int PerformanceId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Debe seleccionar al menos un asiento")]
    [MaxLength(10, ErrorMessage = "No puede comprar más de 10 entradas por transacción")]
    public List<int> SeatNumbers { get; set; } = new();

    [Required]
    public string PaymentMethod { get; set; } = null!;

    public string? StripePaymentId { get; set; }
}

public class PurchaseUpdateStatusDto
{
    [Required]
    public string Status { get; set; } = null!;
}

public class PurchaseResponseDto
{
    public int      Id              { get; set; }
    public int      BuyerId         { get; set; }
    public string   BuyerEmail      { get; set; } = null!;
    public int      PerformanceId   { get; set; }
    public DateOnly PerformanceDate { get; set; }
    public TimeOnly StartTime       { get; set; }
    public string   PlayName        { get; set; } = null!;
    public int      TicketCount     { get; set; }
    public decimal  TotalPrice      { get; set; }
    public string   PaymentMethod   { get; set; } = null!;
    public string   Status          { get; set; } = null!;
    public string?  StripePaymentId { get; set; }
    public DateTime CreatedAt       { get; set; }
    public DateTime UpdatedAt       { get; set; }
}