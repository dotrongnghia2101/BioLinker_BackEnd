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
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IPaymentRepository repo, IPayOSService payos, ILogger<PaymentService> logger)
        {
            _repo = repo;
            _payos = payos;
            _logger = logger;
        }
        public async Task<PayOSResponse> CreatePaymentAsync(PayOSRequest dto)
        {
            try
            {
                // Nếu OrderCode chưa có thì tự sinh mã ngẫu nhiên (bắt buộc là số)
                if (dto.OrderCode <= 0)
                    dto.OrderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // Lưu bản ghi thanh toán trước (Pending)
                var payment = new Payment
                {
                    PaymentId = Guid.NewGuid().ToString(),
                    UserId = dto.UserId,
                    PlanId = dto.PlanId,
                    OrderCode = dto.OrderCode.ToString(),
                    Amount = dto.Amount,
                    Description = dto.Description,
                    Status = "Pending",
                    Method = "PayOS",
                    CreatedAt = DateTime.UtcNow
                };

                await _repo.CreateAsync(payment);

                // Gọi sang PayOS tạo liên kết thanh toán
                var result = await _payos.CreatePaymentAsync(dto);
                payment.PaymentUrl = result.PaymentLink;

                await _repo.UpdateAsync(payment);

                _logger.LogInformation("Tạo thanh toán thành công: OrderCode={0}, Url={1}", dto.OrderCode, result.PaymentLink);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo thanh toán PayOS");
                throw;
            }
        }

        public async Task<bool> HandleWebhookAsync(JsonElement payload, string signature)
        {
            try
            {
                string raw = payload.GetRawText();

                // ✅ Kiểm tra chữ ký
                if (string.IsNullOrWhiteSpace(signature))
                {
                    _logger.LogWarning("Webhook thiếu signature, bỏ qua.");
                    return false;
                }

                if (!_payos.VerifyChecksum(raw, signature))
                {
                    _logger.LogWarning("Checksum không hợp lệ!");
                    return false;
                }

                // ✅ Lấy thông tin cơ bản từ payload
                var data = payload.GetProperty("data");
                long orderCode = data.GetProperty("orderCode").GetInt64();

                // Một số webhook PayOS không gửi “status”, nên ta fallback theo “code”
                string status = data.TryGetProperty("status", out var st)
                    ? st.GetString() ?? ""
                    : data.TryGetProperty("code", out var cd)
                        ? cd.GetString() ?? ""
                        : "";

                string transactionId = data.TryGetProperty("transactionId", out var tx)
                    ? tx.GetString() ?? ""
                    : "";

                _logger.LogInformation("Nhận webhook orderCode={0}, status={1}", orderCode, status);

                // ✅ Tìm bản ghi payment trong DB theo orderCode
                var payment = await _repo.GetByOrderCodeAsync(orderCode.ToString());
                if (payment == null)
                {
                    _logger.LogWarning("Không tìm thấy đơn hàng có orderCode={0}", orderCode);
                    return false;
                }

                payment.Checksum = signature;

                //  Cập nhật trạng thái thanh toán
                if (status.Equals("PAID", StringComparison.OrdinalIgnoreCase) ||
                    status.Equals("00", StringComparison.OrdinalIgnoreCase))
                {
                    payment.Status = "Paid";
                    payment.TransactionId = transactionId;
                    payment.PaidAt = DateTime.UtcNow;
                }
                else if (status.Equals("CANCELED", StringComparison.OrdinalIgnoreCase))
                {
                    payment.Status = "Canceled";
                }
                else
                {
                    payment.Status = "Failed";
                }

                await _repo.UpdateAsync(payment);

                _logger.LogInformation("Cập nhật trạng thái thanh toán thành công: {0} → {1}", orderCode, payment.Status);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý webhook PayOS");
                return false;
            }
        }
    }
    }
  
 

