using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;

namespace FastDeliveruu.Application.Services;

public class AuthenticationServices : IAuthenticationServices
{
    private readonly ILocalUserServices _localUserServices;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthenticationServices(IJwtTokenGenerator jwtTokenGenerator,
        ILocalUserServices localUserServices)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _localUserServices = localUserServices;
    }

    public async Task<Result<AuthenticationResult>> RegisterAsync(
        RegisterationRequestDto registerationRequestDto)
    {
        LocalUser localUser = new LocalUser
        {
            FirstName = registerationRequestDto.FirstName,
            LastName = registerationRequestDto.LastName,
            UserName = registerationRequestDto.UserName,
            Email = registerationRequestDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerationRequestDto.Password),
            PhoneNumber = registerationRequestDto.PhoneNumber,
            Role = registerationRequestDto.Role ?? "Customer",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        Result<Guid> result = await _localUserServices.AddUserAsync(localUser);
        if (result.IsFailed)
        {
            return Result.Fail<AuthenticationResult>(
                new DuplicateError("The user is already existed."));
        }

        localUser.LocalUserId = result.Value;

        // create JWT token
        string token = _jwtTokenGenerator.GenerateTokenAsync(localUser);

        LocalUserDto localUserDto = new LocalUserDto
        {
            LocalUserId = localUser.LocalUserId,
            FirstName = localUser.FirstName,
            LastName = localUser.LastName,
            UserName = localUser.UserName,
            Email = localUser.Email,
            PhoneNumber = localUser.PhoneNumber,
            Role = localUser.Role
        };

        AuthenticationResult authenticationResult = new AuthenticationResult
        {
            LocalUserDto = localUserDto,
            Token = token
        };

        return authenticationResult;
    }

    public async Task<Result<AuthenticationResult>> LoginAsync(LoginRequestDto loginRequestDto)
    {
        Result<LocalUser> localUserResult = await _localUserServices.GetLocalUser(loginRequestDto.UserName);
        if (localUserResult.IsFailed)
        {
            return Result.Fail<AuthenticationResult>(
                new NotFoundError("The username is incorrect."));
        }

        LocalUser localUser = localUserResult.Value;

        bool verified = BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, localUser.PasswordHash);
        if (!verified)
        {
            return Result.Fail<AuthenticationResult>(
                new NotFoundError("The password is incorredt."));
        }

        // generate JWT token
        string token = _jwtTokenGenerator.GenerateTokenAsync(localUser);

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

        AuthenticationResult authenticationResult = new AuthenticationResult
        {
            LocalUserDto = localUserDto,
            Token = token
        };

        return authenticationResult;
    }
}