using BioLinker.DTO.LinkDTO;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.StaticLink
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaticLinksController : ControllerBase
    {
        private readonly IStaticLinkService _service;
        public StaticLinksController(IStaticLinkService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StaticLinkResponse>> GetById(string id)
        {
            var data = await _service.GetAsync(id);
            if (data == null) 
            { 
                return NotFound(new { message = "Khong tim thay static link." });
            }

            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult<StaticLinkResponse>> Create([FromBody] StaticLinkCreate dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.UserId))
                return BadRequest(new { message = "Du lieu khong hop le." });

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.StaticLinkId }, created);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<StaticLinkResponse>>> GetByUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new { message = "userId khong duoc rong." });

            var list = await _service.GetByUserAsync(userId);
            return Ok(list);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromQuery] string userId, [FromBody] StaticLinkUpdate dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
            { 
                return BadRequest(new { message = "userId khong duoc rong." });
            }
            var result = await _service.UpdateAsync(id, userId, dto);
            if (!result)
            {
                return NotFound(new { message = "Cap nhat that bai. Khong ton tai hoac khong thuoc user nay." });
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new { message = "userId khong duoc rong." });

            var result = await _service.DeleteAsync(id, userId);
            if (!result)
                return NotFound(new { message = "Xoa that bai. Khong ton tai hoac khong thuoc user nay." });

            return NoContent();
        }
    }
}
