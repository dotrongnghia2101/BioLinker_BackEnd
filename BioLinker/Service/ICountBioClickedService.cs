using BioLinker.DTO.CountDTO;

namespace BioLinker.Service
{
    public interface ICountBioClickedService
    {
        Task<IEnumerable<CountBioClickedResponse>> GetAllAsync();
        Task<CountBioClickedResponse?> GetByIdAsync(string id);
        Task<IEnumerable<CountBioClickedResponse>> GetByDateAsync(DateTime date);
        Task<IEnumerable<CountBioClickedResponse>> GetByWeekAsync(int week, int year);
        Task<IEnumerable<CountBioClickedResponse>> GetByMonthAsync(int month, int year);
        Task<int> GetTotalCountAsync();
        Task<CountBioClickedResponse> AddAsync(CountBioClickCreate dto);
        Task DeleteAsync(string id);
    }
}
