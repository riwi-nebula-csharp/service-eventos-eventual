using System.ComponentModel.DataAnnotations;
using service_eventos_eventual.Models;

namespace service_eventos_eventual.DTOs;

public class PerformanceRequestDto
{
    public int PlayId { get; set; }
    
    [Required]
    public DateOnly PerformanceDate { get; set; }
    [Required]
    public TimeOnly StartTime { get; set; }
    [Required]
    public TimeOnly EndTime { get; set; }
    [Required]
    public double TicketPrice { get; set; }
    [Required]
    public DateTime SalesStartDate { get; set; }
    [Required]
    public DateTime SalesEndDate { get; set; }
    public PerformanceStatus Status { get; set; }
}

public class PerformanceResponseDto
{
    public int Id { get; set; }
    public int PlayId { get; set; }
    public string PlayName { get; set; } = null!;   // Extra Dto to show the play name
    public DateOnly PerformanceDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public double TicketPrice { get; set; }
    public DateTime SalesStartDate { get; set; }
    public DateTime SalesEndDate { get; set; }
    public PerformanceStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}