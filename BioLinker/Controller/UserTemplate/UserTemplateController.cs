using BioLinker.DTO.UserDTO;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.UserTemplate
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTemplateController : ControllerBase
    {
        private readonly IUserTemplateService _service;

        public UserTemplateController(IUserTemplateService service)
        {
            _service = service;
        }

        // mua template
        [HttpPost("purchase")]
        public async Task<IActionResult> PurchaseTemplate([FromBody] UserTemplateCreate dto)
        {
            try
            {
                var result = await _service.PurchaseTemplateAsync(dto);
                if (result == null)
                    return BadRequest(new { message = "Invalid data." });

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // lấy danh sách template user đã mua
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserTemplates(string userId)
        {
            var result = await _service.GetPurchasedTemplatesByUserAsync(userId);
            return Ok(result);
        }
    }
}
