using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Users.Commands.EditUserRoles;

public class EditUserRolesCommandHandler : IRequestHandler<EditUserRolesCommand, Result<string[]>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<EditUserRolesCommandHandler> _logger;

    public EditUserRolesCommandHandler(UserManager<AppUser> userManager, ILogger<EditUserRolesCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result<string[]>> Handle(EditUserRolesCommand request, CancellationToken cancellationToken)
    {
        string[] allowRoles = { RoleConstants.Admin, RoleConstants.Customer, RoleConstants.Shipper, RoleConstants.Customer };

        string[] selectedRoles = request.Roles
            .Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(r => r.Trim().ToLower())
            .ToArray();

        foreach (string role in selectedRoles)
        {
            if (!allowRoles.Contains(role))
            {
                string message = $"Role: {role} is not allow.";
                _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
                return Result.Fail(new BadRequestError(message));
            }
        }

        AppUser? appUser = await _userManager.FindByIdAsync(request.Id.ToString());
        if (appUser == null)
        {
            string message = $"User not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        var userRoles = await _userManager.GetRolesAsync(appUser);

        var addRolesResult = await _userManager.AddToRolesAsync(appUser, selectedRoles.Except(userRoles));
        if (!addRolesResult.Succeeded)
        {
            string message = string.Join("\n", addRolesResult.Errors.Select(e => e.Description));
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        var removeRolesResult = await _userManager.RemoveFromRolesAsync(appUser, userRoles.Except(selectedRoles));
        if (!removeRolesResult.Succeeded)
        {
            string message = string.Join("\n", removeRolesResult.Errors.Select(e => e.Description));
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        return (await _userManager.GetRolesAsync(appUser)).ToArray();
    }
}
