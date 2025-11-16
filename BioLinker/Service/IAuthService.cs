using BioLinker.DTO;
using BioLinker.DTO.UserDTO;
using BioLinker.Enities;

namespace BioLinker.Service
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(Login request);
        Task<LoginResponse> GoogleLoginAsync(GoogleAuthSettings request);
        Task<User> AddFacebookUserAsync(string email, string name, string? pictureUrl);
        Task<RegisterResponse?> RegisterAsync(Register request);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> ResetPasswordAsync(ResetPassword dto);
        Task<bool> UpdateRoleAsync(UpdateRole dto);
        Task<bool> UpdateProfileAsync(UpdateProfile dto);
        Task<(bool Success, string? Error)> UpdateUserProfileCustomizeAsync(ProfileCustomizeUpdate dto);
        Task<UserProfileResponse?> GetUserProfileAsync(string userId);

        Task<List<string>> GetAllCustomDomainNamesAsync();
        Task<string?> GetCustomDomainByUserIdAsync(string userId);
        Task<List<string>> GetAllUserEmailsAsync();
    }
}
