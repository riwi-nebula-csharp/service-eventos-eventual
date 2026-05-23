using System.ComponentModel.DataAnnotations.Schema;

namespace service_eventos_eventual.Models;
[Table("performances")]
public class Performance
{
    public int Id { get; set; }
    
    public int PlayId { get; set; }
    public Play Play { get; set; } = null!;
    
    public DateOnly PerformanceDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public double TicketPrice { get; set; }
    public DateTime SalesStartDate { get; set; }
    public DateTime SalesEndDate { get; set; }
    public PerformanceStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public ICollection<PerformanceSeat> PerformanceSeats { get; set; } = new List<PerformanceSeat>();
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
}
public enum PerformanceStatus
{
    Scheduled,
    OnSale,
    Canceled
}