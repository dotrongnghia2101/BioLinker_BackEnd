using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.ClickRepo
{
    public class CountBioClickedRepository : ICountBioClickedRepository
    {
        private readonly AppDBContext _context;

        public CountBioClickedRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CountBioClicked entity)
        {
            _context.CountBioClickeds.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entity = await _context.CountBioClickeds.FindAsync(id);
            if (entity != null)
            {
                _context.CountBioClickeds.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CountBioClicked>> GetAllAsync()
        {
            return await _context.CountBioClickeds
                .Include(c => c.User)
                .Include(c => c.BioPage)
                .ToListAsync();
        }

        public async Task<IEnumerable<CountBioClicked>> GetByDateAsync(DateTime date)
        {
            return await _context.CountBioClickeds
                .Where(c => c.ClickedTime.Date == date.Date)
                .ToListAsync();
        }

        public async Task<CountBioClicked?> GetByIdAsync(string id)
        {
            return await _context.CountBioClickeds
               .Include(c => c.User)
               .Include(c => c.BioPage)
               .FirstOrDefaultAsync(x => x.CountBioClickedId == id);
        }

        public async Task<IEnumerable<CountBioClicked>> GetByMonthAsync(int month, int year)
        {
            return await _context.CountBioClickeds
                .Where(c => c.ClickedTime.Month == month && c.ClickedTime.Year == year)
                .ToListAsync();
        }

        public async Task<IEnumerable<CountBioClicked>> GetByWeekAsync(int week, int year)
        {
            return _context.CountBioClickeds
                     .AsEnumerable() // xử lý trong bộ nhớ
                     .Where(c =>
                     {
                         var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
                         var weekNum = cal.GetWeekOfYear(
                             c.ClickedTime,
                             System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                             DayOfWeek.Monday);
                         return weekNum == week && c.ClickedTime.Year == year;
                     })
                     .ToList();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.CountBioClickeds.CountAsync();
        }
    }
}
