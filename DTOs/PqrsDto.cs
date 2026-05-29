using System.ComponentModel.DataAnnotations;

namespace service_eventos_eventual.DTOs;

public class PqrsResponseDto
{
    public int      Id           { get; set; }
    public int      UserId       { get; set; }
    public string   UserEmail    { get; set; } = null!;
    public string   Subject      { get; set; } = null!;
    public string   Description  { get; set; } = null!;
    public string   Type         { get; set; } = null!;
    public string   Status       { get; set; } = null!;
    public string?  Response     { get; set; }
    public int?     RespondedBy  { get; set; }
    public DateTime? RespondedAt { get; set; }
    public DateTime CreatedAt    { get; set; }
    public DateTime UpdatedAt    { get; set; }
}

public class PqrsCreateDto
{
    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    /// <summary>Concern | Petition | Complaint | Grievance</summary>
    [Required]
    public string Type { get; set; } = "Petition";
}

public class PqrsRespondDto
{
    [Required]
    public string Response { get; set; } = null!;

    /// <summary>Pending | In_progress | Completed | Cancelled</summary>
    [Required]
    public string Status { get; set; } = "Completed";
}