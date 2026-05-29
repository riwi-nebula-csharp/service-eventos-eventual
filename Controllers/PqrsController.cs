using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PqrsController : ControllerBase
{
    private readonly IPqrsService _service;

    public PqrsController(IPqrsService service)
    {
        _service = service;
    }

    private int? GetUserIdFromToken() =>
        int.TryParse(
            User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out var id) ? id : null;

    private string? GetEmailFromToken() =>
        User.FindFirst("email")?.Value ?? User.FindFirst(ClaimTypes.Email)?.Value;

    // ─── Cliente ──────────────────────────────────────────────────────────

    /// <summary>Ver mis PQRS y sus respuestas</summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMine()
    {
        var userId = GetUserIdFromToken();
        if (userId == null) return Unauthorized();

        var response = await _service.GetAllByUserAsync(userId.Value);
        return Ok(response);
    }

    /// <summary>Crear una PQRS</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PqrsCreateDto dto)
    {
        var userId = GetUserIdFromToken();
        var email  = GetEmailFromToken();
        if (userId == null || email == null) return Unauthorized();

        var response = await _service.CreateAsync(userId.Value, email, dto);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }

    // ─── Admin ────────────────────────────────────────────────────────────

    /// <summary>Ver todas las PQRS — solo admin</summary>
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAll()
    {
        var response = await _service.GetAllAsync();
        return Ok(response);
    }

    /// <summary>Responder una PQRS — solo admin</summary>
    [HttpPatch("{id:int}/respond")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Respond(int id, [FromBody] PqrsRespondDto dto)
    {
        var adminId = GetUserIdFromToken();
        if (adminId == null) return Unauthorized();

        var response = await _service.RespondAsync(id, adminId.Value, dto);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
}
