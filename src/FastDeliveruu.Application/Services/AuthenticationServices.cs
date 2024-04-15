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
    private readonly IEmailSender _emailSender;
    private readonly IMapper _mapper;

    public AuthenticationServices(IJwtTokenGenerator jwtTokenGenerator,
        ILocalUserServices localUserServices,
        IMapper mapper,
        IEmailSender emailSender)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _localUserServices = localUserServices;
        _mapper = mapper;
        _emailSender = emailSender;
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
                new DuplicateError("The user is already existed."));
        }

        localUser.LocalUserId = result.Value;

        // create JWT token
        string token = _jwtTokenGenerator.GenerateTokenAsync(localUser);

        LocalUserDto localUserDto = _mapper.Map<LocalUserDto>(localUser);

        string receiver = localUser.Email;
        string subject = "this is a subject";
        string message = "this is a message";
        await _emailSender.SendEmailAsync(receiver, subject, message);

        AuthenticationResult authenticationResult = new AuthenticationResult
        {
            LocalUserDto = localUserDto,
            Token = token
        };

        return authenticationResult;
    }

    public async Task<Result<AuthenticationResult>> LoginAsync(LoginRequestDto loginRequestDto)
    {
        Result<LocalUser> localUserResult = await _localUserServices.GetLocalUserByUserNameAsync(loginRequestDto.UserName);
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
                new BadRequestError("The password is incorredt."));
        }

        // generate JWT token
        string token = _jwtTokenGenerator.GenerateTokenAsync(localUser);

        LocalUserDto localUserDto = _mapper.Map<LocalUserDto>(localUser);

        AuthenticationResult authenticationResult = new AuthenticationResult
        {
            LocalUserDto = localUserDto,
            Token = token
        };

        return authenticationResult;
    }
}