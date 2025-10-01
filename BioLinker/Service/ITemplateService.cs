using BioLinker.DTO;
using BioLinker.Enities;

namespace BioLinker.Service
{
    public interface ITemplateService
    {
        Task<IEnumerable<TemplateDTO>> GetAllAsync();
        Task<TemplateDTO?> GetByIdAsync(string id);
        Task<TemplateDTO> CreateAsync(CreateTemplate dto);
        Task<TemplateDTO?> UpdateAsync(UpdateTemplate dto);
        Task<bool> DeleteAsync(string id);
    }
}
