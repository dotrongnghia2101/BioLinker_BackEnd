using BioLinker.DTO;

namespace BioLinker.Service
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(Login request);
        Task<LoginResponse> GoogleLoginAsync(GoogleAuthSettings request);
        Task<string> RegisterAsync(Register request);

        Task<bool> ResetPasswordAsync(ResetPassword dto);
    }
}
