using BioLinker.Enities;

namespace BioLinker.Respository.TemplateRepo
{
    public interface ITemplateRepository
    {
        Task<IEnumerable<Template>> GetAllAsync();
        Task<Template?> GetByIdAsync(string id);
        Task AddAsync(Template template);
        Task UpdateAsync(Template template);
        Task DeleteAsync(string id);
    }
}
