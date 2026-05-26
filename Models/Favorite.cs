using System.ComponentModel.DataAnnotations.Schema;

namespace service_eventos_eventual.Models;
[Table("favorites")]
public class Favorite
{
    [Column("id")]
    public int Id { get; set; }
    [Column("user_id")]
    public int UserId { get; set; }
    [Column("play_id")]
    public int PlayId { get; set; }
    public Play Play { get; set; } = null!;
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }
}