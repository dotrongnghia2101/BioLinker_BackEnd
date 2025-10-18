using BioLinker.DTO.LinkDTO;
using BioLinker.Enities;
using BioLinker.Respository.LinkRepo;

namespace BioLinker.Service
{
    public class LinkService : ILinkService
    {
        private readonly ILinkRepository _repo;
        private readonly IAnalyticLinkRepository _analyticRepo;

        public LinkService(ILinkRepository repo, IAnalyticLinkRepository analyticRepo)
        {
            _repo = repo;
            _analyticRepo = analyticRepo;
        }

        private static LinkResponse ToDto(Link entity, int totalClicks = 0)
        {
            return new LinkResponse
            {
                LinkId = entity.LinkId ?? string.Empty,
                BioPageId = entity.BioPageId,
                StaticLinkId = entity.StaticLinkId,
                Title = entity.Title,
                Url = entity.Url,
                Icon = entity.Icon,
                Platform = entity.Platform,
                LinkType = entity.LinkType,
                TotalClicks = totalClicks
            };
        }

        public async Task<LinkResponse> CreateAsync(LinkCreate dto)
        {
            var entity = new Link
            {
                BioPageId = dto.BioPageId,
                StaticLinkId = dto.StaticLinkId,
                Title = dto.Title,
                Url = dto.Url,
                Icon = dto.Icon,
                Platform = dto.Platform,
                LinkType = dto.LinkType
            };

            await _repo.CreateAsync(entity);
            return ToDto(entity);
        }

        public async Task DeleteAsync(string linkId)
        {
            await _repo.DeleteAsync(linkId);
        }

        public async Task<IEnumerable<LinkResponse>> GetAllByBioPageAsync(string bioPageId)
        {
            var links = await _repo.GetAllByBioPageAsync(bioPageId);
            var result = new List<LinkResponse>();

            foreach (var l in links)
            {
                var totalClicks = await _analyticRepo.GetTotalClicksByLinkAsync(l.LinkId!);
                result.Add(ToDto(l, totalClicks ?? 0));
            }
            return result;
        }

        public async Task<LinkResponse?> GetByIdAsync(string linkId)
        {
            var entity = await _repo.GetByIdAsync(linkId);
            if (entity == null) return null;

            var totalClicks = await _analyticRepo.GetTotalClicksByLinkAsync(linkId) ?? 0;
            return ToDto(entity, totalClicks);
        }

        public async Task UpdateAsync(Link link)
        {
             await _repo.UpdateAsync(link);
        }
    }
}
