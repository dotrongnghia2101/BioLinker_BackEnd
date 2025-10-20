using BioLinker.DTO.UserDTO;

namespace BioLinker.Service
{
    public interface IUserTemplateService
    {
        Task<UserTemplateResponse?> PurchaseTemplateAsync(UserTemplateCreate dto);
        Task<IEnumerable<UserTemplateResponse>> GetPurchasedTemplatesByUserAsync(string userId);
    }
}
