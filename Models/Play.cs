using System.ComponentModel.DataAnnotations.Schema;

namespace service_eventos_eventual.Models;
[Table("plays")]
public class Play
{
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PosterUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        
        public ICollection<Performance> Performances { get; set; } = new List<Performance>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
}