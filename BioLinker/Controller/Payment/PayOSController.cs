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
        public async Task<IActionResult> PayOSWebhook([FromBody] JsonElement body)
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(body))
                return Ok(new { code = "00", message = "Webhook alive" });

            var ok = await _paymentService.HandleWebhookAsync(body);
            return ok
                ? Ok(new { code = "00", message = "Webhook xử lý thành công" })
                : BadRequest(new { code = "01", message = "Webhook xử lý thất bại" });
        }

    }
}
