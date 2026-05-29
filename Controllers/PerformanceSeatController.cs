using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Controllers;

// Controllers/PerformanceSeatController.cs
[ApiController]
[Route("api/[controller]")]
public class PerformanceSeatController : ControllerBase
{
    private readonly IPerformanceSeatService _service;

    public PerformanceSeatController(IPerformanceSeatService service)
    {
        _service = service;
    }

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

    [HttpGet("performance/{performanceId}")]
    public async Task<IActionResult> GetByPerformance(int performanceId)
    {
        var response = await _service.GetByPerformanceAsync(performanceId);
        if (!response.Success) return NotFound(response);
        return Ok(response);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] PerformanceSeatUpdateStatusDto dto)
    {
        var response = await _service.UpdateStatusAsync(id, dto);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }

    [HttpPatch("{id}/scan")]
    [Authorize]
    public async Task<IActionResult> Scan(int id, [FromBody] PerformanceSeatScanDto dto)
    {
        var response = await _service.ScanAsync(id, dto);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
}