using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Models;
using service_eventos_eventual.Response;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Services.Implementations;

public class PqrsService : IPqrsService
{
    private readonly TeatroEventsDbContext _context;

    public PqrsService(TeatroEventsDbContext context)
    {
        _context = context;
    }

    // ─── Cliente: ver sus propias PQRS ────────────────────────────────────

    public async Task<ServiceResponse<IEnumerable<PqrsResponseDto>>> GetAllByUserAsync(int userId)
    {
        var response = new ServiceResponse<IEnumerable<PqrsResponseDto>>();
        try
        {
            response.Data = await _context.Pqrs
                .AsNoTracking()
                .Where(p => p.UserId == userId && p.DeletedAt == null)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => MapToDto(p))
                .ToListAsync();

            response.Success = true;
            response.Message = "PQRS retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving PQRS: {ex.Message}";
        }
        return response;
    }

    // ─── Cliente: crear PQRS ──────────────────────────────────────────────

    public async Task<ServiceResponse<PqrsResponseDto>> CreateAsync(
        int userId, string userEmail, PqrsCreateDto dto)
    {
        var response = new ServiceResponse<PqrsResponseDto>();
        try
        {
            var validTypes = new[] { "Concerns", "Petitions", "Complaints", "Grievances" };
            if (!validTypes.Contains(dto.Type))
            {
                response.Success = false;
                response.Message = $"Invalid type. Valid values: {string.Join(", ", validTypes)}";
                return response;
            }

            var pqrs = new Pqrs
            {
                UserId      = userId,
                UserEmail   = userEmail,
                Subject     = dto.Subject,
                Description = dto.Description,
                TypeString  = dto.Type,
                StatusString= "Pending",
                CreatedAt   = DateTime.UtcNow,
                UpdatedAt   = DateTime.UtcNow
            };

            _context.Pqrs.Add(pqrs);
            await _context.SaveChangesAsync();

            response.Data    = MapToDto(pqrs);
            response.Success = true;
            response.Message = "PQRS created successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error creating PQRS: {ex.Message}";
        }
        return response;
    }

    // ─── Admin: ver todas las PQRS ────────────────────────────────────────

    public async Task<ServiceResponse<IEnumerable<PqrsResponseDto>>> GetAllAsync()
    {
        var response = new ServiceResponse<IEnumerable<PqrsResponseDto>>();
        try
        {
            response.Data = await _context.Pqrs
                .AsNoTracking()
                .Where(p => p.DeletedAt == null)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => MapToDto(p))
                .ToListAsync();

            response.Success = true;
            response.Message = "All PQRS retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving PQRS: {ex.Message}";
        }
        return response;
    }

    // ─── Admin: responder una PQRS ────────────────────────────────────────

    public async Task<ServiceResponse<PqrsResponseDto>> RespondAsync(
        int id, int adminId, PqrsRespondDto dto)
    {
        var response = new ServiceResponse<PqrsResponseDto>();
        try
        {
            var validStatuses = new[] { "Pending", "In_progress", "Completed", "Cancelled" };
            if (!validStatuses.Contains(dto.Status))
            {
                response.Success = false;
                response.Message = $"Invalid status. Valid values: {string.Join(", ", validStatuses)}";
                return response;
            }

            var pqrs = await _context.Pqrs
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (pqrs == null)
            {
                response.Success = false;
                response.Message = $"PQRS with Id {id} not found";
                return response;
            }

            pqrs.Response     = dto.Response;
            pqrs.StatusString = dto.Status;
            pqrs.RespondedBy  = adminId;
            pqrs.RespondedAt  = DateTime.UtcNow;
            pqrs.UpdatedAt    = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            response.Data    = MapToDto(pqrs);
            response.Success = true;
            response.Message = "PQRS responded successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error responding to PQRS: {ex.Message}";
        }
        return response;
    }

    private static PqrsResponseDto MapToDto(Pqrs p) => new()
    {
        Id          = p.Id,
        UserId      = p.UserId,
        UserEmail   = p.UserEmail,
        Subject     = p.Subject,
        Description = p.Description,
        Type        = p.TypeString,
        Status      = p.StatusString,
        Response    = p.Response,
        RespondedBy = p.RespondedBy,
        RespondedAt = p.RespondedAt,
        CreatedAt   = p.CreatedAt,
        UpdatedAt   = p.UpdatedAt
    };
}
