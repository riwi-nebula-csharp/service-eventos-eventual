using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;

namespace service_eventos_eventual.Services.Interfaces;

public interface IMetricsService
{
    Task<ServiceResponse<MetricsResponseDto>> GetMetricsAsync(DateTime from, DateTime to);
}