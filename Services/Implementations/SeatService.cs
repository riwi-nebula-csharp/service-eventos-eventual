using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Models;
using service_eventos_eventual.Response;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Services.Implementations;

public class SeatService : ISeatService
{
    private readonly TeatroEventsDbContext _context;

    public SeatService(TeatroEventsDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<IEnumerable<SeatResponseDto>>> GetAllAsync()
    {
        var response = new ServiceResponse<IEnumerable<SeatResponseDto>>();
        try
        {
            var query = _context.Seats.AsNoTracking();

            response.Data = await query
                .Select(s => new SeatResponseDto
                {
                    Id = s.Id,
                    RowName = s.RowName,
                    RowNumber = s.RowNumber,
                    SeatNumber = s.SeatNumber,
                    SeatOrder = s.SeatOrder,
                    CreatedAt = s.CreatedAt
                }).ToListAsync();

            response.Success = true;
            response.Message = "Asientos obtenidos correctamente";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al obtener asientos: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<SeatResponseDto?>> GetByIdAsync(int id)
    {
        var response = new ServiceResponse<SeatResponseDto?>();
        try
        {
            var seat = await _context.Seats
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seat == null)
            {
                response.Success = false;
                response.Message = $"Asiento con Id {id} no encontrado";
                return response;
            }

            response.Data = new SeatResponseDto
            {
                Id = seat.Id,
                RowName = seat.RowName,
                RowNumber = seat.RowNumber,
                SeatNumber = seat.SeatNumber,
                SeatOrder = seat.SeatOrder,
                CreatedAt = seat.CreatedAt
            };
            response.Success = true;
            response.Message = "Asiento obtenido correctamente";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al obtener asiento: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<SeatResponseDto>> CreateAsync(SeatRequestDto dto)
    {
        var response = new ServiceResponse<SeatResponseDto>();
        try
        {
            var seat = new Seat
            {
                RowName = dto.RowName,
                RowNumber = dto.RowNumber,
                SeatNumber = dto.SeatNumber,
                SeatOrder = dto.SeatOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            response.Data = new SeatResponseDto
            {
                Id = seat.Id,
                RowName = seat.RowName,
                RowNumber = seat.RowNumber,
                SeatNumber = seat.SeatNumber,
                SeatOrder = seat.SeatOrder,
                CreatedAt = seat.CreatedAt
            };
            response.Success = true;
            response.Message = "Asiento creado correctamente";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al crear asiento: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<SeatResponseDto>> UpdateAsync(int id, SeatRequestDto dto)
    {
        var response = new ServiceResponse<SeatResponseDto>();
        try
        {
            var seat = await _context.Seats
                .FirstOrDefaultAsync(s => s.Id == id);

            if (seat == null)
            {
                response.Success = false;
                response.Message = $"Asiento con Id {id} no encontrado";
                return response;
            }

            seat.RowName = dto.RowName;
            seat.RowNumber = dto.RowNumber;
            seat.SeatNumber = dto.SeatNumber;
            seat.SeatOrder = dto.SeatOrder;
            seat.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            response.Data = new SeatResponseDto
            {
                Id = seat.Id,
                RowName = seat.RowName,
                RowNumber = seat.RowNumber,
                SeatNumber = seat.SeatNumber,
                SeatOrder = seat.SeatOrder,
                CreatedAt = seat.CreatedAt
            };
            response.Success = true;
            response.Message = "Asiento actualizado correctamente";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error al actualizar asiento: {ex.Message}";
        }
        return response;
    }
}