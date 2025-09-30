using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.AppUserDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Authentication.Queries.UserLogin;

public class UserLoginQueryHandler : IRequestHandler<UserLoginQuery, Result<UserAuthenticationResponse>>
{
    private readonly FastDeliveruuDbContext _dbContext;
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
        ILogger<UserLoginQueryHandler> logger,
        FastDeliveruuDbContext dbContext)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _mapper = mapper;
        _jwtTokenGenerator = jwtTokenGenerator;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<UserAuthenticationResponse>> Handle(UserLoginQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .Include(u => u.AppUserRoles)
                .ThenInclude(ur => ur.AppRole)
            .FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken: cancellationToken);

        if (user == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WrongUserName} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WrongUserName));
        }

        bool isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        if (!isEmailConfirmed)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.EmailYetConfirmed} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.EmailYetConfirmed));
        }

        SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (signInResult.IsLockedOut)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.Lockout} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.Lockout));
        }
        else if (!signInResult.Succeeded)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WrongPassword} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WrongPassword));
        }

        // generate JWT token
        string token = await _jwtTokenGenerator.GenerateTokenForUserAsync(user);

        _logger.LogInformation($"User {user.UserName} login at: {_dateTimeProvider.VietnamDateTimeNow:dd/MM/yyyy hh:mm tt}.");

        return _mapper.Map<UserAuthenticationResponse>((user, token));
    }
}