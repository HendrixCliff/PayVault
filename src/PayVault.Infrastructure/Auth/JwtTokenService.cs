using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using PayVault.Infrastructure.Identity;

namespace PayVault.Infrastructure.Auth
{
    public interface IJwtTokenService
    {
        Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(ApplicationUser user);
        Task<string> GenerateAccessTokenAsync(ApplicationUser user);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtTokenService(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _config = config;
            _userManager = userManager;
        }

        public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(ApplicationUser user)
        {
            var accessToken = await GenerateAccessTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return (accessToken, refreshToken);
        }

public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
{
    var jwtKey = _config["Jwt:Key"];
    var issuer = _config["Jwt:Issuer"];
    var audience = _config["Jwt:Audience"];

    var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

    var claims = new List<Claim>
    {
        new Claim("nameid", user.Id.ToString()),
        new Claim("email", user.Email ?? ""),
        new Claim("unique_name", user.FullName ?? user.UserName ?? "")
    };

    var roles = await _userManager.GetRolesAsync(user);
    foreach (var role in roles.Distinct())
    {
        claims.Add(new Claim("role", role));
    }

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddMinutes(60),
        Issuer = issuer,
        Audience = audience,
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256)
    };

    var handler = new JwtSecurityTokenHandler();
    var token = handler.CreateToken(tokenDescriptor);
    return handler.WriteToken(token);
}
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}