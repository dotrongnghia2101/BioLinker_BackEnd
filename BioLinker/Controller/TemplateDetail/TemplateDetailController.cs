using BioLinker.DTO.TemplateDTO;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.TemplateDetail
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateDetailController : ControllerBase
    {
        private readonly ITemplateDetailService _service;

        public TemplateDetailController(ITemplateDetailService service)
        {
            _service = service;
        }

        [HttpGet("template/{templateId}")]
        public async Task<IActionResult> GetByTemplateId(string templateId)
        {
            var result = await _service.GetByTemplateIdAsync(templateId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] List<CreateTemplateDetail> dtos)
        {
            var results = new List<TemplateDetailResponse>();

            foreach (var dto in dtos)
            {
                var result = await _service.CreateAsync(dto);
                results.Add(result);
            }

            return Ok(results);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTemplateDetail dto)
        {
            dto.TemplateDetailId = id;
            var result = await _service.UpdateAsync(dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
