using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;

namespace service_eventos_eventual.Services.Interfaces;

public interface IPqrsService
{
    Task<ServiceResponse<IEnumerable<PqrsResponseDto>>> GetAllAsync();
    Task<ServiceResponse<IEnumerable<PqrsResponseDto>>> GetAllByUserAsync(int userId);
    Task<ServiceResponse<PqrsResponseDto?>> GetByIdAsync(int id);
    Task<ServiceResponse<PqrsResponseDto>> CreateAsync(int userId, string userEmail, PqrsRequestDto dto);
    Task<ServiceResponse<PqrsResponseDto>> RespondAsync(int id, PqrsRespondDto dto);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
}