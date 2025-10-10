using BioLinker.DTO.BioDTO;

namespace BioLinker.Service
{
    public interface IStyleService
    {
        Task<IEnumerable<StyleResponse>> GetAllAsync();
        Task<StyleResponse?> GetByIdAsync(string id);
        Task<StyleResponse> CreateAsync(CreateStyle dto);
        Task<bool> UpdateAsync(string id, UpdateStyle dto);
        Task<bool> DeleteAsync(string id);
    }
}
