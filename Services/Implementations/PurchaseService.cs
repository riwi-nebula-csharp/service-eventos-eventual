using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Models;
using service_eventos_eventual.Response;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Services.Implementations;

public class PurchaseService : IPurchaseService
{
    private readonly TeatroEventsDbContext _context;
    private readonly IQrService _qrService;
    private readonly ILogger<PurchaseService> _logger;

    public PurchaseService(
        TeatroEventsDbContext context,
        IQrService qrService,
        ILogger<PurchaseService> logger)
    {
        _context   = context;
        _qrService = qrService;
        _logger    = logger;
    }

    // ─── Queries ──────────────────────────────────────────────────────────

    public async Task<ServiceResponse<IEnumerable<PurchaseResponseDto>>> GetAllAsync()
    {
        var response = new ServiceResponse<IEnumerable<PurchaseResponseDto>>();
        try
        {
            response.Data = await _context.Purchases
                .AsNoTracking()
                .Include(p => p.Performance).ThenInclude(perf => perf.Play)
                .Where(p => p.DeletedAt == null)
                .Select(p => MapToDto(p))
                .ToListAsync();

            response.Success = true;
            response.Message = "Purchases retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving purchases: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<IEnumerable<PurchaseResponseDto>>> GetAllByUserAsync(int buyerId)
    {
        var response = new ServiceResponse<IEnumerable<PurchaseResponseDto>>();
        try
        {
            response.Data = await _context.Purchases
                .AsNoTracking()
                .Include(p => p.Performance).ThenInclude(perf => perf.Play)
                .Where(p => p.BuyerId == buyerId && p.DeletedAt == null)
                .Select(p => MapToDto(p))
                .ToListAsync();

            response.Success = true;
            response.Message = "User purchases retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving user purchases: {ex.Message}";
        }
        return response;
    }

    public async Task<ServiceResponse<PurchaseResponseDto?>> GetByIdAsync(int id)
    {
        var response = new ServiceResponse<PurchaseResponseDto?>();
        try
        {
            var purchase = await _context.Purchases
                .AsNoTracking()
                .Include(p => p.Performance).ThenInclude(perf => perf.Play)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (purchase == null)
            {
                response.Success = false;
                response.Message = $"Purchase with Id {id} not found";
                return response;
            }

            response.Data    = MapToDto(purchase);
            response.Success = true;
            response.Message = "Purchase retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error retrieving purchase: {ex.Message}";
        }
        return response;
    }

    // ─── Create (flujo completo) ──────────────────────────────────────────

    public async Task<ServiceResponse<PurchaseResponseDto>> CreateAsync(
    int buyerId, string buyerEmail, PurchaseRequestDto dto)
{
    var response = new ServiceResponse<PurchaseResponseDto>();

    await using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        // 1. Validar función
        var performance = await _context.Performances
            .Include(p => p.Play)
            .FirstOrDefaultAsync(p => p.Id == dto.PerformanceId && p.DeletedAt == null);

        if (performance == null)
        {
            response.Success = false;
            response.Message = $"Performance with Id {dto.PerformanceId} not found";
            return response;
        }

        if (performance.StatusString != "on_sale")
        {
            response.Success = false;
            response.Message = $"Performance is not available for purchase (status: {performance.StatusString})";
            return response;
        }

        if (dto.PaymentMethod != "online" && dto.PaymentMethod != "box_office")
        {
            response.Success = false;
            response.Message = "Invalid payment method. Use 'online' or 'box_office'";
            return response;
        }

        // 2. Cargar los asientos específicos que eligió el cliente
        var chosenSeats = await _context.PerformanceSeats
            .Include(ps => ps.Seat)
            .Where(ps => ps.PerformanceId == dto.PerformanceId &&
                         dto.SeatNumbers.Contains(ps.Seat.SeatOrder))
            .ToListAsync();

        // Verificar que todos los IDs enviados existen y son de esta función
        if (chosenSeats.Count != dto.SeatNumbers.Count)
        {
            var foundNumbers   = chosenSeats.Select(ps => ps.Seat.SeatOrder).ToList();
            var invalidNumbers = dto.SeatNumbers.Except(foundNumbers).ToList();
            response.Success  = false;
            response.Message  = $"Seat number(s) not found in this performance: {string.Join(", ", invalidNumbers)}";
            return response;
        }

        // Verificar que todos estén disponibles
        var occupiedSeats = chosenSeats.Where(ps => ps.StatusString != "available").ToList();
        if (occupiedSeats.Any())
        {
            var occupiedNumbers = occupiedSeats.Select(ps => ps.Seat.SeatOrder).ToList();
            response.Success = false;
            response.Message = $"The following seats are no longer available: {string.Join(", ", occupiedNumbers)}";
            return response;
        }

        // 3. Crear la compra
        var purchase = new Purchase
        {
            BuyerId             = buyerId,
            BuyerEmail          = buyerEmail,
            PerformanceId       = dto.PerformanceId,
            TicketCount         = chosenSeats.Count,
            TotalPrice          = performance.TicketPrice * chosenSeats.Count,
            PaymentMethodString = dto.PaymentMethod,
            StatusString        = "completed",
            StripePaymentId     = dto.StripePaymentId,
            CreatedAt           = DateTime.UtcNow,
            UpdatedAt           = DateTime.UtcNow
        };

        _context.Purchases.Add(purchase);
        await _context.SaveChangesAsync();

        // 4. Por cada asiento elegido: ocupar + generar QR + crear Ticket
        var tickets = new List<Ticket>();

        foreach (var seat in chosenSeats)
        {
            seat.StatusString = "occupied";
            seat.UpdatedAt    = DateTime.UtcNow;

            var uuid = Guid.NewGuid();

            string qrUrl;
            try
            {
                qrUrl = await _qrService.GenerateAndUploadAsync(uuid.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error subiendo QR para uuid {Uuid}", uuid);
                qrUrl = string.Empty;
            }

            tickets.Add(new Ticket
            {
                QrUuid            = uuid,
                QrCode            = qrUrl,
                PurchaseId        = purchase.Id,
                PerformanceSeatId = seat.Id,
                OwnerId           = buyerId,
                OwnerEmail        = buyerEmail,
                PriceAtPurchase   = (decimal)performance.TicketPrice,
                StatusString      = "Active",
                SoldBy            = null,
                CreatedAt         = DateTime.UtcNow,
                UpdatedAt         = DateTime.UtcNow
            });
        }

        await _context.Tickets.AddRangeAsync(tickets);
        await _context.SaveChangesAsync();

        // 5. Revisar si se agotó la función
        var remaining = await _context.PerformanceSeats
            .CountAsync(ps => ps.PerformanceId == dto.PerformanceId &&
                              ps.StatusString  == "available");

        if (remaining == 0)
        {
            performance.StatusString = "sold_out";
            performance.UpdatedAt    = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        await transaction.CommitAsync();

        purchase.Performance = performance;
        response.Data        = MapToDto(purchase);
        response.Success     = true;
        response.Message     = $"Purchase completed. {tickets.Count} ticket(s) generated";
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        response.Success = false;
        response.Message = $"Error creating purchase: {ex.Message}";
    }
    return response;
}

    // ─── UpdateStatus ─────────────────────────────────────────────────────

    public async Task<ServiceResponse<PurchaseResponseDto>> UpdateStatusAsync(int id, PurchaseUpdateStatusDto dto)
    {
        var response = new ServiceResponse<PurchaseResponseDto>();
        try
        {
            var validStatuses = new[] { "pending", "completed", "failed", "refunded" };
            if (!validStatuses.Contains(dto.Status))
            {
                response.Success = false;
                response.Message = $"Invalid status. Valid values: {string.Join(", ", validStatuses)}";
                return response;
            }

            var purchase = await _context.Purchases
                .Include(p => p.Performance).ThenInclude(perf => perf.Play)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (purchase == null)
            {
                response.Success = false;
                response.Message = $"Purchase with Id {id} not found";
                return response;
            }

            purchase.StatusString = dto.Status;
            purchase.UpdatedAt    = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            response.Data    = MapToDto(purchase);
            response.Success = true;
            response.Message = "Purchase status updated successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error updating purchase status: {ex.Message}";
        }
        return response;
    }

    // ─── Delete ───────────────────────────────────────────────────────────

    public async Task<ServiceResponse<bool>> DeleteAsync(int id)
    {
        var response = new ServiceResponse<bool>();
        try
        {
            var purchase = await _context.Purchases
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (purchase == null)
            {
                response.Success = false;
                response.Data    = false;
                response.Message = $"Purchase with Id {id} not found";
                return response;
            }

            purchase.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data    = true;
            response.Message = "Purchase deleted successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error deleting purchase: {ex.Message}";
        }
        return response;
    }

    // ─── Mapping ──────────────────────────────────────────────────────────

    private static PurchaseResponseDto MapToDto(Purchase p) => new()
    {
        Id              = p.Id,
        BuyerId         = p.BuyerId,
        BuyerEmail      = p.BuyerEmail,
        PerformanceId   = p.PerformanceId,
        PerformanceDate = p.Performance.PerformanceDate,
        StartTime       = p.Performance.StartTime,
        PlayName        = p.Performance.Play.Name,
        TicketCount     = p.TicketCount,
        TotalPrice      = (decimal)p.TotalPrice,
        PaymentMethod   = p.PaymentMethodString,
        Status          = p.StatusString,
        StripePaymentId = p.StripePaymentId,
        CreatedAt       = p.CreatedAt,
        UpdatedAt       = p.UpdatedAt
    };
}
