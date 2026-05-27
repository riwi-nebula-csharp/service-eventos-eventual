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

    public PurchaseService(TeatroEventsDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResponse<IEnumerable<PurchaseResponseDto>>> GetAllAsync()
    {
        var response = new ServiceResponse<IEnumerable<PurchaseResponseDto>>();
        try
        {
            response.Data = await _context.Purchases
                .AsNoTracking()
                .Include(p => p.Performance)
                    .ThenInclude(perf => perf.Play)
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
                .Include(p => p.Performance)
                    .ThenInclude(perf => perf.Play)
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
                .Include(p => p.Performance)
                    .ThenInclude(perf => perf.Play)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (purchase == null)
            {
                response.Success = false;
                response.Message = $"Purchase with Id {id} not found";
                return response;
            }

            response.Data = MapToDto(purchase);
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

    public async Task<ServiceResponse<PurchaseResponseDto>> CreateAsync(int buyerId, string buyerEmail, PurchaseRequestDto dto)
    {
        var response = new ServiceResponse<PurchaseResponseDto>();
        try
        {
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

            var purchase = new Purchase
            {
                BuyerId = buyerId,
                BuyerEmail = buyerEmail,
                PerformanceId = dto.PerformanceId,
                TicketCount = dto.TicketCount,
                TotalPrice = performance.TicketPrice * dto.TicketCount,
                PaymentMethodString = dto.PaymentMethod,
                StatusString = "pending",
                StripePaymentId = dto.StripePaymentId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            purchase.Performance = performance;

            response.Data = MapToDto(purchase);
            response.Success = true;
            response.Message = "Purchase created successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error creating purchase: {ex.Message}";
        }
        return response;
    }

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
                .Include(p => p.Performance)
                    .ThenInclude(perf => perf.Play)
                .FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null);

            if (purchase == null)
            {
                response.Success = false;
                response.Message = $"Purchase with Id {id} not found";
                return response;
            }

            purchase.StatusString = dto.Status;
            purchase.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            response.Data = MapToDto(purchase);
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
                response.Data = false;
                response.Message = $"Purchase with Id {id} not found";
                return response;
            }

            purchase.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Data = true;
            response.Message = "Purchase deleted successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error deleting purchase: {ex.Message}";
        }
        return response;
    }

    private static PurchaseResponseDto MapToDto(Purchase p) => new()
    {
        Id = p.Id,
        BuyerId = p.BuyerId,
        BuyerEmail = p.BuyerEmail,
        PerformanceId = p.PerformanceId,
        PerformanceDate = p.Performance.PerformanceDate,
        StartTime = p.Performance.StartTime,
        PlayName = p.Performance.Play.Name,
        TicketCount = p.TicketCount,
        TotalPrice = p.TotalPrice,
        PaymentMethod = p.PaymentMethodString,
        Status = p.StatusString,
        StripePaymentId = p.StripePaymentId,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}