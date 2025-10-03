using BioLinker.DTO;
using BioLinker.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace BioLinker.Controller.Content
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly IContentService _service;

        public ContentsController(IContentService service)
        {
            _service = service;
        }

        //lay content theo id
        // GET api/contents/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContent(string id)
        {
            var content = await _service.GetContentById(id);
            if (content == null) return NotFound();
            return Ok(content);
        }

        //lay content tu biopage
        // GET api/contents/biopage/{bioPageId}
        [HttpGet("biopage/{bioPageId}")]
        public async Task<IActionResult> GetContentsByBioPage(string bioPageId)
        {
            var contents = await _service.GetContentsByBioPage(bioPageId);
            return Ok(contents);
        }

        //tao content moi
        // POST api/contents
        [HttpPost]
        public async Task<IActionResult> CreateContent([FromBody] CreateContent dto)
        {
            var content = await _service.CreateContent(dto);
            return CreatedAtAction(nameof(GetContent), new { id = content.ContentId }, content);
        }

        //update 1 hay nhieu gia tri content
        // PATCH api/contents/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateContent(string id, [FromBody] UpdateContent dto)
        {
            var content = await _service.UpdateContent(id, dto);
            if (content == null) return NotFound();
            return Ok(content);
        }

        //xoa
        // DELETE api/contents/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(string id)
        {
            var result = await _service.DeleteContent(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
