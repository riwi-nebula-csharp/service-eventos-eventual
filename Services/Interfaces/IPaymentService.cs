using service_eventos_eventual.DTOs;
using service_eventos_eventual.Response;

namespace service_eventos_eventual.Services.Interfaces;

public interface IPaymentService
{
    Task<ServiceResponse<PaymentIntentResponseDto>> CreatePaymentIntentAsync(
        int performanceId, int ticketCount);

    Task<bool> VerifyPaymentAsync(string paymentIntentId);
}