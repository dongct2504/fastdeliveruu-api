using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace FastDeliveruu.Application.Authentication.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICacheService _cacheService;

    public ResetPasswordCommandHandler(UserManager<AppUser> userManager, ICacheService cacheService)
    {
        _userManager = userManager;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return Result.Fail(new BadRequestError(ErrorMessageConstants.AppUserNotFound));
        }

        string key = $"{CacheConstants.ResetPasswordToken}-{user.Id}";
        string? encodedToken = await _cacheService.GetAsync<string>(key, cancellationToken);
        if (encodedToken == null)
        {
            return Result.Fail(new BadRequestError(ErrorMessageConstants.TokenNotFound));
        }

        string token;
        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken));
        }
        catch
        {
            return Result.Fail(new BadRequestError(ErrorMessageConstants.InvalidToken));
        }

        IdentityResult result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
        if (!result.Succeeded)
        {
            return Result.Fail(new BadRequestError(string.Join("\n", result.Errors.Select(e => e.Description))));
        }

        await _cacheService.RemoveAsync(key, cancellationToken);

        return Result.Ok();
    }
}
