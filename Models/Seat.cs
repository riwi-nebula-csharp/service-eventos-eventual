using System.ComponentModel.DataAnnotations.Schema;

namespace service_eventos_eventual.Models;
[Table("seats")]
public class Seat
{
    [Column("id")]
    public int Id { get; set; }
    [Column("row_name")]
    public char RowName { get; set; }
    [Column("row_order")]
    public int RowNumber { get; set; }
    [Column("number")]
    public int SeatNumber { get; set; }
    [Column("seat_order")]
    public int SeatOrder { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<PerformanceSeat> PerformanceSeats { get; set; } = new List<PerformanceSeat>();
}