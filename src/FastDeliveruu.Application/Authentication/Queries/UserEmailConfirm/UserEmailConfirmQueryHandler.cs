using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Authentication.Queries.UserEmailConfirm;

public class UserEmailConfirmQueryHandler : IRequestHandler<UserEmailConfirmQuery, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<UserEmailConfirmQueryHandler> _logger;

    public UserEmailConfirmQueryHandler(UserManager<AppUser> userManager, ILogger<UserEmailConfirmQueryHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(UserEmailConfirmQuery request, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.AppUserNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.AppUserNotFound));
        }

        var decodedToken = System.Text.Encoding.UTF8
            .GetString(WebEncoders.Base64UrlDecode(request.EnCodedToken));
        IdentityResult result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            var errorMessages = result.Errors.Select(e => e.Description);

            string message = string.Join("\n", errorMessages);
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        return Result.Ok();
    }
}