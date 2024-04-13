using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FastDeliveruu.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JwtTokenGenerator(IDateTimeProvider dateTimeProvider, IOptions<JwtSettings> jwtOptions)
    {
        _dateTimeProvider = dateTimeProvider;
        _jwtSettings = jwtOptions.Value;
    }

    public string GenerateTokenAsync(LocalUserDto localUserDto)
    {
        SigningCredentials signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256
        );

        Claim[] claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, localUserDto.LocalUserId.ToString()),
            new Claim(JwtRegisteredClaimNames.GivenName, localUserDto.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, localUserDto.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        JwtSecurityToken securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            expires: _dateTimeProvider.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            claims: claims,
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }
}