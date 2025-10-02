using BioLinker.DTO;
using BioLinker.Enities;
using BioLinker.Helper;
using BioLinker.Respository.BioPageRepo;

namespace BioLinker.Service
{
    public class BackgroundService : IBackgroundService
    {
        private readonly IBackgroundRepository _repo;
        public BackgroundService(IBackgroundRepository repo)
        {
            _repo = repo;
        }

        // tao moi background
        public async Task<BackgroundResponse> CreateAsync(BackgroundCreate dto)
        {
            //check bioid co null hay khong
            if (string.IsNullOrEmpty(dto.BioPageId))
                throw new ArgumentException("BioPageId required");

            //kiem tra type cua background la gi
            if (!BackgroundTypes.All.Contains(dto.Type ?? ""))
                throw new ArgumentException("Invalid background type");

            // tao moi background
            var bg = new Background
            {
                BioPageId = dto.BioPageId,
                Type = dto.Type,
                Value = dto.Value,
                CreatedAt = DateTime.UtcNow,
            };

            await _repo.AddAsync(bg);
            return MapToDto(bg);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var bg = await _repo.GetByIdAsync(id);
            if (bg == null) return false;

            await _repo.DeleteAsync(bg);
            return true;
        }

        // lay background theo bioPageId
        public async Task<BackgroundResponse?> GetByBioPageIdAsync(string bioPageId)
        {
            var bg = await _repo.GetByBioPageIdAsync(bioPageId);
            return bg == null ? null : MapToDto(bg);
        }

        // lay background theo id
        public async Task<BackgroundResponse?> GetByIdAsync(string id)
        {
            var bg = await _repo.GetByIdAsync(id);
            return bg == null ? null : MapToDto(bg);
        }

        // update background
        public async Task<BackgroundResponse?> UpdateAsync(string id, BackgroundUpdate dto)
        {
            var bg = await _repo.GetByIdAsync(id);
            if (bg == null) return null;

            if (!string.IsNullOrEmpty(dto.Type) && !BackgroundTypes.All.Contains(dto.Type))
                throw new ArgumentException("Invalid background type");

            bg.Type = dto.Type ?? bg.Type;
            bg.Value = dto.Value ?? bg.Value;
            bg.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(bg);
            return MapToDto(bg);
        }

        // map sang dto tra ve fe
        private static BackgroundResponse MapToDto(Background bg)
        {
            return new BackgroundResponse
            {
                BackgroundId = bg.BackgroundId,
                BioPageId = bg.BioPageId,
                Type = bg.Type,
                Value = bg.Value
            };
        }
    }
}
