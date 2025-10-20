using BioLinker.Enities;

namespace BioLinker.Respository.PaymentRepo
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment?> GetByOrderCodeAsync(string orderCode);
        Task UpdateAsync(Payment payment);
    }
}
