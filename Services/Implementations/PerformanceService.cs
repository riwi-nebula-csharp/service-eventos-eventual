using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.Database.Seeders;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Models;
using service_eventos_eventual.Response;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Services.Implementations;

public class PerformanceService : IPerformanceService
{
    private readonly TeatroEventsDbContext _context;

    public PerformanceService(TeatroEventsDbContext context)
    {
        _context = context;
    }

    // ─── Private Validations ────────────────────────────────────────────────

    private async Task<ServiceResponse<PerformanceResponseDto>?> ValidatePerformanceDto(
        PerformanceRequestDto dto, int? excludeId = null)
    {
        // Validate play exists and is not deleted
        var play = await _context.Plays
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == dto.PlayId && p.DeletedAt == null);

        if (play == null)
            return new ServiceResponse<PerformanceResponseDto>
            {
                Success = false,
                Message = $"Play with Id {dto.PlayId} not found or has been deleted"
            };

        // Validate performance date is in the future
        if (dto.PerformanceDate <= DateOnly.FromDateTime(DateTime.UtcNow))
            return new ServiceResponse<PerformanceResponseDto>
            {
                Success = false,
                Message = "Performance date must be after today"
            };

        // Validate start time is before end time
        if (dto.StartTime >= dto.EndTime)
            return new ServiceResponse<PerformanceResponseDto>
            {
                Success = false,
                Message = "Start time must be before end time"
            };

        // Validate ticket price is greater than 0
        if (dto.TicketPrice <= 0)
            return new ServiceResponse<PerformanceResponseDto>
            {
                Success = false,
                Message = "Ticket price must be greater than 0"
            };

        // Validate sales start is before sales end
        if (dto.SalesStartDate >= dto.SalesEndDate)
            return new ServiceResponse<PerformanceResponseDto>
            {
                Success = false,
                Message = "Sales start date must be before sales end date"
            };

        // Validate no schedule conflicts (single venue)
        var hasConflict = await _context.Performances
            .AsNoTracking()
            .Where(p => p.DeletedAt == null
                && p.PerformanceDate == dto.PerformanceDate
                && (excludeId == null || p.Id != excludeId)
                && p.StartTime < dto.EndTime
                && p.EndTime > dto.StartTime)
            .AnyAsync();

        if (hasConflict)
            return new ServiceResponse<PerformanceResponseDto>
            {
                Success = false,
                Message = $"There is already a performance scheduled between {dto.StartTime} and {dto.EndTime} on {dto.PerformanceDate}"
            };

