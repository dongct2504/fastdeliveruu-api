using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Authentication.Queries.UserLogin;

public class UserLoginQueryHandler : IRequestHandler<UserLoginQuery, Result<UserAuthenticationResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<UserLoginQueryHandler> _logger;
    private readonly IMapper _mapper;

    public UserLoginQueryHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IMapper mapper,
        IJwtTokenGenerator jwtTokenGenerator,
        IDateTimeProvider dateTimeProvider,
        ILogger<UserLoginQueryHandler> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<Result<UserAuthenticationResponse>> Handle(UserLoginQuery request,
        CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            string message = "The username is incorrect.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        bool isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        if (!isEmailConfirmed)
        {
            string message = "The email has not been confirmed yet.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!signInResult.Succeeded)
        {
            string message = "The password is incorrect.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        // generate JWT token
        string token = await _jwtTokenGenerator.GenerateTokenForUserAsync(user);

        _logger.LogInformation($"User {user.UserName} login at: {_dateTimeProvider.VietnamDateTimeNow:dd/MM/yyyy hh:mm tt}.");

        return _mapper.Map<UserAuthenticationResponse>((user, token));
    }
}