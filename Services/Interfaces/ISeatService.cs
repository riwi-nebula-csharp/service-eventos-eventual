using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;

namespace service_eventos_eventual.Services.Interfaces;

public interface ISeatService
{
    Task<ServiceResponse<IEnumerable<SeatResponseDto>>> GetAllAsync();
    Task<ServiceResponse<SeatResponseDto?>> GetByIdAsync(int id);
    Task<ServiceResponse<SeatResponseDto>> CreateAsync(SeatRequestDto dto);
    Task<ServiceResponse<SeatResponseDto>> UpdateAsync(int id, SeatRequestDto dto);
}