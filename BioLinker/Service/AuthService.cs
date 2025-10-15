using Azure.Core;
using BioLinker.Data;
using BioLinker.DTO;
using BioLinker.DTO.UserDTO;
using BioLinker.Enities;
using BioLinker.Respository.UserRepo;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BioLinker.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRoleRepository  _userRoleRepository;
        private readonly AppDBContext _appDBContext;
        private readonly IEmailVerificationService _emailVerificationService;
        public AuthService(IUserRepository userRepository, 
               JwtService jwtService, 
               IPasswordHasher<User> passwordHasher, 
               IConfiguration configuration, 
               IRoleRepository roleRepository, 
               IUserRoleRepository userRoleRepository,
               AppDBContext appDBContext,
               IEmailVerificationService emailVerificationService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _appDBContext = appDBContext;
            _emailVerificationService = emailVerificationService;
        }

        public async Task<User> AddFacebookUserAsync(string email, string name, string? pictureUrl = null)
        {
            var user = new User
            {
                Email = email,
                FullName = name,
                UserImage = pictureUrl,
                IsActive = true,
                IsGoogle = false,
                IsBeginner = true,
                CreatedAt = DateTime.UtcNow
            };

            // Gán role mặc định FreeUser
            var defaultRole = await _roleRepository.GetByNameAsync("FreeUser");
            if (defaultRole != null)
            {
                user.UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = defaultRole.RoleId,
                        StartDate = DateTime.UtcNow,
                        EndDate = null
                    }
                };
            }

            await _userRepository.AddUserAsync(user);

            return user;
        }

        public async Task<List<string>> GetAllCustomDomainNamesAsync()
        {
            return await _userRepository.GetAllCustomDomainNamesAsync();
        }

        public async Task<string?> GetCustomDomainByUserIdAsync(string userId)
        {
            return await _userRepository.GetCustomDomainByUserIdAsync(userId);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<UserProfileResponse?> GetUserProfileAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            var role = user.UserRoles.FirstOrDefault()?.Role?.RoleName ?? "FreeUser";

            return new UserProfileResponse
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = role,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                UserImage = user.UserImage,
                Job = user.Job,
                DateOfBirth = user.DateOfBirth,
                IsGoogle = user.IsGoogle,
                CustomerDomain = user.CustomerDomain,
                Description = user.Description,
                NickName = user.NickName,
                IsBeginner = user.IsBeginner,
            };       
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
                    CreatedAt = DateTime.Now,
                    IsGoogle = true,
                    CustomerDomain = null,
                    Job = null,
                    Description = null,
                    DateOfBirth = null,
                    NickName = null,
                    Gender = null,
                    IsBeginner = true,
                };

                await _userRepository.AddUserAsync(newUser);

                //role mac dinh la freeuser
                var freeRole = await _roleRepository.GetByNameAsync("FreeUser");
                if (freeRole != null)
                {
                    var userRole = new UserRole
                    {
                        UserRoleId = Guid.NewGuid().ToString(),
                        UserId = newUser.UserId,
                        RoleId = freeRole.RoleId
                    };

                    await _userRoleRepository.AddAsync(userRole);
                    newUser.UserRoles = new List<UserRole> { userRole };
                }               
                existingUser = newUser;
            }

            var roleName = existingUser.UserRoles?
                            .FirstOrDefault()?.Role?.RoleName ?? "FreeUser";

            // Tao JWT Token de client luu dang nhap
            var token = _jwtService.GenerateToken(existingUser);
            return new LoginResponse
            {
                Token = token,
                UserId = existingUser.UserId,
                FullName = existingUser.FullName,
                Email = existingUser.Email,
                PhoneNumber = existingUser.PhoneNumber,
                Role = roleName,
                Gender = existingUser.Gender,
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
            // is google true va k co pass
            if ((user.IsGoogle ?? false) && string.IsNullOrEmpty(user.PasswordHash))
                throw new Exception("This account is Google Account. PLease Login by Google");

            //hash lai pasword de so sanh
            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verifyResult != PasswordVerificationResult.Success)
            {
                return null;
            }

            var roleName = user.UserRoles?
                            .FirstOrDefault()?.Role?.RoleName ?? "FreeUser";

            var token = _jwtService.GenerateToken(user);
            return new LoginResponse
            {
                Token = token,
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = roleName,
                Gender = user.Gender,
                PhoneNumber= user.PhoneNumber,
                UserImage = user.UserImage,       
                Job = user.Job,
                DateOfBirth = user.DateOfBirth,
                NickName = user.NickName,
                Description = user.Description,
                CustomerDomain = user.CustomerDomain,
                IsGoogle = user.IsGoogle,
                IsBeginner = user.IsBeginner,
            };
        }

        //register
        public async Task<RegisterResponse?> RegisterAsync(Register request)
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
                IsActive = false, // ve sau co them xac thuc
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                IsGoogle = false,
                IsBeginner = true,
            };
            //hash password
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            //luu vao database
            await _userRepository.AddUserAsync(user);

            var defaultRole = await _roleRepository.GetByNameAsync("FreeUser");
            if (defaultRole == null)
            {
                throw new Exception("Default role FreeUser not found.");
            }

            var userRole = new UserRole
            {
                UserId = user.UserId,
                RoleId = defaultRole.RoleId,
                StartDate = DateTime.UtcNow, // ngay dang ki
                EndDate = null               // vo thoi han
            };

            await _userRoleRepository.AddAsync(userRole);

            try
            {
                // inject qua constructor: IEmailVerificationService _emailVerificationService
                _ = Task.Run(() => _emailVerificationService.SendEmailConfirmationAsync(user.Email!));
            }
            catch (Exception ex)
            {
                // không làm fail toàn bộ flow nếu lỗi gửi mail
                Console.WriteLine($"Email send failed: {ex.Message}");
            }

            return new RegisterResponse
            {
                UserId = user.UserId,
                Message = "User registered successfully. Please check your email to verify your account.",
            };

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
            user.Gender = dto.Gender ?? user.Gender;

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

        public async Task<(bool Success, string? Error)> UpdateUserProfileCustomizeAsync(ProfileCustomizeUpdate dto)
        {
            var user = await _appDBContext.Users.FindAsync(dto.UserId);
            if (user == null) 
            { 
                return (false, "User not found.");
            }
            // Normalize custom domain: trim + lowercase
            string? normDomain = dto.CustomDomain?.Trim();
            if (!string.IsNullOrEmpty(normDomain))
            {
                normDomain = normDomain.ToLowerInvariant();

                //  validate pattern: a-z0-9- only, 3-63 chars
                var rgx = new Regex(@"^[a-z0-9]([a-z0-9-]{1,61}[a-z0-9])?$");
                if (!rgx.IsMatch(normDomain))
                    return (false, "Custom domain is invalid. Use 3-63 chars [a-z0-9-], no spaces.");
                //check duplicated
                bool existed = await _appDBContext.Users
                          .AnyAsync(u => u.CustomerDomain != null &&
                         u.CustomerDomain.ToLower() == normDomain &&
                         u.UserId != dto.UserId);
                if (existed)
                {
                    return (false, "Customer domain is already taken.");
                }
            }
                // Update fields
                user.Job = dto.Job;
                user.NickName = dto.Nickname;
                user.UserImage = dto.UserImage;
                user.Description = dto.Description;
                user.CustomerDomain = string.IsNullOrEmpty(normDomain) ? null : normDomain;
                user.IsBeginner = dto.IsBeginner;

                await _appDBContext.SaveChangesAsync();
                return (true, null);
            }
        }
    }

