using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
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
    private readonly IEmailSender _emailSender;
    private readonly ICacheService _cacheService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ForgotPasswordQueryHandler> _logger;

    public ForgotPasswordQueryHandler(UserManager<AppUser> userManager, IEmailSender emailSender, ILogger<ForgotPasswordQueryHandler> logger, ICacheService cacheService, IConfiguration configuration)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _logger = logger;
        _cacheService = cacheService;
        _configuration = configuration;
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

        string subject = "Thay đổi mật khẩu";
        string message = $"Xác nhận thành công. Bạn vui lòng nhấn vào link sau đây để tiến hành thay đổi mật khẩu: {_configuration["AppSettings:ResetPasswordUrl"]}?userId={user.Id}";
        await _emailSender.SendEmailAsync(user.Email, subject, message);

        return Result.Ok();
    }
}
