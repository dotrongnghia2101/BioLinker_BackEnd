using BioLinker.DTO.PaymentDTO;
using BioLinker.Enities;
using System.Text.Json;

namespace BioLinker.Service
{
    public interface IPaymentService
    {
        Task<PayOSResponse> CreatePaymentAsync(PayOSRequest dto);
        Task<bool> HandleWebhookAsync(string body);
        Task<IEnumerable<PaymentResponse>> GetPaymentsByUserAsync(string userId);
        Task<IEnumerable<PaymentResponse>> GetAllPaymentsAsync();
        Task<bool> UpgradeToProPlanAsync(string userId);
    }
}
