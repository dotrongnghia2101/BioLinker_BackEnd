using BioLinker.DTO.TemplateDTO;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.MarketPlace
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketplaceController : ControllerBase
    {
        private readonly IMarketplaceService _service;
        public MarketplaceController(IMarketplaceService service)
        {
            _service = service;
        }

        [HttpPost("sell")]
        public async Task<IActionResult> SellTemplate([FromBody] MarketplaceCreate dto)
        {
            try
            {
                var result = await _service.SellTemplateAsync(dto);
                if (result == null)
                    return BadRequest(new { message = "Invalid Data" });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("user/{sellerId}")]
        public async Task<IActionResult> GetBySeller(string sellerId)
        {
            var result = await _service.GetBySellerAsync(sellerId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, [FromQuery] string sellerId)
        {
            var success = await _service.RemoveAsync(id, sellerId);
            if (!success)
                return NotFound(new { message = "Not found or not belong to this user." });
            return NoContent();
        }
    }
}
