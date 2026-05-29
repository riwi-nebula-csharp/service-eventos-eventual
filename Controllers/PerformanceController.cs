using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PerformanceController : ControllerBase
{
    private readonly IPerformanceService _service;

    public PerformanceController(IPerformanceService service)
    {
        _service = service;
    }

    // Público — el catálogo de funciones es visible para todos
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _service.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _service.GetByIdAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    // Solo admin
    [HttpGet("deleted")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllDeleted()
    {
        var response = await _service.GetAllDeletedAsync();
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] PerformanceRequestDto dto)
    {
        var response = await _service.CreateAsync(dto);
        if (!response.Success) return BadRequest(response);
        return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] PerformanceRequestDto dto)
    {
        var response = await _service.UpdateAsync(id, dto);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    // Fix: ruta correcta era /api/performance/{id}, no /api/play/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await _service.DeleteAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }
    
    /// <summary>
    /// Mapa de asientos de una función — público, cualquiera puede consultarlo
    /// para ver qué asientos están disponibles antes de comprar
    /// </summary>
    [HttpGet("{id}/seats")]
    public async Task<IActionResult> GetSeatMap(int id)
    {
        var response = await _service.GetSeatMapAsync(id);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }
}
