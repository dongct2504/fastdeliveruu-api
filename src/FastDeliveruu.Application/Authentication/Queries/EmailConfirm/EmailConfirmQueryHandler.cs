using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Serilog;

namespace FastDeliveruu.Application.Authentication.Queries.EmailConfirm;

public class EmailConfirmQueryHandler : IRequestHandler<EmailConfirmQuery, Result>
{
    private readonly UserManager<AppUser> _userManager;

    public EmailConfirmQueryHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(EmailConfirmQuery request, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            string message = "User not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        var decodedToken = System.Text.Encoding.UTF8
            .GetString(WebEncoders.Base64UrlDecode(request.EnCodedToken));
        IdentityResult result = await _userManager.ConfirmEmailAsync(user, decodedToken);
        if (!result.Succeeded)
        {
            var errorMessages = result.Errors.Select(e => e.Description);

            string message = string.Join(" ", errorMessages);
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        return Result.Ok();
    }
}