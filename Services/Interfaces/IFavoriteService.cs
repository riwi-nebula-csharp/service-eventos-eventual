using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;

namespace service_eventos_eventual.Services.Interfaces;

public interface IFavoriteService
{
    Task<ServiceResponse<IEnumerable<FavoriteResponseDto>>> GetAllByUserAsync(int userId);
    Task<ServiceResponse<FavoriteResponseDto>> CreateAsync(int userId, int playId);
    Task<ServiceResponse<bool>> DeleteAsync(int userId, int id);
}