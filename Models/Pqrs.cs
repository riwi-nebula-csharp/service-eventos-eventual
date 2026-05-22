namespace service_eventos_eventual.Models;

public class Pqrs
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Description { get; set; } = null!;
    public PqrsType Type { get; set; }
    public PqrsStatus Status { get; set; }
    public string? Response { get; set; } = null!;
    public DateTime? ResponseAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
public enum PqrsStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}
public enum PqrsType
{
    Concerns,
    Petitions, 
    Complaints, 
    Grievances
}