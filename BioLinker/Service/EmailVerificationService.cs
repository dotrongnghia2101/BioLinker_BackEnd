using BioLinker.DTO.UserDTO;
using BioLinker.Respository.UserRepo;
using System.Security.Cryptography;
using System.Text;

namespace BioLinker.Service
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly IUserRepository _userRepository;
        private const string SecretKey = "BioLinkerSecretKey2025";

        public EmailVerificationService(IEmailSender emailSender, IUserRepository userRepository)
        {
            _emailSender = emailSender;
            _userRepository = userRepository;
        }

        public async Task<bool> ConfirmEmailAsync(EmailConfirmation dto)
        {
            if (!VerifyEmailCode(dto.Email, dto.Code))
                return false;

            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) return false;

            user.IsActive = true;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public  string GenerateEmailCode(string email)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey));
            long timestamp = DateTime.UtcNow.Ticks / TimeSpan.FromMinutes(10).Ticks;
            string data = $"{email}:{timestamp}";
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            int code = BitConverter.ToInt32(hash, 0) & 0x7FFFFFFF;
            return (code % 1000000).ToString("D6");
        }

        public async Task SendEmailConfirmationAsync(string email)
        {
            var code = GenerateEmailCode(email);
            await _emailSender.SendConfirmationEmailAsync(email, code);
        }

        public bool VerifyEmailCode(string email, string code)
        {
            var current = GenerateEmailCode(email);
            var previous = GenerateEmailCode(email + ":prev"); // hỗ trợ lệch nhẹ
            return code == current || code == previous;
        }
    }
}
