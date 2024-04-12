using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Dapper;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FastDeliveruu.Application.Services;

public class LocalUserServices : ILocalUserServices
{
    private readonly ISP_Call _sP_Call;
    private readonly IMapper _mapper;
    private readonly string _secretKey;

    public LocalUserServices(ISP_Call sP_Call,
        IMapper mapper,
        IConfiguration configuration)
    {
        _sP_Call = sP_Call;
        _mapper = mapper;
        _secretKey = configuration["ApiSettings:Secret"];
    }

    public async Task<bool> IsUniqueUserAsync(string username)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UserName", username);

        return await _sP_Call.OneRecordAsync<LocalUser>(
            StoreProcedureNames.SpGetLocalUserByUserName, parameters) == null;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UserName", loginRequestDto.UserName);

        LocalUser? localUser = await _sP_Call.OneRecordAsync<LocalUser>(
            StoreProcedureNames.SpGetLocalUserByUserName, parameters);
        if (localUser == null)
        {
            return new LoginResponseDto
            {
                LocalUserDto = null,
                Token = string.Empty
            };
        }

        bool verified = BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, localUser.PasswordHash);
        if (!verified)
        {
            return new LoginResponseDto
            {
                LocalUserDto = null,
                Token = string.Empty
            };
        }

        // generate JWT token
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(_secretKey);

        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, localUser.LocalUserId.ToString()),
                new Claim(ClaimTypes.Role, localUser.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(descriptor);

        LoginResponseDto loginResponseDto = new LoginResponseDto
        {
            LocalUserDto = _mapper.Map<LocalUserDto>(localUser),
            Token = tokenHandler.WriteToken(token)
        };

        return loginResponseDto;
    }

    public async Task<LocalUserDto> RegisterAsync(RegisterationRequestDto registerationRequestDto)
    {
        LocalUserDto localUserDto = new LocalUserDto
        {
            FirstName = registerationRequestDto.FirstName,
            LastName = registerationRequestDto.LastName,
            UserName = registerationRequestDto.LastName,
            PhoneNumber = registerationRequestDto.PhoneNumber,
            Email = registerationRequestDto.Email,
            Role = registerationRequestDto.Role ?? "customer"
        };

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@FirstName", localUserDto.FirstName);
        parameters.Add("@LastName", localUserDto.LastName);
        parameters.Add("@UserName", localUserDto.UserName);
        parameters.Add("@Email", localUserDto.Email);
        parameters.Add("@PasswordHash", BCrypt.Net.BCrypt.HashPassword(registerationRequestDto.Password));
        parameters.Add("@PhoneNumber", localUserDto.PhoneNumber);
        parameters.Add("@Role", localUserDto.Role);
        parameters.Add("@CreatedAt", DateTime.Now);
        parameters.Add("@UpdatedAt", DateTime.Now);
        parameters.Add("@LocalUserId", DbType.Int32, direction: ParameterDirection.Output);

        await _sP_Call.ExecuteAsync(StoreProcedureNames.SpCreateLocalUser, parameters);

        localUserDto.LocalUserId = parameters.Get<int>("@LocalUserId");

        return localUserDto;
    }
}