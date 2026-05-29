using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;

namespace service_eventos_eventual.Services.Interfaces;

public interface ITicketService
{
    Task<ServiceResponse<IEnumerable<TicketResponseDto>>> GetAllByUserAsync(int userId);
    Task<ServiceResponse<TicketResponseDto?>>             GetByIdAsync(int id, int userId);
    Task<ServiceResponse<TicketResponseDto?>> GetByUuidAsync(Guid uuid); // para el scanner de acceso
}