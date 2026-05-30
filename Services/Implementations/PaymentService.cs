using Microsoft.EntityFrameworkCore;
using service_eventos_eventual.Database.Data;
using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;
using service_eventos_eventual.Services.Interfaces;
using Stripe;

namespace service_eventos_eventual.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly TeatroEventsDbContext _context;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(TeatroEventsDbContext context, ILogger<PaymentService> logger)
    {
        _context = context;
        _logger  = logger;
    }

    public async Task<ServiceResponse<PaymentIntentResponseDto>> CreatePaymentIntentAsync(
        int performanceId, int ticketCount)
    {
        var response = new ServiceResponse<PaymentIntentResponseDto>();
        try
        {
            var performance = await _context.Performances
                .FirstOrDefaultAsync(p => p.Id == performanceId && p.DeletedAt == null);

            if (performance == null)
            {
                response.Success = false;
                response.Message = $"Performance with Id {performanceId} not found";
                return response;
            }

            if (performance.StatusString != "on_sale")
            {
                response.Success = false;
                response.Message = $"Performance is not available for purchase (status: {performance.StatusString})";
                return response;
            }

            
            var amount = (long)(performance.TicketPrice * ticketCount * 100);

            var options = new PaymentIntentCreateOptions
            {
                Amount   = amount,
                Currency = "cop",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled         = true,
                    AllowRedirects  = "never"  
                },
                Metadata = new Dictionary<string, string>
                {
                    { "performanceId", performanceId.ToString() },
                    { "ticketCount",   ticketCount.ToString() }
                }
            };

            var service       = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            response.Data = new PaymentIntentResponseDto
            {
                ClientSecret    = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.Id,
                Amount          = (decimal)performance.TicketPrice * ticketCount,
                Currency        = "COP"
            };

            response.Success = true;
            response.Message = "Payment intent created successfully";
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error creating payment intent");
            response.Success = false;
            response.Message = $"Payment error: {ex.StripeError.Message}";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error creating payment intent: {ex.Message}";
        }
        return response;
    }

    public async Task<bool> VerifyPaymentAsync(string paymentIntentId)
    {
        try
        {
            var service       = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId);

            // El pago es válido solo si Stripe confirma que está pagado
            return paymentIntent.Status == "succeeded";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment {PaymentIntentId}", paymentIntentId);
            return false;
        }
    }
}