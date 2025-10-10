using BioLinker.DTO.TemplateDTO;
using BioLinker.Enities;

namespace BioLinker.Service
{
    public interface ITemplateService
    {
        Task<IEnumerable<TemplateResponse>> GetAllAsync();
        Task<TemplateResponse?> GetByIdAsync(string id);
        Task<TemplateResponse> CreateAsync(CreateTemplate dto);
        Task<TemplateResponse?> UpdateAsync(UpdateTemplate dto);
        Task<bool> DeleteAsync(string id);
    }
}
