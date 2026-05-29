using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;

namespace service_eventos_eventual.Services.Interfaces;

// Services/Interfaces/IPerformanceSeatService.cs
public interface IPerformanceSeatService
{
    Task<ServiceResponse<IEnumerable<PerformanceSeatResponseDto>>> GetAllAsync();
    Task<ServiceResponse<IEnumerable<PerformanceSeatResponseDto>>> GetByPerformanceAsync(int performanceId);
    Task<ServiceResponse<PerformanceSeatResponseDto?>> GetByIdAsync(int id);
    Task<ServiceResponse<PerformanceSeatResponseDto>> UpdateStatusAsync(int id, PerformanceSeatUpdateStatusDto dto);
    Task<ServiceResponse<PerformanceSeatResponseDto>> ScanAsync(int id, PerformanceSeatScanDto dto);
}