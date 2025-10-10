using BioLinker.DTO.CountDTO;
using BioLinker.Enities;
using BioLinker.Respository.ClickRepo;

namespace BioLinker.Service
{
    public class CountBioClickedService : ICountBioClickedService
    {

        private readonly ICountBioClickedRepository _repository;

        public CountBioClickedService(ICountBioClickedRepository repository)
        {
            _repository = repository;
        }

        public async Task<CountBioClickedResponse> AddAsync(CountBioClickCreate dto)
        {
            var entity = new CountBioClicked
            {
                CountBioClickedId = Guid.NewGuid().ToString(),
                UserId = dto.UserId!,
                BioPageId = dto.BioPageId!,
                ClickedId = dto.ClickedId!,
                ClickedTime = DateTime.UtcNow,
            };
            await _repository.AddAsync(entity);
            return new CountBioClickedResponse
            {
                CountBioClickedId = entity.CountBioClickedId,
                UserId = entity.UserId,
                BioPageId = entity.BioPageId,
                ClickedId = entity.ClickedId,
                ClickedTime = entity.ClickedTime
            };
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CountBioClickedResponse>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            return data.Select(x => new CountBioClickedResponse
            {
                CountBioClickedId = x.CountBioClickedId,
                UserId = x.UserId,
                BioPageId = x.BioPageId,
                ClickedId = x.ClickedId,
                ClickedTime = x.ClickedTime
            });
        }

        public async Task<IEnumerable<CountBioClickedResponse>> GetByDateAsync(DateTime date)
        {
            var data = await _repository.GetByDateAsync(date);
            return data.Select(x => new CountBioClickedResponse
            {
                CountBioClickedId = x.CountBioClickedId,
                UserId = x.UserId,
                BioPageId = x.BioPageId,
                ClickedId = x.ClickedId,
                ClickedTime = x.ClickedTime
            });
        }

        public async Task<CountBioClickedResponse?> GetByIdAsync(string id)
        {
            var x = await _repository.GetByIdAsync(id);
            if (x == null) return null;

            return new CountBioClickedResponse
            {
                CountBioClickedId = x.CountBioClickedId,
                UserId = x.UserId,
                BioPageId = x.BioPageId,
                ClickedId = x.ClickedId,
                ClickedTime = x.ClickedTime
            };
        }

        public async Task<IEnumerable<CountBioClickedResponse>> GetByMonthAsync(int month, int year)
        {
            var data = await _repository.GetByMonthAsync(month, year);
            return data.Select(x => new CountBioClickedResponse
            {
                CountBioClickedId = x.CountBioClickedId,
                UserId = x.UserId,
                BioPageId = x.BioPageId,
                ClickedId = x.ClickedId,
                ClickedTime = x.ClickedTime
            });
        }

        public async Task<IEnumerable<CountBioClickedResponse>> GetByWeekAsync(int week, int year)
        {
            var data = await _repository.GetByWeekAsync(week, year);
            return data.Select(x => new CountBioClickedResponse
            {
                CountBioClickedId = x.CountBioClickedId,
                UserId = x.UserId,
                BioPageId = x.BioPageId,
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
