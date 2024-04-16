using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FluentResults;

namespace FastDeliveruu.Application.Services;

public class AuthenticationServices : IAuthenticationServices
{
    private readonly ILocalUserServices _localUserServices;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public AuthenticationServices(IJwtTokenGenerator jwtTokenGenerator,
        ILocalUserServices localUserServices,
        IMapper mapper)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _localUserServices = localUserServices;
        _mapper = mapper;
    }

    public async Task<Result<AuthenticationResult>> RegisterAsync(
        RegisterationRequestDto registerationRequestDto)
    {
        LocalUser localUser = _mapper.Map<LocalUser>(registerationRequestDto);
        localUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerationRequestDto.Password);
        localUser.CreatedAt = DateTime.Now;
        localUser.UpdatedAt = DateTime.Now;

        Result<Guid> result = await _localUserServices.AddUserAsync(localUser);
        if (result.IsFailed)
        {
            return Result.Fail<AuthenticationResult>(
                new DuplicateError(result.Errors[0].Message));
        }

        localUser.LocalUserId = result.Value;

        string token = _jwtTokenGenerator.GenerateEmailConfirmationToken(localUser);

        LocalUserDto localUserDto = _mapper.Map<LocalUserDto>(localUser);

        AuthenticationResult authenticationResult = new AuthenticationResult
        {
            LocalUserDto = localUserDto,
            Token = token
        };

        return authenticationResult;
    }

    public async Task<Result<AuthenticationResult>> LoginAsync(LoginRequestDto loginRequestDto)
    {
        Result<LocalUser> localUserResult =
            await _localUserServices.GetLocalUserByUserNameAsync(loginRequestDto.UserName);
        if (localUserResult.IsFailed)
        {
            return Result.Fail<AuthenticationResult>(
                new BadRequestError("The username is incorrect."));
        }

        LocalUser localUser = localUserResult.Value;

        bool verified = BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, localUser.PasswordHash);
        if (!verified)
        {
            return Result.Fail<AuthenticationResult>(
                new BadRequestError("The password is incorrect."));
        }

        bool isConfirmEmail = localUserResult.Value.IsConfirmEmail;
        if (!isConfirmEmail)
        {
            return Result.Fail<AuthenticationResult>(
                new BadRequestError("The email is not yet confirmed."));
        }

        // generate JWT token
        string token = _jwtTokenGenerator.GenerateToken(localUser);

        LocalUserDto localUserDto = _mapper.Map<LocalUserDto>(localUser);

        AuthenticationResult authenticationResult = new AuthenticationResult
        {
            LocalUserDto = localUserDto,
            Token = token
        };

        return authenticationResult;
    }

    public async Task<Result<bool>> IsEmailConfirm(string token, string email)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

        // Retrieve the user ID and email from the token
        Claim? userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");
        Claim? emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);

        if (userIdClaim == null || emailClaim == null)
        {
            return Result.Fail<bool>(new BadRequestError("Invalid token format."));
        }

        // Retrieve the user based on email
        Result<LocalUser> getLocalUserResult =
            await _localUserServices.GetLocalUserByEmailAsync(email);
        if (getLocalUserResult.IsFailed)
        {
            return Result.Fail<bool>(new NotFoundError("User not found."));
        }

        // Verify the email confirmation token
        if (emailClaim.Value != email)
        {
            return Result.Fail<bool>(new BadRequestError("Email not match."));
        }

        // Update the user's email confirmation status
        getLocalUserResult.Value.IsConfirmEmail = true;
        getLocalUserResult.Value.UpdatedAt = DateTime.Now;

        await _localUserServices.UpdateUserAsync(Guid.Parse(userIdClaim.Value), getLocalUserResult.Value);

        return true;
    }
}