using BioLinker.DTO;

namespace BioLinker.Service
{
    public interface ITemplateDetailService
    {
        Task<IEnumerable<TemplateDetailResponse>> GetByTemplateIdAsync(string templateId);
        Task<TemplateDetailResponse?> GetByIdAsync(string id);
        Task<TemplateDetailResponse> CreateAsync(CreateTemplateDetail dto);
        Task<TemplateDetailResponse?> UpdateAsync(UpdateTemplateDetail dto);
        Task<bool> DeleteAsync(string id);
    }
}
