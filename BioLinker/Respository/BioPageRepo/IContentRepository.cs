using BioLinker.Enities;

namespace BioLinker.Respository.BioPageRepo
{
    public interface IContentRepository
    {
        Task<Content?> GetByIdAsync(string id);
        Task<List<Content>> GetByBioPageAsync(string bioPageId);
        Task<Content> AddAsync(Content content);
        Task<Content> UpdateAsync(Content content);
        Task<bool> DeleteAsync(string id);
    }
}

