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
                _logger.LogError(ex, "Loi tao thanh toan PayOS");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> PayOSWebhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                // ✅ Nếu PayOS test ping (body rỗng hoặc không có data) thì phản hồi OK luôn
                if (string.IsNullOrWhiteSpace(body))
                {
                    _logger.LogWarning("Webhook nhận request kiểm tra từ PayOS (ping), trả về 200 OK.");
                    return Ok(new { code = "00", message = "Webhook alive" });
                }

                var payload = JsonDocument.Parse(body).RootElement;

                // ✅ Nếu không có trường "data" thì cũng coi là ping test
                if (!payload.TryGetProperty("data", out _))
                {
                    _logger.LogWarning("Webhook nhận ping test (không có trường data), trả về 200 OK.");
                    return Ok(new { code = "00", message = "Webhook alive" });
                }

                string checksumHeader = Request.Headers["x-checksum"]!;
                _logger.LogInformation("Webhook nhận dữ liệu thực tế: {payload}", body);

                bool ok = await _paymentService.HandleWebhookAsync(payload, checksumHeader);

                if (ok)
                {
                    _logger.LogInformation("Xử lý webhook thành công.");
                    return Ok(new { code = "00", message = "Webhook xử lý thành công" });
                }

                _logger.LogWarning("Webhook thất bại, checksum hoặc orderCode không hợp lệ.");
                return BadRequest(new { code = "01", message = "Webhook xử lý thất bại" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý webhook PayOS");
                return StatusCode(500, new { code = "99", message = "Lỗi hệ thống", error = ex.Message });
            }
        }


    }
}
