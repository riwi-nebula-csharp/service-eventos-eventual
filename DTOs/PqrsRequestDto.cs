using System.ComponentModel.DataAnnotations;

namespace service_eventos_eventual.DTOs;

public class PqrsResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Response { get; set; }
    public DateTime? ResponseAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PqrsRequestDto
{
    [Required]
    public string Subject { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public string Type { get; set; } = "Petition"; // "Petition" | "Concern" | "Complaint" | "Grievance"
}

public class PqrsRespondDto
{
    [Required]
    public string Response { get; set; } = null!;

    [Required]
    public string Status { get; set; } = null!; // "Pending" | "In_progress" | "Completed" | "Cancelled"
}