using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Infrastructure.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FastDeliveruu.Infrastructure.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<AppUser> _userManager;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtOptions, UserManager<AppUser> userManager)
    {
        _jwtSettings = jwtOptions.Value;
        _userManager = userManager;
    }

    public async Task<string> GenerateTokenAsync(AppUser appUser)
    {
        SigningCredentials signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256
        );

        var roles = await _userManager.GetRolesAsync(appUser);

        IEnumerable<Claim> roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));

        var claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, appUser.Id),
            new Claim(JwtRegisteredClaimNames.NameId, appUser.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        }
        .Union(roleClaims);

        JwtSecurityToken securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: DateTime.UtcNow.AddDays(_jwtSettings.ExpiryDays),
            claims: claims,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}