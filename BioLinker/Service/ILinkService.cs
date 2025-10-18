using BioLinker.DTO.LinkDTO;
using BioLinker.Enities;

namespace BioLinker.Service
{
    public interface ILinkService
    {
        Task<IEnumerable<LinkResponse?>> GetAllByBioPageAsync(string bioPageId);
        Task<LinkResponse?> GetByIdAsync(string linkId);
        Task<LinkResponse?> CreateAsync(LinkCreate link);
        Task UpdateAsync(Link link);
        Task DeleteAsync(string linkId);
    }
}
