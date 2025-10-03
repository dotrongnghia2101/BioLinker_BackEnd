using BioLinker.DTO;
using BioLinker.Enities;
using BioLinker.Respository.BioPageRepo;

namespace BioLinker.Service
{
    public class StyleSettingsService : IStyleSettingsService
    {
        private readonly IStyleSettingsRepository _repo;

        public StyleSettingsService(IStyleSettingsRepository repo)
        {
            _repo = repo;
        }

        public async Task<StyleSettingsResponse> CreateAsync(CreateStyleSettings dto)
        {
            var entity = new StyleSettings
            {
                BioPageId = dto.BioPageId,
                Thumbnail = dto.Thumbnail,
                MetaTitle = dto.MetaTitle,
                MetaDescription = dto.MetaDescription,
                CookieBanner = dto.CookieBanner
            };
            var result = await _repo.CreateAsync(entity);
            return MapToDto(result);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            return await _repo.DeleteAsync(id);
        }

        public async Task<StyleSettingsResponse?> GetByBioPageIdAsync(string bioPageId)
        {
            var entity = await _repo.GetByBioPageIdAsync(bioPageId);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<StyleSettingsResponse?> GetByIdAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<StyleSettingsResponse?> UpdateAsync(string id, UpdateStyleSettings dto)
        {
            var entity = new StyleSettings
            {
                Thumbnail = dto.Thumbnail,
                MetaTitle = dto.MetaTitle,
                MetaDescription = dto.MetaDescription,
                CookieBanner = dto.CookieBanner
            };
            var result = await _repo.UpdateAsync(id, entity);
            return result == null ? null : MapToDto(result);
        }

        private StyleSettingsResponse MapToDto(StyleSettings entity)
        {
            return new StyleSettingsResponse
            {
                StyleSettingsId = entity.StyleSettingsId,
                BioPageId = entity.BioPageId,
                Thumbnail = entity.Thumbnail,
                MetaTitle = entity.MetaTitle,
                MetaDescription = entity.MetaDescription,
                CookieBanner = entity.CookieBanner
            };
        }
    }
}
