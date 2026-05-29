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
    public string TypeString { get; set; } = "Petitions";

    [NotMapped]
    public PqrsType Type
    {
        get => TypeString switch
        {
            "Concerns"   => PqrsType.Concerns,
            "Petitions"  => PqrsType.Petitions,
            "Complaints" => PqrsType.Complaints,
            "Grievances" => PqrsType.Grievances,
            _            => PqrsType.Petitions
        };
        set => TypeString = value switch
        {
            PqrsType.Concerns   => "Concerns",
            PqrsType.Petitions  => "Petitions",
            PqrsType.Complaints => "Complaints",
            PqrsType.Grievances => "Grievances",
            _                   => "Petitions"
        };
    }

    [Column("status")]
    public string StatusString { get; set; } = "Pending";

    [NotMapped]
    public PqrsStatus Status
    {
        get => StatusString switch
        {
            "Pending"     => PqrsStatus.Pending,
            "In_progress" => PqrsStatus.InProgress,
            "Completed"   => PqrsStatus.Completed,
            "Cancelled"   => PqrsStatus.Cancelled,
            _             => PqrsStatus.Pending
        };
        set => StatusString = value switch
        {
            PqrsStatus.Pending    => "Pending",
            PqrsStatus.InProgress => "In_progress",
            PqrsStatus.Completed  => "Completed",
            PqrsStatus.Cancelled  => "Cancelled",
            _                     => "Pending"
        };
    }

    [Column("response")]
    public string? Response { get; set; }

    [Column("responded_by")]
    public int? RespondedBy { get; set; }

    [Column("responded_at")]
    public DateTime? RespondedAt { get; set; }

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