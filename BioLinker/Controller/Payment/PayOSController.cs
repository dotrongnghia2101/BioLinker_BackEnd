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
        public async Task<IActionResult> Webhook([FromBody] JsonElement payload, [FromHeader(Name = "x-checksum")] string checksum)
        {
            bool success = await _paymentService.HandleWebhookAsync(payload, checksum);
            if (!success) return BadRequest("Invalid checksum");
            return Ok(new { message = "Webhook processed successfully" });
        }


    }
}
