using BioLinker.Enities;

namespace BioLinker.Respository.ClickRepo
{
    public interface ICountTemplateClickedRepository
    {
        Task<IEnumerable<CountTemplateClicked>> GetAllAsync();
        Task<CountTemplateClicked?> GetByIdAsync(string id);
        Task<IEnumerable<CountTemplateClicked>> GetByDateAsync(DateTime date);
        Task<IEnumerable<CountTemplateClicked>> GetByWeekAsync(int week, int year);
        Task<IEnumerable<CountTemplateClicked>> GetByMonthAsync(int month, int year);
        Task<int> GetTotalCountAsync();
        Task AddAsync(CountTemplateClicked entity);
        Task DeleteAsync(string id);
    }
}
