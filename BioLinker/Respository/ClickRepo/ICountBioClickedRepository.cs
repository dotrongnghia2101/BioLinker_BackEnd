using BioLinker.Enities;

namespace BioLinker.Respository.ClickRepo
{
    public interface ICountBioClickedRepository
    {
        Task<IEnumerable<CountBioClicked>> GetAllAsync();
        Task<CountBioClicked?> GetByIdAsync(string id);
        Task<IEnumerable<CountBioClicked>> GetByDateAsync(DateTime date);
        Task<IEnumerable<CountBioClicked>> GetByWeekAsync(int week, int year);
        Task<IEnumerable<CountBioClicked>> GetByMonthAsync(int month, int year);
        Task<int> GetTotalCountAsync();
        Task AddAsync(CountBioClicked entity);
        Task DeleteAsync(string id);
    }
}
