using BioLinker.DTO.BioDTO;
using BioLinker.Enities;

namespace BioLinker.Service
{
    public interface IContentService
    {
        Task<Content?> GetContentById(string id);
        Task<List<Content>> GetContentsByBioPage(string bioPageId);
        Task<Content> CreateContent(CreateContent dto);
        Task<Content?> UpdateContent(string id, UpdateContent dto);
        Task<bool> DeleteContent(string id);
    }
}
