namespace service_eventos_eventual.DTOs;

public class SeatMapDto
{
    public int    PerformanceId   { get; set; }
    public string PlayName        { get; set; } = null!;
    public DateOnly PerformanceDate { get; set; }
    public TimeOnly StartTime     { get; set; }
    public decimal TicketPrice    { get; set; }
    public int    TotalSeats      { get; set; }
    public int    AvailableSeats  { get; set; }
    public List<SeatRowDto> Rows  { get; set; } = new();
}

public class SeatRowDto
{
    public string RowName        { get; set; } = null!;
    public List<SeatSlotDto> Seats { get; set; } = new();
}

public class SeatSlotDto
{
    public int    PerformanceSeatId { get; set; }  // este es el ID que el cliente envía al comprar
    public int    SeatNumber        { get; set; }
    public int    RowOrder          { get; set; }
    public int    SeatOrder         { get; set; }
    public string Status            { get; set; } = null!;  // "available" | "occupied" | "reserved"
}