using BioLinker.Enities;

namespace BioLinker.Respository.TemplateRepo
{
    public interface ITemplateDetailRepository
    {
        Task<List<TemplateDetail>> GetByTemplateIdAsync(string templateId);
        Task<TemplateDetail?> GetByIdAsync(string id);
        Task<TemplateDetail> AddAsync(TemplateDetail detail);
        Task<TemplateDetail?> UpdateAsync(TemplateDetail detail);
        Task<bool> DeleteAsync(string id);
    }
}
