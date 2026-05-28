using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Models;
using service_eventos_eventual.Response;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Services.Implementations;

// Services/Implementations/PerformanceSeatService.cs
public class PerformanceSeatService : IPerformanceSeatService
{
    private readonly TeatroEventsDbContext _context;

    public PerformanceSeatService(TeatroEventsDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<IEnumerable<PerformanceSeatResponseDto>>> GetAllAsync()
    {
        var response = new ServiceResponse<IEnumerable<PerformanceSeatResponseDto>>();
        try
        {
            response.Data = await _context.PerformanceSeats
                .AsNoTracking()
                .Include(ps => ps.Seat)
                .Select(ps => MapToDto(ps))
                .ToListAsync();

            response.Success = true;
            response.Message = "Performance seats retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving performance seats: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<IEnumerable<PerformanceSeatResponseDto>>> GetByPerformanceAsync(int performanceId)
    {
        var response = new ServiceResponse<IEnumerable<PerformanceSeatResponseDto>>();
        try
        {
            var performanceExists = await _context.Performances
                .AsNoTracking()
                .AnyAsync(p => p.Id == performanceId && p.DeletedAt == null);

            if (!performanceExists)
            {
                response.Success = false;
                response.Message = $"Performance with Id {performanceId} not found";
                return response;
            }

            response.Data = await _context.PerformanceSeats
                .AsNoTracking()
                .Include(ps => ps.Seat)
                .Where(ps => ps.PerformanceId == performanceId)
                .Select(ps => MapToDto(ps))
                .ToListAsync();

            response.Success = true;
            response.Message = "Performance seats retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving performance seats: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<PerformanceSeatResponseDto?>> GetByIdAsync(int id)
    {
        var response = new ServiceResponse<PerformanceSeatResponseDto?>();
        try
        {
            var seat = await _context.PerformanceSeats
                .AsNoTracking()
                .Include(ps => ps.Seat)
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (seat == null)
            {
                response.Success = false;
                response.Message = $"Performance seat with Id {id} not found";
                return response;
            }

            response.Data = MapToDto(seat);
            response.Success = true;
            response.Message = "Performance seat retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving performance seat: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<PerformanceSeatResponseDto>> UpdateStatusAsync(int id, PerformanceSeatUpdateStatusDto dto)
    {
        var response = new ServiceResponse<PerformanceSeatResponseDto>();
        try
        {
            var validStatuses = new[] { "available", "reserved", "occupied" };
            if (!validStatuses.Contains(dto.Status.ToLower()))
            {
                response.Success = false;
                response.Message = $"Invalid status. Valid values: {string.Join(", ", validStatuses)}";
                return response;
            }

            var seat = await _context.PerformanceSeats
                .Include(ps => ps.Seat)
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (seat == null)
            {
                response.Success = false;
                response.Message = $"Performance seat with Id {id} not found";
                return response;
            }

            seat.StatusString = dto.Status.ToLower();
            seat.UpdatedAt = DateTime.UtcNow;

            // Clear reservation if status changes from reserved
            if (dto.Status.ToLower() != "reserved")
                seat.ReservedUntil = null;

            await _context.SaveChangesAsync();

            response.Data = MapToDto(seat);
            response.Success = true;
            response.Message = "Performance seat status updated successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error updating performance seat status: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<PerformanceSeatResponseDto>> ScanAsync(int id, PerformanceSeatScanDto dto)
    {
        var response = new ServiceResponse<PerformanceSeatResponseDto>();
        try
        {
            var seat = await _context.PerformanceSeats
                .Include(ps => ps.Seat)
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (seat == null)
            {
                response.Success = false;
                response.Message = $"Performance seat with Id {id} not found";
                return response;
            }

            // Validate seat is occupied before scanning
            if (seat.StatusString != "occupied")
            {
                response.Success = false;
                response.Message = $"Cannot scan a seat with status '{seat.StatusString}'. Seat must be occupied";
                return response;
            }

            // Validate not already scanned
            if (seat.ScannedAt != null)
            {
                response.Success = false;
                response.Message = $"Seat already scanned at {seat.ScannedAt}";
                return response;
            }

            seat.ScannedByUserId = dto.ScannedByUserId;
            seat.ScannedAt = DateTime.UtcNow;
            seat.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            response.Data = MapToDto(seat);
            response.Success = true;
            response.Message = "Seat scanned successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error scanning seat: {ex.Message}";
        }
        return response;
    }

    private static PerformanceSeatResponseDto MapToDto(PerformanceSeat ps) => new()
    {
        Id = ps.Id,
        PerformanceId = ps.PerformanceId,
        SeatId = ps.SeatId,
        RowName = ps.Seat.RowName,
        RowNumber = ps.Seat.RowNumber,
        SeatNumber = ps.Seat.SeatNumber,
        Status = ps.StatusString,
        ReservedUntil = ps.ReservedUntil,
        ScannedByUserId = ps.ScannedByUserId,
        ScannedAt = ps.ScannedAt,
        CreatedAt = ps.CreatedAt,
        UpdatedAt = ps.UpdatedAt
    };
}