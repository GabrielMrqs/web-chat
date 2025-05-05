using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebChat.Application.DTOs;
using WebChat.Application.Services;
using WebChat.Infra.Configuration;

namespace WebChat.Infra.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwt;

        public JwtTokenService(IOptions<JwtSettings> opts)
        {
            _jwt = opts.Value;
        }

        public WebToken GenerateToken(string nameIdentifier)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, nameIdentifier)
            };

            var expires = DateTime.UtcNow.AddHours(_jwt.ExpiryHours);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new WebToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = DateTime.UtcNow.AddHours(_jwt.ExpiryHours)
            };
        }
    }
}
