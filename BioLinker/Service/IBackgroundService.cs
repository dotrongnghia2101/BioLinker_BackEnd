using BioLinker.DTO.BioDTO;

namespace BioLinker.Service
{
    public interface IBackgroundService
    {
        Task<BackgroundResponse?> GetByIdAsync(string id);
        //Task<BackgroundResponse?> GetByBioPageIdAsync(string bioPageId);
        Task<BackgroundResponse> CreateAsync(BackgroundCreate dto);
        Task<BackgroundResponse?> UpdateAsync(string id, BackgroundUpdate dto);
        Task<bool> DeleteAsync(string id);
    }
}
