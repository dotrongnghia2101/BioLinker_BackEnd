using BioLinker.DTO.CountDTO;

namespace BioLinker.Service
{
    public interface ICountTemplateClickedService
    {
        Task<IEnumerable<CountTemplateClickedResponse>> GetAllAsync();
        Task<CountTemplateClickedResponse?> GetByIdAsync(string id);
        Task<IEnumerable<CountTemplateClickedResponse>> GetByDateAsync(DateTime date);
        Task<IEnumerable<CountTemplateClickedResponse>> GetByWeekAsync(int week, int year);
        Task<IEnumerable<CountTemplateClickedResponse>> GetByMonthAsync(int month, int year);
        Task<int> GetTotalCountAsync();
        Task<CountTemplateClickedResponse> AddAsync(CountTemplateClickedCreated dto);
        Task DeleteAsync(string id);
    }
}
