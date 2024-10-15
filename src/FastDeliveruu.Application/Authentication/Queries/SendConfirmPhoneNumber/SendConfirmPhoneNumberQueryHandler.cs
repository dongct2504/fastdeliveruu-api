using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Common.Helpers;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Authentication.Queries.SendConfirmPhoneNumber;

public class SendConfirmPhoneNumberQueryHandler : IRequestHandler<SendConfirmPhoneNumberQuery, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ISmsSenderService _smsSenderService;
    private readonly ICacheService _cacheService;
    private readonly ILogger<SendConfirmPhoneNumberQueryHandler> _logger;

    public SendConfirmPhoneNumberQueryHandler(
        ISmsSenderService smsSenderService,
        ICacheService cacheService,
        ILogger<SendConfirmPhoneNumberQueryHandler> logger,
        UserManager<AppUser> userManager)
    {
        _smsSenderService = smsSenderService;
        _cacheService = cacheService;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<Result> Handle(SendConfirmPhoneNumberQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
        {
            string message = "User not found";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        string otpCode = Utils.GenerateOtpCode();

        string key = $"{CacheConstants.OtpCode}-{request.UserId}";
        await _cacheService.SetAsync(key, otpCode, CacheOptions.OtpExpiration, cancellationToken);

        string otpMessage = $"Mã OTP của bạn là: ${otpCode}";

        var response = await _smsSenderService.SendSmsAsync(request.UserId, request.PhoneNumber, otpMessage);
        if (!response.Messages[0].Status.Equals("0"))
        {
            string message = response.Messages[0].ErrorText;
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        return Result.Ok();
    }
}
