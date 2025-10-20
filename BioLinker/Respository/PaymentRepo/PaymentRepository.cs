using BioLinker.Data;
using BioLinker.Enities;
using Microsoft.EntityFrameworkCore;

namespace BioLinker.Respository.PaymentRepo
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDBContext _db;

        public PaymentRepository(AppDBContext db)
        {
            _db = db;
        }
        public async Task<Payment> CreateAsync(Payment payment)
        {
            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetByOrderCodeAsync(string orderCode)
        {
            return await _db.Payments.FirstOrDefaultAsync(p => p.OrderCode == orderCode);
        }

        public async Task UpdateAsync(Payment payment)
        {
            _db.Payments.Update(payment);
            await _db.SaveChangesAsync();
        }
    }
}
