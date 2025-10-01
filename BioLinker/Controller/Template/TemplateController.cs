using BioLinker.DTO;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.Template
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private readonly ITemplateService _service;
        public TemplateController(ITemplateService service)
        {
            _service = service;
        }

        //get all template
        //api/templates
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var templates = await _service.GetAllAsync();
            return Ok(templates);
        }

        //get theo id
        // GET: api/templates/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var template = await _service.GetByIdAsync(id);
            if (template == null) return NotFound();
            return Ok(template);
        }

        //tao template moi
        // POST: api/templates
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTemplate dto)
        {
            var template = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = template.TemplateId }, template);
        }

        // PUT: api/templates/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTemplate dto)
        {
            if (id != dto.TemplateId)
                return BadRequest("TemplateId mismatch");

            var result = await _service.UpdateAsync(dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // DELETE: api/templates/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
