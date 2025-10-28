using BioLinker.DTO.PaymentDTO;
using BioLinker.Respository.PaymentRepo;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BioLinker.Controller.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayOSController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PayOSController> _logger;

        public PayOSController(
            IPaymentService paymentService,
            IPaymentRepository paymentRepository,
            ILogger<PayOSController> logger)
        {
            _paymentService = paymentService;
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PayOSRequest dto)
        {
            try
            {
                var result = await _paymentService.CreatePaymentAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi tạo thanh toán PayOS");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> PayOSWebhook([FromBody] JsonElement? body)
        {
            try
            {
                // Trường hợp body rỗng (PayOS ping test hoặc người test trên Swagger mà không nhập gì)
                if (body == null || body.Value.ValueKind == JsonValueKind.Undefined)
                {
                    _logger.LogInformation("Webhook nhận ping test (body rỗng)");
                    return Ok(new { code = "00", message = "Webhook alive" });
                }

                // Lấy nội dung JSON gốc
                string jsonBody = body.Value.GetRawText();

                _logger.LogInformation("Webhook nhận dữ liệu: {body}", jsonBody);

                // Gọi xử lý nghiệp vụ
                bool ok = await _paymentService.HandleWebhookAsync(jsonBody);

                if (ok)
                {
                    _logger.LogInformation("✅ Webhook xử lý thành công");
                    return Ok(new { code = "00", message = "Webhook xử lý thành công" });
                }

                _logger.LogWarning("❌ Webhook xử lý thất bại");
                return BadRequest(new { code = "01", message = "Webhook xử lý thất bại" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Lỗi khi xử lý webhook PayOS");
                return StatusCode(500, new { code = "99", message = "Lỗi hệ thống", error = ex.Message });
            }
        }

        [HttpGet("cancel")]
        public async Task<IActionResult> PaymentCancel([FromQuery] long orderCode, [FromQuery] string userId)
        {
            try
            {
                var payment = await _paymentRepository.GetByOrderCodeAsync(orderCode.ToString());
                if (payment == null)
                {
                    _logger.LogWarning("⚠️ Khong tim thay giao dich voi orderCode {OrderCode}", orderCode);
                    return NotFound(new { message = "Khong tim thay giao dich" });
                }

                // cap nhat trang thai Cancelled
                payment.Status = "Cancelled";
                payment.PaidAt = null;
                await _paymentRepository.UpdateAsync(payment);

                _logger.LogInformation("🛑 Nguoi dung {UserId} huy giao dich {OrderCode}", userId, orderCode);

                // Co the tra ve trang thong bao hoac JSON tuy muc dich
                return Ok(new
                {
                    message = "Nguoi dung da huy thanh toan thanh cong",
                    orderCode,
                    userId,
                    status = "Cancelled"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Loi khi cap nhat trang thai Cancelled");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserPayments([FromQuery] string userId)
        {
            var result = await _paymentService.GetPaymentsByUserAsync(userId);
            return Ok(result);
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAllPayments()
        {
            var result = await _paymentService.GetAllPaymentsAsync();
            return Ok(result);
        }

        [HttpPost("upgrade-to-pro")]
        public async Task<IActionResult> UpgradeToPro([FromBody] UpgradePlanRequest request)
        {
            try
            {
                bool ok = await _paymentService.UpgradeToProPlanAsync(request.UserId);
                if (ok)
                    return Ok(new { message = "Đã nâng cấp người dùng lên ProUser trong 1 tháng" });

                return BadRequest(new { message = "Không thể nâng cấp người dùng" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi nâng cấp user {UserId} lên Pro", request.UserId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
