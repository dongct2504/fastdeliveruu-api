using FastDeliveruu.Application.Common;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace FastDeliveruu.Application.Authentication.Queries.ForgotPassword;

public class ForgotPasswordQueryHandler : IRequestHandler<ForgotPasswordQuery, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICacheService _cacheService;
    private readonly IConfiguration _configuration;
    private readonly IMailNotificationService _mailNotificationService;
    private readonly ILogger<ForgotPasswordQueryHandler> _logger;

    public ForgotPasswordQueryHandler(UserManager<AppUser> userManager, ILogger<ForgotPasswordQueryHandler> logger, ICacheService cacheService, IConfiguration configuration, IMailNotificationService mailNotificationService)
    {
        _userManager = userManager;
        _logger = logger;
        _cacheService = cacheService;
        _configuration = configuration;
        _mailNotificationService = mailNotificationService;
    }

    public async Task<Result> Handle(ForgotPasswordQuery request, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Fail(new BadRequestError(ErrorMessageConstants.WrongEmail));
        }

        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
        string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        string key = $"{CacheConstants.ResetPasswordToken}-{user.Id}";
        await _cacheService.SetAsync(key, encodedToken, CacheOptions.ChangePasswordTokenExpiration, cancellationToken);

        string resetPasswordLink = $"{_configuration["AppSettings:RedirectUrl"]}/authen/reset-password?userId={user.Id}";
        await _mailNotificationService.SendResetPasswordEmailAsync(user.UserName, user.Email, resetPasswordLink);

        return Result.Ok();
    }
}
