using BioLinker.DTO;
using BioLinker.Enities;
using BioLinker.Respository.TemplateRepo;

namespace BioLinker.Service
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository _repo;

        public TemplateService(ITemplateRepository repo)
        {
            _repo = repo;
        }
        public async Task<TemplateDTO> CreateAsync(CreateTemplate dto)
        {
            var template = new Template
            {
                TemplateId = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                IsPremium = dto.IsPremium,
                CreatedBy = dto.CreatedBy,
                Status = "Active"
            };
            await _repo.AddAsync(template);
            return new TemplateDTO
            {
                TemplateId = template.TemplateId,
                Name = template.Name,
                Description = template.Description,
                Category = template.Category,
                IsPremium = template.IsPremium,
                Status = template.Status
            };
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var template = await _repo.GetByIdAsync(id);
            if (template == null) return false;
            await _repo.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<TemplateDTO>> GetAllAsync()
        {
            var templates = await _repo.GetAllAsync();
            return templates.Select(t => new TemplateDTO
            {
                TemplateId = t.TemplateId,
                Name = t.Name,
                Description = t.Description,
                Category = t.Category,
                IsPremium = t.IsPremium,
                Status = t.Status
            });
        }

        public async Task<TemplateDTO?> GetByIdAsync(string id)
        {
            var t = await _repo.GetByIdAsync(id);
            if (t == null) return null;
            return new TemplateDTO
            {
                TemplateId = t.TemplateId,
                Name = t.Name,
                Description = t.Description,
                Category = t.Category,
                IsPremium = t.IsPremium,
                Status = t.Status
            };
        }

        public async Task<TemplateDTO?> UpdateAsync(UpdateTemplate dto)
        {
            var template = await _repo.GetByIdAsync(dto.TemplateId);
            if (template == null) return null;

            template.Name = dto.Name;
            template.Description = dto.Description;
            template.Category = dto.Category;
            template.IsPremium = dto.IsPremium;
            template.Status = dto.Status ?? template.Status;

            await _repo.UpdateAsync(template);
            return new TemplateDTO
            {
                TemplateId = template.TemplateId,
                Name = template.Name,
                Description = template.Description,
                Category = template.Category,
                IsPremium = template.IsPremium,
                Status = template.Status
            };
        }
    }
}