        return null; // No errors
    }

    // ─── Mapping ────────────────────────────────────────────────────────────

    private static PerformanceResponseDto MapToDto(Performance p, string playName) =>
        new PerformanceResponseDto
        {
            Id = p.Id,
            PlayId = p.PlayId,
            PlayName = playName,
            PerformanceDate = p.PerformanceDate,
            StartTime = p.StartTime,
            EndTime = p.EndTime,
            TicketPrice = p.TicketPrice,
            SalesStartDate = p.SalesStartDate,
            SalesEndDate = p.SalesEndDate,
            Status = p.StatusString,
            CreatedAt = p.CreatedAt
        };

    // ─── Methods ────────────────────────────────────────────────────────────

    public async Task<ServiceResponse<IEnumerable<PerformanceResponseDto>>> GetAllAsync()
    {
        var response = new ServiceResponse<IEnumerable<PerformanceResponseDto>>();
        try
        {
            var query = _context.Performances
                .AsNoTracking()
                .Include(p => p.Play)
                .Where(p => p.DeletedAt == null);

            response.Data = await query
                .Select(p => new PerformanceResponseDto
                {
                    Id = p.Id,
                    PlayId = p.PlayId,
                    PlayName = p.Play.Name,
                    PerformanceDate = p.PerformanceDate,
                    StartTime = p.StartTime,
                    EndTime = p.EndTime,
                    TicketPrice = p.TicketPrice,
                    SalesStartDate = p.SalesStartDate,
                    SalesEndDate = p.SalesEndDate,
                    Status = p.StatusString,
                    CreatedAt = p.CreatedAt
                }).ToListAsync();

            response.Success = true;
            response.Message = "Performances retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving performances: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<IEnumerable<PerformanceResponseDto>>> GetAllDeletedAsync()
    {
        var response = new ServiceResponse<IEnumerable<PerformanceResponseDto>>();
        try
        {
            var query = _context.Performances
                .AsNoTracking()
                .Include(p => p.Play)
                .Where(p => p.DeletedAt != null);

            response.Data = await query
                .Select(p => new PerformanceResponseDto
                {
                    Id = p.Id,
                    PlayId = p.PlayId,
                    PlayName = p.Play.Name,
                    PerformanceDate = p.PerformanceDate,
                    StartTime = p.StartTime,
                    EndTime = p.EndTime,
                    TicketPrice = p.TicketPrice,
                    SalesStartDate = p.SalesStartDate,
                    SalesEndDate = p.SalesEndDate,
                    Status = p.StatusString,
                    CreatedAt = p.CreatedAt
                }).ToListAsync();

            response.Success = true;
            response.Message = "Deleted performances retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving deleted performances: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<PerformanceResponseDto?>> GetByIdAsync(int id)
    {
        var response = new ServiceResponse<PerformanceResponseDto?>();
        try
        {
            var performance = await _context.Performances
                .AsNoTracking()
                .Include(p => p.Play)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (performance == null)
            {
                response.Success = false;
                response.Message = $"Performance with Id {id} not found";
                return response;
            }

            response.Data = MapToDto(performance, performance.Play.Name);
            response.Success = true;
            response.Message = "Performance retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving performance: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<PerformanceResponseDto>> CreateAsync(PerformanceRequestDto dto)
    {
        var response = new ServiceResponse<PerformanceResponseDto>();
        try
        {
            // Run validations
            var validationError = await ValidatePerformanceDto(dto);
            if (validationError != null) return validationError;

            var performance = new Performance
            {
                PlayId = dto.PlayId,
                PerformanceDate = dto.PerformanceDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                TicketPrice = dto.TicketPrice,
                SalesStartDate = dto.SalesStartDate,
                SalesEndDate = dto.SalesEndDate,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Performances.Add(performance);
            await _context.SaveChangesAsync();

            // Auto-generate performance seats
            await PerformanceSeatSeeder.SeedAsync(_context, performance.Id);

            // Load play name for response
            var playName = await _context.Plays
                .AsNoTracking()
                .Where(p => p.Id == dto.PlayId)
                .Select(p => p.Name)
                .FirstAsync();

            response.Data = MapToDto(performance, playName);
            response.Success = true;
            response.Message = "Performance created successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error creating performance: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<PerformanceResponseDto>> UpdateAsync(int id, PerformanceRequestDto dto)
    {
        var response = new ServiceResponse<PerformanceResponseDto>();
        try
        {
            var performance = await _context.Performances
                .Include(p => p.Play)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (performance == null)
            {
                response.Success = false;
                response.Message = $"Performance with Id {id} not found";
                return response;
            }

            // Pass excludeId to avoid conflict with itself
            var validationError = await ValidatePerformanceDto(dto, excludeId: id);
            if (validationError != null) return validationError;

            performance.PlayId = dto.PlayId;
            performance.PerformanceDate = dto.PerformanceDate;
            performance.StartTime = dto.StartTime;
            performance.EndTime = dto.EndTime;
            performance.TicketPrice = dto.TicketPrice;
            performance.SalesStartDate = dto.SalesStartDate;
            performance.SalesEndDate = dto.SalesEndDate;
            performance.Status = dto.Status;
            performance.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            response.Data = MapToDto(performance, performance.Play.Name);
            response.Success = true;
            response.Message = "Performance updated successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error updating performance: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(int id)
    {
        var response = new ServiceResponse<bool>();
        try
        {
            var performance = await _context.Performances
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (performance == null)
            {
                response.Success = false;
                response.Data = false;
                response.Message = $"Performance with Id {id} not found";
                return response;
            }

            performance.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = true;
            response.Message = "Performance deleted successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error deleting performance: {ex.Message}";
        }
        return response;
    }
}