using BioLinker.DTO.TemplateDTO;
using BioLinker.Enities;
using BioLinker.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controller.Collections
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionController : ControllerBase
    {
        private readonly ICollectionService _service;

        public CollectionController(ICollectionService service)
        {
            _service = service;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserCollections(string userId)
        {
            var collection = await _service.GetCollectionByUserAsync(userId);

            if (collection == null)
            {
                return Ok(new
                {
                    collectionId = (string?)null,
                    userId = userId,
                    templates = new List<string>() // mảng trống
                });
            }


            return Ok(new
            {
                collectionId = collection.CollectionId,
                userId = collection.UserId,
                templates = collection.TemplateIds,
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddCollection([FromBody] CollectionRequest dto)
        {
            if (string.IsNullOrEmpty(dto.UserId) || dto.TemplateIds == null || !dto.TemplateIds.Any())
                return BadRequest("UserId và TemplateIds không được để trống.");

            var collection = await _service.AddOrUpdateCollectionAsync(dto.UserId, dto.TemplateIds);

            return Ok(new
            {
                message = "Tạo hoặc cập nhật collection thành công.",
                collectionId = collection.CollectionId,
                userId = collection.UserId,
                templates = collection.TemplateIds
            });
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveCollection(string userId, [FromBody] List<string> templateIds)
        {
            if (templateIds == null || !templateIds.Any())
                return BadRequest("Danh sách template cần xóa không được rỗng.");

            await _service.RemoveTemplatesAsync(userId, templateIds);
            return Ok(new { message = "Đã xóa template khỏi collection." });
        }
    }
}
