using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FastDeliveruu.Application.Authentication.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;

    public ChangePasswordCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return Result.Fail(new BadRequestError(ErrorMessageConstants.AppUserNotFound));
        }

        IdentityResult result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return Result.Fail(new BadRequestError(string.Join("\n", result.Errors.Select(e => e.Description))));
        }

        return Result.Ok();
    }
}
