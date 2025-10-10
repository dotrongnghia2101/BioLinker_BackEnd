using BioLinker.DTO.BioDTO;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.Background
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackgroundsController : ControllerBase
    {
        private readonly IBackgroundService _service;
        public BackgroundsController(IBackgroundService service)
        {
            _service = service;
        }

        // GET api/backgrounds/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var bg = await _service.GetByIdAsync(id);
            if (bg == null) return NotFound();
            return Ok(bg);
        }


        // POST api/backgrounds
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BackgroundCreate dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.BackgroundId }, result);
        }

        // PUT api/backgrounds/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] BackgroundUpdate dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // DELETE api/backgrounds/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
