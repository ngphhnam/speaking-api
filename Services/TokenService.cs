using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Options;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Services;

public class TokenService(
    UserManager<ApplicationUser> userManager,
    IOptions<JwtOptions> jwtOptions) : ITokenService
{
    private readonly JwtOptions _options = jwtOptions.Value;
    private readonly byte[] _key = Encoding.UTF8.GetBytes(jwtOptions.Value.Key);

    public async Task<(string Token, DateTimeOffset ExpiresAt)> GenerateAccessTokenAsync(ApplicationUser user, CancellationToken ct)
    {
        var roles = await GetUserRolesAsync(user);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var expires = DateTimeOffset.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires.UtcDateTime,
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return (handler.WriteToken(token), expires);
    }

    public async Task<IReadOnlyCollection<string>> GetUserRolesAsync(ApplicationUser user)
        => (await userManager.GetRolesAsync(user)).ToList();

    public RefreshToken GenerateRefreshToken(Guid userId)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(randomBytes);
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_options.RefreshTokenDays)
        };
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(_key),
            ValidateLifetime = false
        };

        var handler = new JwtSecurityTokenHandler();
        try
        {
            var principal = handler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}

