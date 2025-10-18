using BioLinker.DTO.LinkDTO;
using BioLinker.Enities;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.Links
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkController : ControllerBase
    {
        private readonly ILinkService _service;

        public LinkController(ILinkService service)
        {
            _service = service;
        }

        [HttpGet("biopage/{bioPageId}")]
        public async Task<ActionResult<IEnumerable<LinkResponse>>> GetAllByBioPage(string bioPageId)
        {
            var result = await _service.GetAllByBioPageAsync(bioPageId);
            return Ok(result);
        }

        [HttpGet("{linkId}")]
        public async Task<ActionResult<LinkResponse>> GetById(string linkId)
        {
            var result = await _service.GetByIdAsync(linkId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<LinkResponse>> Create([FromBody] LinkCreate dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { linkId = created.LinkId }, created);
        }

        [HttpPut("{linkId}")]
        public async Task<IActionResult> Update(string linkId, [FromBody] Link link)
        {
            if (linkId != link.LinkId) return BadRequest("Link ID mismatch");
            await _service.UpdateAsync(link);
            return Ok(new { message = "Link updated successfully" });
        }

        [HttpDelete("{linkId}")]
        public async Task<IActionResult> Delete(string linkId)
        {
            await _service.DeleteAsync(linkId);
            return Ok(new { message = "Link deleted successfully" });
        }
    }
}
