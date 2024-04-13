using System.Data;
using Dapper;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Application.Services;

public class AuthenticationServices : IAuthenticationServices
{
    private readonly ISP_Call _sP_Call;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationServices(ISP_Call sP_Call,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _sP_Call = sP_Call;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthenticationResult> RegisterAsync(RegisterationRequestDto registerationRequestDto)
    {
        // check if the user already exist
        DynamicParameters checkUserparameters = new DynamicParameters();
        checkUserparameters.Add("@UserName" ,registerationRequestDto.UserName);

        LocalUser? localUser = await _sP_Call.OneRecordAsync<LocalUser>(
            StoreProcedureNames.SpGetLocalUserByUserName, checkUserparameters);
        if (localUser != null)
        {
            return new AuthenticationResult
            {
                LocalUserDto = null,
                Token = string.Empty
            };
        }

        // create user
        LocalUserDto localUserDto = new LocalUserDto
        {
            FirstName = registerationRequestDto.FirstName,
            LastName = registerationRequestDto.LastName,
            UserName = registerationRequestDto.LastName,
            Email = registerationRequestDto.Email,
            PhoneNumber = registerationRequestDto.PhoneNumber,
            Role = registerationRequestDto.Role ?? "Customer"
        };

        DynamicParameters AddUserparameters = new DynamicParameters();
        AddUserparameters.Add("@FirstName", localUserDto.FirstName);
        AddUserparameters.Add("@LastName", localUserDto.LastName);
        AddUserparameters.Add("@UserName", localUserDto.UserName);
        AddUserparameters.Add("@Email", localUserDto.Email);
        AddUserparameters.Add("@PasswordHash", BCrypt.Net.BCrypt.HashPassword(registerationRequestDto.Password));
        AddUserparameters.Add("@PhoneNumber", localUserDto.PhoneNumber);
        AddUserparameters.Add("@Role", localUserDto.Role);
        AddUserparameters.Add("@CreatedAt", DateTime.Now);
        AddUserparameters.Add("@UpdatedAt", DateTime.Now);
        AddUserparameters.Add("@LocalUserId", DbType.Int32, direction: ParameterDirection.Output);

        await _sP_Call.ExecuteAsync(StoreProcedureNames.SpCreateLocalUser, AddUserparameters);

        localUserDto.LocalUserId = AddUserparameters.Get<int>("@LocalUserId");

        // create JWT token
        string token = _jwtTokenGenerator.GenerateTokenAsync(localUserDto);

        AuthenticationResult authenticationResult = new AuthenticationResult
        {
            LocalUserDto = localUserDto,
            Token = token
        };

        return authenticationResult;
    }

    public async Task<AuthenticationResult> LoginAsync(LoginRequestDto loginRequestDto)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@UserName", loginRequestDto.UserName);

        LocalUser? localUser = await _sP_Call.OneRecordAsync<LocalUser>(
            StoreProcedureNames.SpGetLocalUserByUserName, parameters);
        if (localUser == null)
        {
            return new AuthenticationResult
            {
                LocalUserDto = null,
                Token = string.Empty
            };
        }

        bool verified = BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, localUser.PasswordHash);
        if (!verified)
        {
            return new AuthenticationResult
            {
                LocalUserDto = null,
                Token = string.Empty
            };
        }

        LocalUserDto localUserDto = new LocalUserDto
        {
            LocalUserId = localUser.LocalUserId,
            FirstName = localUser.FirstName,
            LastName = localUser.LastName,
            UserName = localUser.UserName,
            PhoneNumber = localUser.PhoneNumber,
            Email = localUser.Email,
            Role = localUser.Role,
            ImageUrl = localUser.ImageUrl,
            Address = localUser.Address,
            Ward = localUser.Ward,
            District = localUser.District,
            City = localUser.City
        };

        // generate JWT token
        string token = _jwtTokenGenerator.GenerateTokenAsync(localUserDto);

        AuthenticationResult authenticationResult = new AuthenticationResult
        {
            LocalUserDto = localUserDto,
            Token = token
        };

        return authenticationResult;
    }
}