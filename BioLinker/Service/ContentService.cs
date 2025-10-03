using BioLinker.DTO;
using BioLinker.Enities;
using BioLinker.Respository.BioPageRepo;

namespace BioLinker.Service
{
    public class ContentService : IContentService
    {

        private readonly IContentRepository _repo;

        public ContentService(IContentRepository repo)
        {
            _repo = repo;
        }


        public async Task<Content> CreateContent(CreateContent dto)
        {
            var content = new Content
            {
                BioPageId = dto.BioPageId,
                ElementType = dto.ElementType,
                Alignment = dto.Alignment,
                Visible = dto.Visible,
                CreatedAt = DateTime.UtcNow,
                Position = dto.Position,
                Size = dto.Size,
                Style = dto.Style,
                Element = dto.Element
            };

            return await _repo.AddAsync(content);
        }

        public async Task<bool> DeleteContent(string id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<Content?> GetContentById(string id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<List<Content>> GetContentsByBioPage(string bioPageId)
        {
            return await _repo.GetByBioPageAsync(bioPageId);
        }

        public async Task<Content?> UpdateContent(string id, UpdateContent dto)
        {
            var content = await _repo.GetByIdAsync(id);
            if (content == null) return null;

            if (!string.IsNullOrEmpty(dto.ElementType)) content.ElementType = dto.ElementType;
            if (!string.IsNullOrEmpty(dto.Alignment)) content.Alignment = dto.Alignment;
            if (dto.Visible.HasValue) content.Visible = dto.Visible.Value;
            if (dto.Position != null) content.Position = dto.Position;
            if (dto.Size != null) content.Size = dto.Size;
            if (dto.Style != null) content.Style = dto.Style;
            if (dto.Element != null) content.Element = dto.Element;

            content.UpdatedAt = DateTime.UtcNow;

            return await _repo.UpdateAsync(content);
        }
    }
}
