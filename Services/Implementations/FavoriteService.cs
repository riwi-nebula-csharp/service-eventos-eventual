using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Models;
using service_eventos_eventual.Response;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Services.Implementations;

// Services/Implementations/FavoriteService.cs
public class FavoriteService : IFavoriteService
{
    private readonly TeatroEventsDbContext _context;

    public FavoriteService(TeatroEventsDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<IEnumerable<FavoriteResponseDto>>> GetAllByUserAsync(int userId)
    {
        var response = new ServiceResponse<IEnumerable<FavoriteResponseDto>>();
        try
        {
            var query = _context.Favorites
                .AsNoTracking()
                .Include(f => f.Play)
                .Where(f => f.UserId == userId && f.DeletedAt == null);

            response.Data = await query
                .Select(f => new FavoriteResponseDto
                {
                    Id = f.Id,
                    UserId = f.UserId,
                    PlayId = f.PlayId,
                    PlayName = f.Play.Name,
                    PosterUrl = f.Play.PosterUrl,
                    CreatedAt = f.CreatedAt
                }).ToListAsync();

            response.Success = true;
            response.Message = "Favorites retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving favorites: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<FavoriteResponseDto>> CreateAsync(int userId, int playId)
    {
        var response = new ServiceResponse<FavoriteResponseDto>();
        try
        {
            // Validate play exists
            var play = await _context.Plays
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == playId && p.DeletedAt == null);

            if (play == null)
            {
                response.Success = false;
                response.Message = $"Play with Id {playId} not found";
                return response;
            }

            // Check if favorite exists (including soft deleted)
            var existing = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PlayId == playId);

            if (existing != null)
            {
                if (existing.DeletedAt == null)
                {
                    // Already active
                    response.Success = false;
                    response.Message = "Play already in favorites";
                    return response;
                }

                // ✅ Restore soft deleted favorite instead of creating new one
                existing.DeletedAt = null;
                existing.CreatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                response.Data = new FavoriteResponseDto
                {
                    Id = existing.Id,
                    UserId = existing.UserId,
                    PlayId = existing.PlayId,
                    PlayName = play.Name,
                    PosterUrl = play.PosterUrl,
                    CreatedAt = existing.CreatedAt
                };
                response.Success = true;
                response.Message = "Favorite restored successfully";
                return response;
            }

            // Create new favorite
            var favorite = new Favorite
            {
                UserId = userId,
                PlayId = playId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            response.Data = new FavoriteResponseDto
            {
                Id = favorite.Id,
                UserId = favorite.UserId,
                PlayId = favorite.PlayId,
                PlayName = play.Name,
                PosterUrl = play.PosterUrl,
                CreatedAt = favorite.CreatedAt
            };
            response.Success = true;
            response.Message = "Favorite added successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error adding favorite: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(int userId, int id)
    {
        var response = new ServiceResponse<bool>();
        try
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId && f.DeletedAt == null);

            if (favorite == null)
            {
                response.Success = false;
                response.Data = false;
                response.Message = $"Favorite with Id {id} not found";
                return response;
            }

            favorite.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = true;
            response.Message = "Favorite removed successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error removing favorite: {ex.Message}";
        }
        return response;
    }
}