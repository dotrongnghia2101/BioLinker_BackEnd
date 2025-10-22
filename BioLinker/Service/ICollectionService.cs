using BioLinker.DTO.TemplateDTO;
using BioLinker.Enities;

namespace BioLinker.Service
{
    public interface ICollectionService
    {
        Task<Collection> AddOrUpdateCollectionAsync(string userId, List<string> templateIds);
        Task<Collection?> GetCollectionByUserAsync(string userId);
        Task RemoveTemplatesAsync(string userId, List<string> templateIds);
    }
}
