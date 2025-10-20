using BioLinker.DTO.PaymentDTO;

namespace BioLinker.Service
{
    public interface IPayOSService
    {
        Task<PayOSResponse> CreatePaymentAsync(PayOSRequest dto);
        bool VerifyChecksum(string payload, string receivedChecksum);
    }
}
