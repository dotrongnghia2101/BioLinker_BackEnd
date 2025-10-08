using BioLinker.DTO;
using BioLinker.Enities;
using BioLinker.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BioLinker.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly JwtService _jwtService;
        private readonly IEmailVerificationService _emailVerificationService;

        public AuthController(IAuthService authService, JwtService jwtService , IEmailVerificationService emailVerificationService)
        {
            _authService = authService;
            _jwtService = jwtService;
            _emailVerificationService = emailVerificationService;
        }

        //dang ki nguoi dung moi
        [HttpPost("Register")]
        public async Task<IActionResult> Register(Register request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //dang nhap
        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Invalid input." });

            try
            {
                var response = await _authService.LoginAsync(request);

                //  401
                if (response == null)
                    return Unauthorized(new { error = "Invalid email or password." });

                // Thành công -> set cookie + trả data
                Response.Cookies.Append("jwt", response.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Trường hợp tài khoản Google chưa có password (service throw)
                return BadRequest(new { error = ex.Message });
            }
        }

        // Controllers/User/AuthController.cs
        [HttpPost("login-google")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleAuthSettings request)
        {
            try
            {
                var response = await _authService.GoogleLoginAsync(request);

                // Lưu token vào cookie
                Response.Cookies.Append("jwt", response.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Google login failed: " + ex.Message
                });
            }
        }

        
        [HttpGet("login-facebook")]
        public IActionResult LoginFacebook()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("FacebookResponse")
            };
            return Challenge(props, "Facebook");
        }

        [HttpGet("facebook-response")]
        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return BadRequest("Facebook login failed");

            // Lấy thông tin người dùng từ Facebook
            var claims = result.Principal.Claims.ToDictionary(c => c.Type, c => c.Value);

            var email = claims.GetValueOrDefault(ClaimTypes.Email);
            var name = claims.GetValueOrDefault(ClaimTypes.Name);


            if (string.IsNullOrEmpty(email)) { 
                return BadRequest("Facebook login failed: missing email permission");
            }

            var user = await _authService.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = await _authService.AddFacebookUserAsync(email, name ?? "Facebook User");
            }

            //  Sinh JWT cho frontend (tùy bạn muốn redirect hay trả JSON)
            var token = _jwtService.GenerateToken(user);

            return Ok(new FacebookLoginResponse
            {
                Message = "Facebook login success",
                Email = user.Email,
                Name = user.FullName,
                UserId = user.UserId,
                Role = "FreeUser",
                Token = token
            });
        }

        //reset mat khau
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(dto);
            if (!result)
                return NotFound("Email not found");

            return Ok("Password reset sucessfully.");
        }

        //update profile
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfile dto)
        {
            var result = await _authService.UpdateProfileAsync(dto);
            if (!result) return NotFound("User not found");
            return Ok("Profile updated successfully.");
        }

        //update lai role neu user co mua goi 
        [HttpPut("update-role")]
        public async Task<IActionResult> UpdateRole(UpdateRole dto)
        {
            var result = await _authService.UpdateRoleAsync(dto);
            if (!result) return NotFound("User not found");
            return Ok("Role updated successfully.");
        }

        //update profile customize (job, UserImage, NickName, Description, CustomerDomain)
        [HttpPatch("profile-customize")]
        public async Task<IActionResult> UpdateProfileCustomize([FromBody] ProfileCustomizeUpdate dto)
        {
            var (ok, error) = await _authService.UpdateUserProfileCustomizeAsync(dto);
            if (!ok) 
            {
                return BadRequest(new { error });
            }
            return Ok(new { message = "Profile updated successfully." });
        }

        [HttpPost("email/send")]
        public async Task<IActionResult> SendCode([FromBody] EmailSend dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
                return BadRequest(new { message = "Email cannot be empty." });

            await _emailVerificationService.SendEmailConfirmationAsync(dto.Email);
            return Ok(new { message = "Verification code sent to your email." });
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailConfirmation dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Code))
                return BadRequest(new { message = "Email and code are required." });

            var success = await _emailVerificationService.ConfirmEmailAsync(dto);
            if (!success)
                return BadRequest(new { message = "Invalid or expired code." });

            return Ok(new { message = "Email verified successfully." });
        }

    }
}
