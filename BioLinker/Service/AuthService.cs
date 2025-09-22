using BioLinker.DTO;
using BioLinker.Enities;
using BioLinker.Repository;
using BioLinker.Respository;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;

namespace BioLinker.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IRoleRepository _roleRepository;

        public AuthService(IUserRepository userRepository, JwtService jwtService, IPasswordHasher<User> passwordHasher, IConfiguration configuration, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _roleRepository = roleRepository;
        }

        //login google
        public async Task<LoginResponse> GoogleLoginAsync(GoogleAuthSettings request)
        {
            // Lay clientId tu appsettings
            var clientId = _configuration["GoogleAuthSettings:ClientId"];

            // Kiem tra token tu Google va xac minh dung cho app minh (clientId)
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { clientId }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            // Kiem tra co ton tai nguoi dung 
            var existingUser = await _userRepository.GetByEmailAsync(payload.Email);
            // Neu chua co thi tao moi user moi tu thong tin google tra ve
            if (existingUser == null)
            {
                // Neu chua co thi tao moi user
                var newUser = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = payload.Email,
                    FullName = payload.Name,
                    FirstName = payload.FamilyName,
                    LastName = payload.GivenName,
                    UserImage = payload.Picture,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                await _userRepository.AddUserAsync(newUser);
                existingUser = newUser;

            }
            // Tao JWT Token de client luu dang nhap
            var token = _jwtService.GenerateToken(existingUser);
            return new LoginResponse
            {
                Token = token,
                UserId = existingUser.UserId,
                FullName = existingUser.FullName,
                Email = existingUser.Email,
            };
        }

        //login
        public async Task<LoginResponse?> LoginAsync(Login request)
        {
            //tim user theo email
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return null;
            }

            //hash lai pasword de so sanh
            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verifyResult != PasswordVerificationResult.Success)
            {
                return null;
            }

            var token = _jwtService.GenerateToken(user);
            return new LoginResponse
            {
                Token = token,
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
            };
        }

        //register
        public async Task<string> RegisterAsync(Register request)
        {
            //check mail da co hay chua
            var existUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existUser != null)
            {
                throw new Exception("Email already exits.");
            }
            //tao user moi
            var user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                FullName = $"{request.FirstName} {request.LastName}",
                IsActive = true, // ve sau co them xac thuc
                CreatedAt = DateTime.UtcNow,
            };
            //hash password
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            //luu vao database
            await _userRepository.AddUserAsync(user);
            return "User registered successfully.";
        }

        //reset password
        public async Task<bool> ResetPasswordAsync(ResetPassword dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) return false;

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.NewPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UpdateProfileAsync(UpdateProfile dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null) return false;

            user.FullName = dto.FullName ?? user.FullName;
            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.UserImage = dto.UserImage ?? user.UserImage;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UpdateRoleAsync(UpdateRole dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null) return false;

            // tim role theo ten
            var newRole = await _roleRepository.GetByNameAsync(dto.NewRoleName);
            if (newRole == null) return false;

            // da co role thi update
            var userRole = user.UserRoles.FirstOrDefault();
            if (userRole != null)
            {
                userRole.RoleId = newRole.RoleId;
                userRole.StartDate = DateTime.UtcNow;
            }
            else
            {
                // neu user chua co role them moi
                user.UserRoles = new List<UserRole>
        {
            new UserRole
            {
                UserId = user.UserId,
                RoleId = newRole.RoleId,
                StartDate = DateTime.UtcNow
            }
        };
            }

            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}
