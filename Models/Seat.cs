using System.ComponentModel.DataAnnotations.Schema;

namespace service_eventos_eventual.Models;
[Table("seats")]
public class Seat
{
    public int Id { get; set; }
    public char RowName { get; set; }
    public int RowNumber { get; set; }
    public int SeatNumber { get; set; }
    public int SeatOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<PerformanceSeat> PerformanceSeats { get; set; } = new List<PerformanceSeat>();
}