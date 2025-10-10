using BioLinker.DTO.BioDTO;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.StyleSetting
{
    [Route("api/[controller]")]
    [ApiController]
    public class StyleSettingsController : ControllerBase
    {

        private readonly IStyleSettingsService _service;

        public StyleSettingsController(IStyleSettingsService service)
        {
            _service = service;
        }

        // GET api/stylesettings/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }


        // POST api/stylesettings
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStyleSettings dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.StyleSettingsId }, result);
        }

        // PATCH api/stylesettings/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateStyleSettings dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // DELETE api/stylesettings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
