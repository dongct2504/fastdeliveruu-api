using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace FastDeliveruu.Application.Authentication.Queries.Login;

public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<AuthenticationResponse>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public LoginQueryHandler(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IMapper mapper,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthenticationResponse>> Handle(LoginQuery request,
        CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            string message = "The username is incorrect.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        bool isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        if (!isEmailConfirmed)
        {
            string message = "The email has not been confirmed yet.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!signInResult.Succeeded)
        {
            string message = "The password is incorrect.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        // generate JWT token
        string token = await _jwtTokenGenerator.GenerateTokenAsync(user);

        Log.Information($"User login at: {DateTime.Now:dd/MM/yyyy hh:mm tt}.");

        return _mapper.Map<AuthenticationResponse>((user, token));
    }
}