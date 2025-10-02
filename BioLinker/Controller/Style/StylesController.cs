using BioLinker.DTO;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.Style
{
    [Route("api/[controller]")]
    [ApiController]
    public class StylesController : ControllerBase
    {
        private readonly IStyleService _service;

        public StylesController(IStyleService service)
        {
            _service = service;
        }

        // GET api/styles
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var styles = await _service.GetAllAsync();
            return Ok(styles);
        }

        // GET api/styles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var style = await _service.GetByIdAsync(id);
            if (style == null) return NotFound();
            return Ok(style);
        }

        // POST api/styles
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStyle dto)
        {
            var style = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = style.StyleId }, style);
        }

        // PATCH api/styles/{id} // update 1 phan hoac toan bo
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateStyle dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        // DELETE api/styles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
