using BioLinker.Enities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BioLinker.Service
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        //Tao token tu info user
        public string GenerateToken(User user)
        {
            //tao key tu AppSetting
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roleName = user.UserRoles?
                .FirstOrDefault()?.Role?.RoleName ?? "FreeUser";

            //dua thong tin vao token
            var claims = new[]
            {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("UserId", user.UserId.ToString()),
            new Claim(ClaimTypes.Role, roleName)
        };

            //tao token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
