using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.TemplateRepo
{
    public class TemplateDetailRepository : ITemplateDetailRepository
    {
        private readonly AppDBContext _context;

        public TemplateDetailRepository(AppDBContext context)
        {
            _context = context;
        }

        //tao moi
        public async Task<TemplateDetail> AddAsync(TemplateDetail detail)
        {
            _context.TemplateDetails.Add(detail);
            await _context.SaveChangesAsync();
            return detail;
        }

        //xoa
        public async Task<bool> DeleteAsync(string id)
        {
            var detail = await _context.TemplateDetails
               .FirstOrDefaultAsync(td => td.TemplateDetailId == id);

            if (detail == null) return false;

            _context.TemplateDetails.Remove(detail);
            await _context.SaveChangesAsync();
            return true;
        }

        //lay chi tiet 1 phan
        public async Task<TemplateDetail?> GetByIdAsync(string id)
        {
            return await _context.TemplateDetails.FindAsync(id);
        }

        //lay toan bo theo templateid
        public async Task<List<TemplateDetail>> GetByTemplateIdAsync(string templateId)
        {
            return await _context.TemplateDetails
                .Where(td => td.TemplateId == templateId)
                .OrderBy(td => td.OrderIndex)
                .ToListAsync();
        }

        //cap nhat
        public async Task<TemplateDetail?> UpdateAsync(TemplateDetail detail)
        {
            var existing = await _context.TemplateDetails
                 .FirstOrDefaultAsync(td => td.TemplateDetailId == detail.TemplateDetailId);

            if (existing == null) return null;

            _context.Entry(existing).CurrentValues.SetValues(detail);
            await _context.SaveChangesAsync();

            return existing;
        }
    }
}
