
using EquipmentRental.Application.Common.Contracts.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EquipmentRental.Infrastructure.Identity;

public sealed class AccessTokenService : IAccessTokenService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtSettings _jwtSettings;

    public AccessTokenService(
        UserManager<IdentityUser> userManager,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AccessTokenResult> GenerateTokenAsync(
        IdentityUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
{
    new(JwtRegisteredClaimNames.Sub, user.Id),
    new(ClaimTypes.NameIdentifier, user.Id),       // ← سطر جديد
    new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
    new(ClaimTypes.Email, user.Email ?? string.Empty),   // ← سطر جديد
    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
};

        claims.AddRange(
            roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        var signingCredentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(
            _jwtSettings.DurationInMinutes);

        var jwtToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: signingCredentials);

        var token = new JwtSecurityTokenHandler()
            .WriteToken(jwtToken);

        return new AccessTokenResult(token, expiresAtUtc);
    }
}
