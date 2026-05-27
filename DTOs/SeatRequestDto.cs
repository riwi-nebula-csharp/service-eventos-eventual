namespace service_eventos_eventual.DTOs;

public class SeatRequestDto
{
    public char RowName { get; set; }
    public int RowNumber { get; set; }
    public int SeatNumber { get; set; }
    public int SeatOrder { get; set; }
}

public class SeatResponseDto
{
    public int Id { get; set; }
    public char RowName { get; set; }
    public int RowNumber { get; set; }
    public int SeatNumber { get; set; }
    public int SeatOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}