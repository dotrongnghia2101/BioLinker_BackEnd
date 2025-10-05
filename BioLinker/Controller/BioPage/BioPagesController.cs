using BioLinker.DTO;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.BioPage
{
    [Route("api/[controller]")]
    [ApiController]
    public class BioPagesController : ControllerBase
    {

        private readonly IBioPageService _service;
        public BioPagesController(IBioPageService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var bios = await _service.GetAllAsync();
            return Ok(bios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var bio = await _service.GetByIdAsync(id);
            if (bio == null) return NotFound();
            return Ok(bio);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBioPage dto)
        {
            var bio = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = bio.BioPageId }, bio);
        }

        [HttpPost("from-template/{templateId}")]
        public async Task<IActionResult> CreateFromTemplate(string templateId, [FromBody] CreateBioPageFromTemplate dto)
        {
            var bio = await _service.CreateFromTemplateAsync(templateId, dto);
            if (bio == null) return NotFound(new { message = "Template not found" });
            return CreatedAtAction(nameof(GetById), new { id = bio.BioPageId }, bio);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateBioPage dto)
        {
            var bio = await _service.UpdateAsync(id, dto);
            if (bio == null) return NotFound();
            return Ok(bio);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id}/full")]
        public async Task<IActionResult> UpdateFull(string id, [FromBody] UpdateFullBioPage dto)
        {
            var bio = await _service.UpdateFullAsync(id, dto);
            if (bio == null) return NotFound();
            return Ok(bio);
        }
    }
}
