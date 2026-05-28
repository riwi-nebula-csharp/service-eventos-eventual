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

    private int? GetUserIdFromToken()
    {
        var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

        return int.TryParse(value, out var id) ? id : null;
    }

    private string? GetEmailFromToken()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value
               ?? User.FindFirst("email")?.Value;
    }

    // GET api/pqrs  — admin: all
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _service.GetAllAsync();
        return Ok(response);
    }

    // GET api/pqrs/my  — user: own
    [HttpGet("my")]
    public async Task<IActionResult> GetMine()
    {
        var userId = GetUserIdFromToken();
        if (userId == null) return Unauthorized();

        var response = await _service.GetAllByUserAsync(userId.Value);
        return Ok(response);
    }

    // GET api/pqrs/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _service.GetByIdAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    // POST api/pqrs
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PqrsRequestDto dto)
    {
        var userId = GetUserIdFromToken();
        if (userId == null) return Unauthorized();

        var email = GetEmailFromToken();
        if (email == null) return Unauthorized();

        var response = await _service.CreateAsync(userId.Value, email, dto);
        if (!response.Success) return BadRequest(response);
        return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
    }

    // PATCH api/pqrs/{id}/respond  — admin
    [HttpPatch("{id}/respond")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Respond(int id, [FromBody] PqrsRespondDto dto)
    {
        var response = await _service.RespondAsync(id, dto);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }

    // DELETE api/pqrs/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _service.DeleteAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }
}