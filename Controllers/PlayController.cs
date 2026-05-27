using Microsoft.AspNetCore.Mvc;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayController : ControllerBase
{
    private readonly IPlayService _service;

    public PlayController(IPlayService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _service.GetAllAsync();
        return Ok(response);
    }

    [HttpGet("deleted")]
    public async Task<IActionResult> GetAllDeleted()
    {
        var response = await _service.GetAllDeletedAsync();
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
    public async Task<IActionResult> Create([FromBody] PlayRequestDto dto)
    {
        var response = await _service.CreateAsync(dto);
        if (!response.Success) return BadRequest(response);
        return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PlayRequestDto dto)
    {
        var response = await _service.UpdateAsync(id, dto);
        if (!response.Success) return NotFound(response);
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