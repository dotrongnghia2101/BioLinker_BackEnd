using BioLinker.DTO.TemplateDTO;
using BioLinker.Enities;
using BioLinker.Respository.TemplateRepo;

namespace BioLinker.Service
{
    public class CollectionService : ICollectionService
    {
        private readonly ICollectionRepository _repo;

        public CollectionService(ICollectionRepository repo)
        {
            _repo = repo;
        }
        

        public async Task<Collection> AddOrUpdateCollectionAsync(string userId, List<string> templateIds)
        {
            var existing = await _repo.GetByUserIdAsync(userId);
            if (existing == null)
            {
                var newCollection = new Collection
                {
                    UserId = userId,
                    TemplateIds = templateIds
                };
                await _repo.AddAsync(newCollection);
                return newCollection;
            }
            else
            {
                // Gộp không trùng templateId
                var merged = existing.TemplateIds.Union(templateIds).ToList();
                existing.TemplateIds = merged;
                await _repo.UpdateAsync(existing);
                return existing;
            }
        }

        public async Task<Collection?> GetCollectionByUserAsync(string userId)
        {
            return await _repo.GetByUserIdAsync(userId);
        }

        public async Task RemoveTemplatesAsync(string userId, List<string> templateIds)
        {
            var collection = await _repo.GetByUserIdAsync(userId);
            if (collection == null) return;

            var remaining = collection.TemplateIds.Except(templateIds).ToList();
            collection.TemplateIds = remaining;
            await _repo.UpdateAsync(collection);
        }
    }
}
