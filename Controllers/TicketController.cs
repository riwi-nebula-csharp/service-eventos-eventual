using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketController : ControllerBase
{
    private readonly ITicketService _service;

    public TicketController(ITicketService service)
    {
        _service = service;
    }

    private int? GetUserIdFromToken() =>
        int.TryParse(
            User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out var id) ? id : null;

    /// <summary>Todos los tickets del usuario autenticado</summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyTickets()
    {
        var userId = GetUserIdFromToken();
        if (userId == null) return Unauthorized();

        var response = await _service.GetAllByUserAsync(userId.Value);
        return Ok(response);
    }

    /// <summary>Ticket específico del usuario autenticado</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserIdFromToken();
        if (userId == null) return Unauthorized();

        var response = await _service.GetByIdAsync(id, userId.Value);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    /// <summary>
    /// Buscar ticket por UUID del QR — usado por el servicio de access.
    /// Solo empleados con permiso "access" o admin pueden llamar esto.
    /// </summary>
    [HttpGet("by-uuid/{uuid}")]
    [Authorize(Roles = "admin,employee")]
    public async Task<IActionResult> GetByUuid(Guid uuid)  // Guid, no string
    {
        var response = await _service.GetByUuidAsync(uuid);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }
}