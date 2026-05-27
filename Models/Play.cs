using System.ComponentModel.DataAnnotations.Schema;

namespace service_eventos_eventual.Models;
[Table("plays")]
public class Play
{
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; } = null!;
        [Column("description")]
        public string Description { get; set; } = null!;
        [Column("poster_url")]
        public string PosterUrl { get; set; } = null!;
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }
        
        public ICollection<Performance> Performances { get; set; } = new List<Performance>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}