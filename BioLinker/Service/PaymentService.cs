using BioLinker.DTO.PaymentDTO;
using BioLinker.Enities;
using BioLinker.Respository.PaymentRepo;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using System.Text.Json;

namespace BioLinker.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repo;
        private readonly PayOS _payos;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IPaymentRepository repo, PayOS payos, ILogger<PaymentService> logger)
        {
            _repo = repo;
            _payos = payos;
            _logger = logger;
        }

        public async Task<PayOSResponse> CreatePaymentAsync(PayOSRequest dto)
        {
            try
            {
                long orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var items = new List<ItemData>
                {
                    new ItemData(dto.ItemName ?? "Gói BioLink Pro", 1, (int)dto.Amount)
                };

                var paymentData = new PaymentData(
                    orderCode,
                    (int)dto.Amount,
                    dto.Description,
                    items,
                    dto.CancelUrl,
                    dto.ReturnUrl
                );

                _logger.LogInformation(" Gửi yêu cầu PayOS: {data}", paymentData);

                var res = await _payos.createPaymentLink(paymentData);

                var payment = new Payment
                {
                    PaymentId = Guid.NewGuid().ToString(),
                    UserId = dto.UserId,
                    PlanId = dto.PlanId,
                    OrderCode = orderCode.ToString(),
                    Amount = dto.Amount,
                    Description = dto.Description,
                    Status = "Pending",
                    Method = "PayOS",
                    PaymentUrl = res.checkoutUrl,
                    CreatedAt = DateTime.UtcNow
                };

                await _repo.CreateAsync(payment);

                _logger.LogInformation(" Tạo thanh toán thành công: {orderCode}", orderCode);

                return new PayOSResponse
                {
                    OrderCode = orderCode.ToString(),
                    PaymentLink = res.checkoutUrl,
                    Message = "Tạo liên kết thanh toán thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Lỗi tạo thanh toán PayOS");
                throw;
            }
        }

        public async Task<bool> HandleWebhookAsync(string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(body) || body.Trim() == "{}")
                {
                    _logger.LogInformation("Webhook test (body rỗng hoặc {}) → trả về 200 OK cho PayOS.");
                    return true;
                }

                var webhook = JsonSerializer.Deserialize<WebhookType>(body);
                if (webhook?.data?.orderCode == 123)
                {
                    _logger.LogInformation("Webhook test (orderCode=123) từ PayOS → trả về 200 OK.");
                    return true;
                }

                // Xác minh chữ ký và lấy data
                WebhookData webhookData = _payos.verifyPaymentWebhookData(webhook);

                long orderCode = webhookData.orderCode;
                int amount = webhookData.amount;                    // nếu cần
                string code = webhookData.code;                     // "00" = thành công
                _logger.LogInformation("📦 Nhận webhook orderCode={OrderCode}, code={Code}", orderCode, code);

                // Tìm payment trong DB theo orderCode
                var payment = await _repo.GetByOrderCodeAsync(orderCode.ToString());
                if (payment == null)
                {
                    _logger.LogWarning("Không tìm thấy đơn hàng orderCode={OrderCode}", orderCode);
                    return false;
                }

                // Cập nhật trạng thái
                if (code == "00")
                {
                    payment.Status = "Paid";
                    payment.PaidAt = DateTime.UtcNow;
                }
                else
                {
                    payment.Status = "Failed";
                }

                await _repo.UpdateAsync(payment);
                _logger.LogInformation("Cập nhật trạng thái thanh toán {OrderCode} → {Status}", orderCode, payment.Status);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xử lý webhook PayOS");
                return false;
            }
        }
    }
}