using BioLinker.DTO.PaymentDTO;
using System.Text.Json;

namespace BioLinker.Service
{
    public interface IPaymentService
    {
        Task<PayOSResponse> CreatePaymentAsync(PayOSRequest dto);
        Task<bool> HandleWebhookAsync(string body);
    }
}
