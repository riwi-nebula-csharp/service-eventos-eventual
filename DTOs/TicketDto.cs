namespace service_eventos_eventual.DTOs;

public class TicketResponseDto
{
    public int    Id               { get; set; }
    public string QrUuid           { get; set; } = null!;
    public string QrUrl            { get; set; } = null!;
    public int    PurchaseId       { get; set; }
    public int    PerformanceSeatId{ get; set; }
    public string RowName          { get; set; } = null!;
    public int    SeatNumber       { get; set; }
    public string PlayName         { get; set; } = null!;
    public DateOnly PerformanceDate{ get; set; }
    public TimeOnly StartTime      { get; set; }
    public int    OwnerId          { get; set; }
    public string OwnerEmail       { get; set; } = null!;
    public string Status           { get; set; } = null!;
    public int? SoldBy { get; set; }
    public DateTime CreatedAt      { get; set; }
}