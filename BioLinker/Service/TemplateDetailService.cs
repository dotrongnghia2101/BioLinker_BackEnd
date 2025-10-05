using BioLinker.DTO;
using BioLinker.Enities;
using BioLinker.Respository.TemplateRepo;

namespace BioLinker.Service
{
    public class TemplateDetailService : ITemplateDetailService
    {
        private readonly ITemplateDetailRepository _repo;

        public TemplateDetailService(ITemplateDetailRepository repo)
        {
            _repo = repo;
        }

        public async Task<TemplateDetailResponse> CreateAsync(CreateTemplateDetail dto)
        {
            var entity = new TemplateDetail
            {
                TemplateId = dto.TemplateId,
                ElementType = dto.ElementType,
                Position = dto.Position,
                Size = dto.Size,
                Style = dto.Style,
                Element = dto.Element,
                OrderIndex = dto.OrderIndex,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity);
            return MapToResponse(entity);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<TemplateDetailResponse?> GetByIdAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : MapToResponse(entity);
        }

        public async Task<IEnumerable<TemplateDetailResponse>> GetByTemplateIdAsync(string templateId)
        {
            var list = await _repo.GetByTemplateIdAsync(templateId);
            return list.Select(MapToResponse);
        }

        public async Task<TemplateDetailResponse?> UpdateAsync(UpdateTemplateDetail dto)
        {
            var entity = await _repo.GetByIdAsync(dto.TemplateDetailId!);
            if (entity == null) return null;

            entity.ElementType = dto.ElementType ?? entity.ElementType;
            entity.Position = dto.Position ?? entity.Position;
            entity.Size = dto.Size ?? entity.Size;
            entity.Style = dto.Style ?? entity.Style;
            entity.Element = dto.Element ?? entity.Element;
            entity.OrderIndex = dto.OrderIndex ?? entity.OrderIndex;

            var updated = await _repo.UpdateAsync(entity);
            return updated == null ? null : MapToResponse(updated);
        }

        private static TemplateDetailResponse MapToResponse(TemplateDetail td)
        {
            return new TemplateDetailResponse
            {
                TemplateDetailId = td.TemplateDetailId,
                TemplateId = td.TemplateId,
                ElementType = td.ElementType,
                Position = td.Position,
                Size = td.Size,
                Style = td.Style,
                Element = td.Element,
                OrderIndex = td.OrderIndex,
                CreatedAt = td.CreatedAt
            };
        }
    }
}
