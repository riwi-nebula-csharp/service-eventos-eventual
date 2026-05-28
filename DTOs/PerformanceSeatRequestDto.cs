using System.ComponentModel.DataAnnotations;

namespace service_eventos_eventual.DTOs;

// DTOs/PerformanceSeat/PerformanceSeatResponseDto.cs
public class PerformanceSeatResponseDto
{
    public int Id { get; set; }
    public int PerformanceId { get; set; }
    public int SeatId { get; set; }
    public char RowName { get; set; }
    public int RowNumber { get; set; }
    public int SeatNumber { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? ReservedUntil { get; set; }
    public int? ScannedByUserId { get; set; }
    public DateTime? ScannedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// DTOs/PerformanceSeat/PerformanceSeatUpdateStatusDto.cs
public class PerformanceSeatUpdateStatusDto
{
    [Required]
    public string Status { get; set; } = null!; // "available" | "reserved" | "occupied"
}

// DTOs/PerformanceSeat/PerformanceSeatScanDto.cs
public class PerformanceSeatScanDto
{
    [Required]
    public int ScannedByUserId { get; set; }
}