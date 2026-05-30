using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Services.Interfaces;

namespace service_eventos_eventual.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _service;
    private readonly IConfiguration  _config;

    public PaymentController(IPaymentService service, IConfiguration config)
    {
        _service = service;
        _config  = config;
    }

    /// <summary>
    /// Paso 1 del flujo de pago.
    /// El frontend llama esto primero para inicializar el pago con Stripe.
    /// Devuelve el clientSecret que Stripe Elements necesita para mostrar el formulario de tarjeta.
    /// </summary>
    [HttpPost("create-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentDto dto)
    {
        var response = await _service.CreatePaymentIntentAsync(dto.PerformanceId, dto.TicketCount);
        if (!response.Success) return BadRequest(response);
        return Ok(response);
    }

    /// <summary>
    /// Devuelve la publishable key para que el frontend inicialice Stripe Elements.
    /// </summary>
    [HttpGet("config")]
    public IActionResult GetConfig()
    {
        return Ok(new
        {
            success        = true,
            publishableKey = _config["Stripe:PublishableKey"]
        });
    }
}