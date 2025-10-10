using BioLinker.DTO.CountDTO;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.CountTemplateClicked
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountTemplateClickedController : ControllerBase
    {

        private readonly ICountTemplateClickedService _service;

        public CountTemplateClickedController(ICountTemplateClickedService service)
        {
            _service = service;
        }

        // Lay tat ca du lieu
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        // Lay theo ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound("ID not found");
            return Ok(item);
        }

        // Lay theo ngay
        [HttpGet("date")]
        public async Task<IActionResult> GetByDate(DateTime date)
        {
            var data = await _service.GetByDateAsync(date);
            return Ok(data);
        }

        // Lay theo tuan (tham so: week, year)
        [HttpGet("week")]
        public async Task<IActionResult> GetByWeek(int week, int year)
        {
            var data = await _service.GetByWeekAsync(week, year);
            return Ok(data);
        }

        // Lay theo thang (tham so: month, year)
        [HttpGet("month")]
        public async Task<IActionResult> GetByMonth(int month, int year)
        {
            var data = await _service.GetByMonthAsync(month, year);
            return Ok(data);
        }

        // Lay tong so luot click
        [HttpGet("total")]
        public async Task<IActionResult> GetTotalCount()
        {
            var total = await _service.GetTotalCountAsync();
            return Ok(total);
        }

        // Them moi luot click
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CountTemplateClickedCreated dto)
        {
            if (dto == null)
                return BadRequest("invalid data");

            var created = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.CountTemplateClickedId }, created);
        }

        // Xoa theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return Ok("Successfully");
        }
    }
}
