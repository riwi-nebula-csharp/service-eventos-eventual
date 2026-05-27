using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;

namespace service_eventos_eventual.Services.Interfaces;

public interface IPurchaseService
{
    Task<ServiceResponse<IEnumerable<PurchaseResponseDto>>> GetAllAsync();
    Task<ServiceResponse<IEnumerable<PurchaseResponseDto>>> GetAllByUserAsync(int buyerId);
    Task<ServiceResponse<PurchaseResponseDto?>> GetByIdAsync(int id);
    Task<ServiceResponse<PurchaseResponseDto>> CreateAsync(int buyerId, PurchaseRequestDto dto);
    Task<ServiceResponse<PurchaseResponseDto>> UpdateStatusAsync(int id, PurchaseUpdateStatusDto dto);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
}