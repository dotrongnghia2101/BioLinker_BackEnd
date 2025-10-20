using BioLinker.DTO.PaymentDTO;
using BioLinker.Enities;
using BioLinker.Respository.PaymentRepo;
using System.Text.Json;

namespace BioLinker.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repo;
        private readonly IPayOSService _payos;

        public PaymentService(IPaymentRepository repo, IPayOSService payos)
        {
            _repo = repo;
            _payos = payos;
        }
        public async Task<PayOSResponse> CreatePaymentAsync(PayOSRequest dto)
        {
            var payment = new Payment
            {
                PaymentId = Guid.NewGuid().ToString(),
                UserId = dto.UserId,
                PlanId = dto.PlanId,
                OrderCode = dto.OrderCode,
                Amount = (decimal)dto.Amount,
                Description = dto.Description,
                Status = "Pending",
                Method = "PayOS"
            };

            await _repo.CreateAsync(payment);

            var result = await _payos.CreatePaymentAsync(dto);
            payment.PaymentUrl = result.PaymentLink;

            await _repo.UpdateAsync(payment);
            return result;
        }

        public async Task<bool> HandleWebhookAsync(JsonElement payload, string checksumHeader)
        {
            var raw = payload.ToString() ?? "";
            if (!_payos.VerifyChecksum(raw, checksumHeader))
                return false;

            string? orderCode = payload.GetProperty("orderCode").GetString();
            string? status = payload.GetProperty("status").GetString();
            string? transactionId = payload.GetProperty("transactionId").GetString();

            var payment = await _repo.GetByOrderCodeAsync(orderCode!);
            if (payment == null) return false;

            payment.Checksum = checksumHeader;
            if (status == "PAID")
            {
                payment.Status = "Paid";
                payment.TransactionId = transactionId;
                payment.PaidAt = DateTime.UtcNow;
            }
            else if (status == "CANCELED")
                payment.Status = "Canceled";
            else
                payment.Status = "Failed";

            await _repo.UpdateAsync(payment);
            return true;
        }
    }
 }

