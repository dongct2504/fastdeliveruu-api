using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FastDeliveruu.Application.Users.Commands.ToggleLock;

public class ToggleLockCommandHandler : IRequestHandler<ToggleLockCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;

    public ToggleLockCommandHandler(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(ToggleLockCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
            return Result.Fail(new BadRequestError(ErrorMessageConstants.AppUserNotFound));

        switch (request.Action)
        {
            case "lock":
                user.LockoutEnabled = true;
                user.LockoutEnd = DateTime.UtcNow.AddYears(100);
                break;
            case "unlock":
                user.LockoutEnd = null;
                break;
            default:
                return Result.Fail(new BadRequestError("Action không phù hợp"));
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return Result.Fail(new BadRequestError(string.Join("\n", result.Errors)));

        return Result.Ok();
    }
}
