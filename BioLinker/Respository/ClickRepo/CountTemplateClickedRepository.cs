using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.ClickRepo
{
    public class CountTemplateClickedRepository : ICountTemplateClickedRepository
    {
        private readonly AppDBContext _context;

        public CountTemplateClickedRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CountTemplateClicked entity)
        {
            _context.CountTemplateClickeds.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.CountTemplateClickeds.FindAsync(id);
            if (entity != null)
            {
                _context.CountTemplateClickeds.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CountTemplateClicked>> GetAllAsync()
        {
            return await _context.CountTemplateClickeds
               .Include(c => c.Template)
               .ToListAsync();
        }

        public async Task<IEnumerable<CountTemplateClicked>> GetByDateAsync(DateTime date)
        {
            return await _context.CountTemplateClickeds
              .Where(c => c.ClickedTime.Date == date.Date)
              .ToListAsync();
        }

        public async Task<CountTemplateClicked?> GetByIdAsync(string id)
        {
            return await _context.CountTemplateClickeds
                .Include(c => c.Template)
                .FirstOrDefaultAsync(x => x.CountTemplateClickedId == id);
        }

        public async Task<IEnumerable<CountTemplateClicked>> GetByMonthAsync(int month, int year)
        {
            return await _context.CountTemplateClickeds
                .Where(c => c.ClickedTime.Month == month && c.ClickedTime.Year == year)
                .ToListAsync();
        }

        public async Task<IEnumerable<CountTemplateClicked>> GetByWeekAsync(int week, int year)
        {
            return await _context.CountTemplateClickeds
                .FromSqlRaw("SELECT * FROM CountTemplateClicked WHERE WEEK(clickedTime, 1) = {0} AND YEAR(clickedTime) = {1}", week, year)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.CountTemplateClickeds.CountAsync();
        }
    }
}
