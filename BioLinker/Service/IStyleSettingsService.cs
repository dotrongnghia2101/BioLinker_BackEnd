using BioLinker.DTO;

namespace BioLinker.Service
{
    public interface IStyleSettingsService
    {
        Task<StyleSettingsResponse?> GetByIdAsync(string id);
        Task<StyleSettingsResponse?> GetByBioPageIdAsync(string bioPageId);
        Task<StyleSettingsResponse> CreateAsync(CreateStyleSettings dto);
        Task<StyleSettingsResponse?> UpdateAsync(string id, UpdateStyleSettings dto);
        Task<bool> DeleteAsync(string id);
    }
}
