using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;

namespace service_eventos_eventual.Services.Interfaces;



public interface IPerformanceService
{
    Task<ServiceResponse<IEnumerable<PerformanceResponseDto>>> GetAllAsync();
    Task<ServiceResponse<IEnumerable<PerformanceResponseDto>>> GetAllDeletedAsync();
    Task<ServiceResponse<PerformanceResponseDto?>> GetByIdAsync(int id);
    
    Task<ServiceResponse<SeatMapDto>> GetSeatMapAsync(int performanceId);
    Task<ServiceResponse<PerformanceResponseDto>> CreateAsync(PerformanceRequestDto dto);
    Task<ServiceResponse<PerformanceResponseDto>> UpdateAsync(int id, PerformanceRequestDto dto);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
}