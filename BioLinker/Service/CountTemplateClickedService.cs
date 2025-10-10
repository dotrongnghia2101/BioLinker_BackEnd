using BioLinker.DTO.CountDTO;
using BioLinker.Enities;
using BioLinker.Respository.ClickRepo;

namespace BioLinker.Service
{
    public class CountTemplateClickedService : ICountTemplateClickedService
    {

        private readonly ICountTemplateClickedRepository _repository;

        public CountTemplateClickedService(ICountTemplateClickedRepository repository)
        {
            _repository = repository;
        }

        public async Task<CountTemplateClickedResponse> AddAsync(CountTemplateClickedCreated dto)
        {
            var entity = new CountTemplateClicked
            {
                CountTemplateClickedId = Guid.NewGuid().ToString(),
                TemplateId = dto.TemplateId!,
                ClickedId = dto.ClickedId!,
                ClickedTime = DateTime.Now,
            };
            await _repository.AddAsync(entity);

            return new CountTemplateClickedResponse
            {
                CountTemplateClickedId = entity.CountTemplateClickedId,
                TemplateId = entity.TemplateId,
                ClickedId = entity.ClickedId,
                ClickedTime = entity.ClickedTime
            };
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CountTemplateClickedResponse>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            return data.Select(x => new CountTemplateClickedResponse
            {
                CountTemplateClickedId = x.CountTemplateClickedId,
                TemplateId = x.TemplateId,
                ClickedId = x.ClickedId,
                ClickedTime = x.ClickedTime
            });
        }

        public async Task<IEnumerable<CountTemplateClickedResponse>> GetByDateAsync(DateTime date)
        {
            var data = await _repository.GetByDateAsync(date);
            return data.Select(x => new CountTemplateClickedResponse
            {
                CountTemplateClickedId = x.CountTemplateClickedId,
                TemplateId = x.TemplateId,
                ClickedId = x.ClickedId,
                ClickedTime = x.ClickedTime
            });
        }

        public async Task<CountTemplateClickedResponse?> GetByIdAsync(string id)
        {
            var x = await _repository.GetByIdAsync(id);
            if (x == null) return null;

            return new CountTemplateClickedResponse
            {
                CountTemplateClickedId = x.CountTemplateClickedId,
                TemplateId = x.TemplateId,
                ClickedId = x.ClickedId,
                ClickedTime = x.ClickedTime
            };
        }

        public async Task<IEnumerable<CountTemplateClickedResponse>> GetByMonthAsync(int month, int year)
        {
            var data = await _repository.GetByMonthAsync(month, year);
            return data.Select(x => new CountTemplateClickedResponse
            {
                CountTemplateClickedId = x.CountTemplateClickedId,
                TemplateId = x.TemplateId,
                ClickedId = x.ClickedId,
                ClickedTime = x.ClickedTime
            });
        }

        public async Task<IEnumerable<CountTemplateClickedResponse>> GetByWeekAsync(int week, int year)
        {
            var data = await _repository.GetByWeekAsync(week, year);
            return data.Select(x => new CountTemplateClickedResponse
            {
                CountTemplateClickedId = x.CountTemplateClickedId,
                TemplateId = x.TemplateId,
                ClickedId = x.ClickedId,
                ClickedTime = x.ClickedTime
            });
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _repository.GetTotalCountAsync();
        }
    }
}
