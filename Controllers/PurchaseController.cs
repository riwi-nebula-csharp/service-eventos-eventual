using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _service;

    public PurchaseController(IPurchaseService service)
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
    
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAll()
    {
        var response = await _service.GetAllAsync();
        return Ok(response);
    }
    
    [HttpGet("my")]
    public async Task<IActionResult> GetMine()
    {
        var userId = GetUserIdFromToken();
        if (userId == null) return Unauthorized();

        var response = await _service.GetAllByUserAsync(userId.Value);
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _service.GetByIdAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PurchaseRequestDto dto)
    {
        var userId = GetUserIdFromToken();
        if (userId == null) return Unauthorized();

        var email = GetEmailFromToken();
        if (email == null) return Unauthorized();
        
        var response = await _service.CreateAsync(userId.Value, email, dto);
        if (!response.Success) return BadRequest(response);
        return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] PurchaseUpdateStatusDto dto)
    {
        var response = await _service.UpdateStatusAsync(id, dto);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _service.DeleteAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }
}