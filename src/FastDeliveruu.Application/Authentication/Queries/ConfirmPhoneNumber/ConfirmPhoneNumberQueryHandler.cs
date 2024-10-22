using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Authentication.Queries.ConfirmPhoneNumber;

public class ConfirmPhoneNumberQueryHandler : IRequestHandler<ConfirmPhoneNumberQuery, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ConfirmPhoneNumberQueryHandler> _logger;

    public ConfirmPhoneNumberQueryHandler(
        UserManager<AppUser> userManager,
        ICacheService cacheService,
        ILogger<ConfirmPhoneNumberQueryHandler> logger)
    {
        _userManager = userManager;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result> Handle(ConfirmPhoneNumberQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.AppUserNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.AppUserNotFound));
        }

        string key = $"{CacheConstants.OtpCode}-{request.UserId}";
        string? otpCodeCache = await _cacheService.GetAsync<string>(key, cancellationToken);
        if (otpCodeCache == null || otpCodeCache != request.OtpCode)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.OtpError} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.OtpError));
        }

        user.PhoneNumberConfirmed = true;
        await _userManager.UpdateAsync(user);

        await _cacheService.RemoveAsync(key, cancellationToken);

        return Result.Ok();
    }
}
