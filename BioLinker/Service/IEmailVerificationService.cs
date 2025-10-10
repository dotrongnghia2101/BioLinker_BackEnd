using BioLinker.DTO.UserDTO;

namespace BioLinker.Service
{
    public interface IEmailVerificationService
    {
        string GenerateEmailCode(string email);
        bool VerifyEmailCode(string email, string code);
        Task SendEmailConfirmationAsync(string email);
        Task<bool> ConfirmEmailAsync(EmailConfirmation dto);
    }
}
