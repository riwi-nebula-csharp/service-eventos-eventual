using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoriteController : ControllerBase
{
    private readonly IFavoriteService _service;

    public FavoriteController(IFavoriteService service)
    {
        _service = service;
    }

    private int? GetUserIdFromToken()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

        return int.TryParse(value, out var id) ? id : null;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserIdFromToken();
        if (userId == null) return Unauthorized();

        var response = await _service.GetAllByUserAsync(userId.Value);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FavoriteRequestDto dto)
    {
        var userId = GetUserIdFromToken();
        if (userId == null) return Unauthorized();

        var response = await _service.CreateAsync(userId.Value, dto.PlayId);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserIdFromToken();
        if (userId == null) return Unauthorized();

        var response = await _service.DeleteAsync(userId.Value, id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }
}