using BioLinker.Enities;
using Microsoft.EntityFrameworkCore.Storage;

namespace BioLinker.Respository.BioPageRepo
{
    public interface IBioPageRepository
    {
        Task<BioPage?> GetByIdAsync(string id);
        Task<IEnumerable<BioPage>> GetAllAsync();
        Task<IEnumerable<BioPage>> GetByUserIdAsync(string userId);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task AddAsync(BioPage entity);
        Task UpdateAsync(BioPage entity);
        Task DeleteAsync(BioPage entity);
    }
}
