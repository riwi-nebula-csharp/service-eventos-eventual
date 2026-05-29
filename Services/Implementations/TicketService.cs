using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Services.Implementations;

public class TicketService : ITicketService
{
    private readonly TeatroEventsDbContext _context;

    public TicketService(TeatroEventsDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<IEnumerable<TicketResponseDto>>> GetAllByUserAsync(int userId)
    {
        var response = new ServiceResponse<IEnumerable<TicketResponseDto>>();
        try
        {
            response.Data = await _context.Tickets
                .AsNoTracking()
                .Include(t => t.PerformanceSeat)
                    .ThenInclude(ps => ps.Seat)
                .Include(t => t.PerformanceSeat)
                    .ThenInclude(ps => ps.Performance)
                        .ThenInclude(perf => perf.Play)
                .Where(t => t.OwnerId == userId && t.DeletedAt == null)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => MapToDto(t))
                .ToListAsync();

            response.Success = true;
            response.Message = "Tickets retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving tickets: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<TicketResponseDto?>> GetByIdAsync(int id, int userId)
    {
        var response = new ServiceResponse<TicketResponseDto?>();
        try
        {
            var ticket = await _context.Tickets
                .AsNoTracking()
                .Include(t => t.PerformanceSeat).ThenInclude(ps => ps.Seat)
                .Include(t => t.PerformanceSeat).ThenInclude(ps => ps.Performance).ThenInclude(perf => perf.Play)
                .FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == userId && t.DeletedAt == null);

            if (ticket == null)
            {
                response.Success = false;
                response.Message = $"Ticket with Id {id} not found";
                return response;
            }

            response.Data    = MapToDto(ticket);
            response.Success = true;
            response.Message = "Ticket retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving ticket: {ex.Message}";
        }
        return response;
    }

    // Usado por el servicio de access para validar el QR escaneado
    public async Task<ServiceResponse<TicketResponseDto?>> GetByUuidAsync(Guid uuid)
    {
        var response = new ServiceResponse<TicketResponseDto?>();
        try
        {
            var ticket = await _context.Tickets
                .AsNoTracking()
                .Include(t => t.PerformanceSeat).ThenInclude(ps => ps.Seat)
                .Include(t => t.PerformanceSeat).ThenInclude(ps => ps.Performance).ThenInclude(perf => perf.Play)
                .FirstOrDefaultAsync(t => t.QrUuid == uuid && t.DeletedAt == null);

            if (ticket == null)
            {
                response.Success = false;
                response.Message = "Ticket not found or invalid QR";
                return response;
            }

            response.Data    = MapToDto(ticket);
            response.Success = true;
            response.Message = "Ticket found";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving ticket: {ex.Message}";
        }
        return response;
    }

    private static TicketResponseDto MapToDto(Models.Ticket t) => new()
    {
        Id                = t.Id,
        QrUuid            = t.QrUuid.ToString(), 
        QrUrl             = t.QrCode,
        PurchaseId        = t.PurchaseId,
        PerformanceSeatId = t.PerformanceSeatId,
        RowName           = t.PerformanceSeat.Seat.RowName.ToString(),
        SeatNumber        = t.PerformanceSeat.Seat.SeatNumber,
        PlayName          = t.PerformanceSeat.Performance.Play.Name,
        PerformanceDate   = t.PerformanceSeat.Performance.PerformanceDate,
        StartTime         = t.PerformanceSeat.Performance.StartTime,
        OwnerId           = t.OwnerId,
        OwnerEmail        = t.OwnerEmail,
        Status            = t.StatusString,
        SoldBy            = t.SoldBy,
        CreatedAt         = t.CreatedAt
    };
}
