using BioLinker.DTO.BioDTO;
using BioLinker.Enities;
using BioLinker.Helper;
using BioLinker.Respository.BioPageRepo;

namespace BioLinker.Service
    {
        public class BaseBackgroundService : IBackgroundService
        {
            private readonly IBackgroundRepository _repo;
            public BaseBackgroundService(IBackgroundRepository repo)
            {
                _repo = repo;
            }

            // tao moi background
            public async Task<BackgroundResponse> CreateAsync(BackgroundCreate dto)
            {
                if (string.IsNullOrEmpty(dto.Type))
                    throw new ArgumentException("Background type is required.");

                if (!BackgroundTypes.All.Contains(dto.Type))
                    throw new ArgumentException("Invalid background type.");

                // tao moi
                var bg = new Background
                {
                    BackgroundId = Guid.NewGuid().ToString(),
                    Type = dto.Type,
                    Value = dto.Value,
                    CreatedAt = DateTime.UtcNow
                };

                await _repo.AddAsync(bg);
                return MapToDto(bg);
            }

            public async Task<bool> DeleteAsync(string id)
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentException("Background ID is required.");

                var bg = await _repo.GetByIdAsync(id);
                if (bg == null) return false;

                await _repo.DeleteAsync(bg);
                return true;
            }

            // lay background theo bioPageId
            //public async Task<BackgroundResponse?> GetByBioPageIdAsync(string bioPageId)
            //{
            //    var bg = await _repo.GetByBioPageIdAsync(bioPageId);
            //    return bg == null ? null : MapToDto(bg);
            //}

            // lay background theo id
            public async Task<BackgroundResponse?> GetByIdAsync(string id)
            {
                var bg = await _repo.GetByIdAsync(id);
                return bg == null ? null : MapToDto(bg);
            }

            // update background
            public async Task<BackgroundResponse?> UpdateAsync(string id, BackgroundUpdate dto)
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentException("Background ID is required.");

                var bg = await _repo.GetByIdAsync(id);
                if (bg == null) return null;

                // Validate type neu co
                if (!string.IsNullOrEmpty(dto.Type) && !BackgroundTypes.All.Contains(dto.Type))
                    throw new ArgumentException("Invalid background type.");

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
                    Type = bg.Type,
                    Value = bg.Value,
                    CreatedAt = bg.CreatedAt,
                    UpdatedAt = bg.UpdatedAt
                };
            }
        }
    }
