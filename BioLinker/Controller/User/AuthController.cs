using BioLinker.DTO;
using BioLinker.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BioLinker.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly JwtService _jwtService;

        public AuthController(IAuthService authService, JwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
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
            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized(new { error = "Invalid email or password." });
            }

            Response.Cookies.Append("jwt", response.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
            ////an token khoi body
            //response.Token = "";

            return Ok(response);
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

    }
}
