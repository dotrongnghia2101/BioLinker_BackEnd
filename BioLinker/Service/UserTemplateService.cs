using BioLinker.DTO.UserDTO;
using BioLinker.Enities;
using BioLinker.Respository.User;

namespace BioLinker.Service
{
    public class UserTemplateService : IUserTemplateService
    {
        private readonly IUserTemplateRepository _repo;

        public UserTemplateService(IUserTemplateRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<UserTemplateResponse>> GetPurchasedTemplatesByUserAsync(string userId)
        {
            var data = await _repo.GetByUserAsync(userId);
            return data.Select(x => new UserTemplateResponse
            {
                UTemplateId = x.UTemplateId,
                UserId = x.UserId,
                TemplateId = x.TemplateId,
                PricePaid = x.PricePaid,
                Currency = x.Currency,
                PurchaseAt = x.PurchaseAt,
                ExpireDate = x.ExpireDate
            });
        }

        public async Task<UserTemplateResponse?> PurchaseTemplateAsync(UserTemplateCreate dto)
        {
            if (string.IsNullOrEmpty(dto.UserId) || string.IsNullOrEmpty(dto.TemplateId))
                return null;

            // kiểm tra user đã mua template này chưa
            bool exists = await _repo.ExistsAsync(dto.UserId, dto.TemplateId);
            if (exists)
                throw new InvalidOperationException("User already owns this template.");

            var entity = new UserTemplate
            {
                UserId = dto.UserId,
                TemplateId = dto.TemplateId,
                PricePaid = dto.PricePaid,
                Currency = dto.Currency ?? "USD",
                PurchaseAt = DateTime.UtcNow,
                ExpireDate = dto.DurationInDays.HasValue
                    ? DateTime.UtcNow.AddDays(dto.DurationInDays.Value)
                    : null
            };

            await _repo.AddAsync(entity);

            return new UserTemplateResponse
            {
                UTemplateId = entity.UTemplateId,
                UserId = entity.UserId,
                TemplateId = entity.TemplateId,
                PricePaid = entity.PricePaid,
                Currency = entity.Currency,
                PurchaseAt = entity.PurchaseAt,
                ExpireDate = entity.ExpireDate
            };
        }
    }
}
