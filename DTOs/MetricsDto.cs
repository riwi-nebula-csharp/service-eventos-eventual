namespace service_eventos_eventual.DTOs;

public class MetricsResponseDto
{
    public PeriodDto          Period    { get; set; } = null!;
    public TicketMetricsDto   Tickets   { get; set; } = null!;
    public PurchaseMetricsDto Purchases { get; set; } = null!;
    public List<OccupancyDto> Occupancy { get; set; } = new();
    public List<TopPlayDto>   TopPlays  { get; set; } = new();
}

public class PeriodDto
{
    public DateTime From { get; set; }
    public DateTime To   { get; set; }
}

public class TicketMetricsDto
{
    public int              TotalSold { get; set; }
    public List<WeeklyDto>  ByWeek    { get; set; } = new();
}

public class WeeklyDto
{
    public string Week  { get; set; } = null!; // "2026-05-01"
    public int    Count { get; set; }
}

public class PurchaseMetricsDto
{
    public int     Total           { get; set; }
    public decimal TotalRevenue    { get; set; }
    public int     OnlineCount     { get; set; }
    public int     BoxOfficeCount  { get; set; }
}

public class OccupancyDto
{
    public int      PerformanceId   { get; set; }
    public string   PlayName        { get; set; } = null!;
    public DateOnly PerformanceDate { get; set; }
    public TimeOnly StartTime       { get; set; }
    public int      TotalSeats      { get; set; }
    public int      OccupiedSeats   { get; set; }
    public double   OccupancyRate   { get; set; }
}

public class TopPlayDto
{
    public string PlayName    { get; set; } = null!;
    public int    TicketsSold { get; set; }
}