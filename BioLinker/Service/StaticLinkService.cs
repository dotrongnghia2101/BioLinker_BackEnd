using BioLinker.DTO.LinkDTO;
using BioLinker.Enities;
using BioLinker.Respository.LinkRepo;

namespace BioLinker.Service
{
    public class StaticLinkService : IStaticLinkService
    {
        private readonly IStaticLinkRepository _repo;
        public StaticLinkService(IStaticLinkRepository repo) 
        { 
            _repo = repo; 
        }

        private static StaticLinkResponse ToDto(StaticLink x) => new StaticLinkResponse
        {
            StaticLinkId = x.StaticLinkId ?? string.Empty,
            UserId = x.UserId,
            Title = x.Title,
            Icon = x.Icon,
            Platform = x.Platform,
            DefaultUrl = x.DefaultUrl,
            Status = x.Status
        };
        public async Task<StaticLinkResponse> CreateAsync(StaticLinkCreate dto)
        {
            var entity = new StaticLink
            {
                StaticLinkId = Guid.NewGuid().ToString(),
                UserId = dto.UserId,
                Title = dto.Title,
                Icon = dto.Icon,
                Platform = dto.Platform,
                DefaultUrl = dto.DefaultUrl,
                Status = dto.Status,
            };
            await _repo.AddAsync(entity);
            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(string id, string userId)
        {
            var current = await _repo.GetByIdAsync(id);
            if (current is null) return false;
            if (!string.Equals(current.UserId, userId, StringComparison.Ordinal)) return false;

            await _repo.DeleteAsync(current);
            return true;
        }

        public async Task<StaticLinkResponse?> GetAsync(string id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity is null ? null : ToDto(entity);
        }

        public async Task<IEnumerable<StaticLinkResponse>> GetByUserAsync(string userId)
        {
            var items = await _repo.GetByUserAsync(userId);
            return items.Select(ToDto);
        }

        public async Task<bool> UpdateAsync(string id, string userId, StaticLinkUpdate dto)
        {
            var current = await _repo.GetByIdAsync(id);
            if (current is null) return false;
            if (!string.Equals(current.UserId, userId, StringComparison.Ordinal)) return false;

            current.Title = dto.Title ?? current.Title;
            current.Icon = dto.Icon ?? current.Icon;
            current.Platform = dto.Platform ?? current.Platform;
            current.DefaultUrl = dto.DefaultUrl ?? current.DefaultUrl;
            current.Status = dto.Status ?? current.Status;

            await _repo.UpdateAsync(current);
            return true;
        }
    }
}
