using BioLinker.DTO.BioDTO;

namespace BioLinker.Service
{
    public interface IBioPageService
    {
        Task<BioPageResponse?> GetByIdAsync(string id);
        Task<IEnumerable<BioPageResponse>> GetAllAsync();
        Task<BioPageResponse> CreateAsync(CreateBioPage dto);
        Task<BioPageResponse?> UpdateAsync(string id, UpdateBioPage dto);
        Task<bool> DeleteAsync(string id);
        Task<BioPageResponse?> UpdateFullAsync(string id, UpdateFullBioPage dto);
        Task<BioPageResponse?> CreateFromTemplateAsync(string templateId, CreateBioPageFromTemplate dto);
    }
}
