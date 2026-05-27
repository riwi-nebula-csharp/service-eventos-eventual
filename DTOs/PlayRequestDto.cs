namespace service_eventos_eventual.DTOs;

public class PlayRequestDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string PosterUrl { get; set; } = null!;
}

public class PlayResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string PosterUrl { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}