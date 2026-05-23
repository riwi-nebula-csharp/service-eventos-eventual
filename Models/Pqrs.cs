using System.ComponentModel.DataAnnotations.Schema;

namespace service_eventos_eventual.Models;
[Table("pqrs")]
public class Pqrs
{
    [Column("id")]
    public int Id { get; set; }
    [Column("user_id")]
    public int UserId { get; set; }
    [Column("user_email")]
    public string UserEmail { get; set; } = null!;
    [Column("subject")]
    public string Subject { get; set; } = null!;
    [Column("description")]
    public string Description { get; set; } = null!;
    [Column("type")]
    public PqrsType Type { get; set; }
    [Column("status")]
    public PqrsStatus Status { get; set; }
    [Column("response")]
    public string? Response { get; set; } = null!;
    [Column("response_at")]
    public DateTime? ResponseAt { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [Column("deleted_at")]
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