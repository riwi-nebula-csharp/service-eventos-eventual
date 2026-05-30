using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class MetricsController : ControllerBase
{
    private readonly IMetricsService _service;

    public MetricsController(IMetricsService service)
    {
        _service = service;
    }

    /// <summary>
    /// Métricas generales del teatro en un rango de fechas.
    /// Si no se especifican fechas, devuelve los últimos 30 días.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMetrics(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        // Si no mandan fechas usamos los últimos 30 días por defecto
        var dateFrom = from ?? DateTime.UtcNow.AddDays(-30);
        var dateTo   = to   ?? DateTime.UtcNow;

        if (dateFrom > dateTo)
        {
            return BadRequest(new
            {
                success = false,
                message = "'from' no puede ser mayor que 'to'"
            });
        }

        var response = await _service.GetMetricsAsync(dateFrom, dateTo);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }
}