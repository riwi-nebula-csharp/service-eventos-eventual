using System.ComponentModel.DataAnnotations.Schema;

namespace service_eventos_eventual.Models;
[Table("favorites")]
public class Favorite
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    
    public int PlayId { get; set; }
    public Play Play { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}