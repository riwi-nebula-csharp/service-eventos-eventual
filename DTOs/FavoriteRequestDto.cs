using System.ComponentModel.DataAnnotations;

namespace service_eventos_eventual.DTOs;

// DTOs/Favorite/FavoriteResponseDto.cs
public class FavoriteResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PlayId { get; set; }
    public string PlayName { get; set; } = null!;
    public string PosterUrl { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

// DTOs/Favorite/FavoriteRequestDto.cs
public class FavoriteRequestDto
{
    [Required]
    public int PlayId { get; set; }
}