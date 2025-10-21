using BioLinker.DTO.PaymentDTO;
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
        private readonly ILogger<PayOSController> _logger;

        public PayOSController(IPaymentService paymentService, ILogger<PayOSController> logger)
        {
            _paymentService = paymentService;
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

    }
}
