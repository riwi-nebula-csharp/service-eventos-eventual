using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Models;
using service_eventos_eventual.Response;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Services.Implementations;

public class PlayService : IPlayService
{
    private readonly TeatroEventsDbContext _context;

    public PlayService(TeatroEventsDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<IEnumerable<PlayResponseDto>>> GetAllAsync()
    {
        var response = new ServiceResponse<IEnumerable<PlayResponseDto>>();
        try
        {
            var query = _context.Plays
                .AsNoTracking()
                .Where(p => p.DeletedAt == null);

            response.Data = await query
                .Select(p => new PlayResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    PosterUrl = p.PosterUrl,
                    CreatedAt = p.CreatedAt
                }).ToListAsync();

            response.Success = true;
            response.Message = "Plays listed correctly";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error while obtain the plays: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<IEnumerable<PlayResponseDto>>> GetAllDeletedAsync()
    {
        var response = new ServiceResponse<IEnumerable<PlayResponseDto>>();
        try
        {
            var query = _context.Plays
                .AsNoTracking()
                .Where(p => p.DeletedAt != null);

            response.Data = await query
                .Select(p => new PlayResponseDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    PosterUrl = p.PosterUrl,
                    CreatedAt = p.CreatedAt
                }).ToListAsync();

            response.Success = true;
            response.Message = "Plays eliminados obtenidos correctamente";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al obtener plays eliminados: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<PlayResponseDto?>> GetByIdAsync(int id)
    {
        var response = new ServiceResponse<PlayResponseDto?>();
        try
        {
            var play = await _context.Plays
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (play == null)
            {
                response.Success = false;
                response.Message = $"Play con Id {id} no encontrado";
                return response;
            }

            response.Data = new PlayResponseDto
            {
                Id = play.Id,
                Name = play.Name,
                Description = play.Description,
                PosterUrl = play.PosterUrl,
                CreatedAt = play.CreatedAt
            };
            response.Success = true;
            response.Message = "Play obtenido correctamente";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al obtener play: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<PlayResponseDto>> CreateAsync(PlayRequestDto dto)
    {
        var response = new ServiceResponse<PlayResponseDto>();
        try
        {
            var play = new Play
            {
                Name = dto.Name,
                Description = dto.Description,
                PosterUrl = dto.PosterUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Plays.Add(play);
            await _context.SaveChangesAsync();

            response.Data = new PlayResponseDto
            {
                Id = play.Id,
                Name = play.Name,
                Description = play.Description,
                PosterUrl = play.PosterUrl,
                CreatedAt = play.CreatedAt
            };
            response.Success = true;
            response.Message = "Play creado correctamente";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al crear play: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<PlayResponseDto>> UpdateAsync(int id, PlayRequestDto dto)
    {
        var response = new ServiceResponse<PlayResponseDto>();
        try
        {
            var play = await _context.Plays
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (play == null)
            {
                response.Success = false;
                response.Message = $"Play con Id {id} no encontrado";
                return response;
            }

            play.Name = dto.Name;
            play.Description = dto.Description;
            play.PosterUrl = dto.PosterUrl;
            play.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            response.Data = new PlayResponseDto
            {
                Id = play.Id,
                Name = play.Name,
                Description = play.Description,
                PosterUrl = play.PosterUrl,
                CreatedAt = play.CreatedAt
            };
            response.Success = true;
            response.Message = "Play actualizado correctamente";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al actualizar play: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(int id)
    {
        var response = new ServiceResponse<bool>();
        try
        {
            var play = await _context.Plays
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (play == null)
            {
                response.Success = false;
                response.Data = false;
                response.Message = $"Play con Id {id} no encontrado";
                return response;
            }

            play.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = true;
            response.Message = "Play eliminado correctamente";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al eliminar play: {ex.Message}";
        }
        return response;
    }
}