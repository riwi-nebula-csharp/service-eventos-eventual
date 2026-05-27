using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;

namespace service_eventos_eventual.Services.Interfaces;

public interface IPlayService
{
    Task<ServiceResponse<IEnumerable<PlayResponseDto>>> GetAllAsync();
    Task<ServiceResponse<IEnumerable<PlayResponseDto>>> GetAllDeletedAsync();
    Task<ServiceResponse<PlayResponseDto?>> GetByIdAsync(int id);
    Task<ServiceResponse<PlayResponseDto>> CreateAsync(PlayRequestDto dto);
    Task<ServiceResponse<PlayResponseDto>> UpdateAsync(int id, PlayRequestDto dto);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
}