using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;

namespace service_eventos_eventual.Services.Interfaces;

public interface IPqrsService
{
    // Cliente
    Task<ServiceResponse<IEnumerable<PqrsResponseDto>>> GetAllByUserAsync(int userId);
    Task<ServiceResponse<PqrsResponseDto>>              CreateAsync(int userId, string userEmail, PqrsCreateDto dto);

    // Admin
    Task<ServiceResponse<IEnumerable<PqrsResponseDto>>> GetAllAsync();
    Task<ServiceResponse<PqrsResponseDto>>              RespondAsync(int id, int adminId, PqrsRespondDto dto);
}