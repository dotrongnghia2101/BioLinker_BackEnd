using BioLinker.DTO.CountDTO;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.CountBioClicked
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountBioClickedController : ControllerBase
    {
        private readonly ICountBioClickedService _service;

        public CountBioClickedController(ICountBioClickedService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpGet("date")]
        public async Task<IActionResult> GetByDate(DateTime date) => Ok(await _service.GetByDateAsync(date));

        [HttpGet("week")]
        public async Task<IActionResult> GetByWeek(int week, int year) => Ok(await _service.GetByWeekAsync(week, year));

        [HttpGet("month")]
        public async Task<IActionResult> GetByMonth(int month, int year) => Ok(await _service.GetByMonthAsync(month, year));

        [HttpGet("total")]
        public async Task<IActionResult> GetTotalCount() => Ok(await _service.GetTotalCountAsync());

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CountBioClickCreate dto)
        {
            if (dto == null)
                return BadRequest("Invalid");

            var created = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.CountBioClickedId }, created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return Ok("Deleted");
        }
    }
}
