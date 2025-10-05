using BioLinker.Enities;

namespace BioLinker.Respository.BioPageRepo
{
    public interface IStyleSettingsRepository
    {
        Task<StyleSettings?> GetByIdAsync(string id);
        Task<StyleSettings> CreateAsync(StyleSettings settings);
        Task<StyleSettings?> UpdateAsync(string id, StyleSettings settings);
        Task<bool> DeleteAsync(string id);
    }
}
