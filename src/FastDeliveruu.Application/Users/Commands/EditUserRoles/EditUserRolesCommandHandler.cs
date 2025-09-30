using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Users.Commands.EditUserRoles;

public class EditUserRolesCommandHandler : IRequestHandler<EditUserRolesCommand, Result<string[]>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICacheService _cacheService;
    private readonly ILogger<EditUserRolesCommandHandler> _logger;

    public EditUserRolesCommandHandler(UserManager<AppUser> userManager, ILogger<EditUserRolesCommandHandler> logger, ICacheService cacheService)
    {
        _userManager = userManager;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<Result<string[]>> Handle(EditUserRolesCommand request, CancellationToken cancellationToken)
    {
        string[] allowRoles = { RoleConstants.Admin, RoleConstants.Staff, RoleConstants.Shipper, RoleConstants.Customer };

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
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.AppUserNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.AppUserNotFound));
        }

        var userRoles = await _userManager.GetRolesAsync(appUser);

        var rolesToAdd = selectedRoles.Except(userRoles, StringComparer.OrdinalIgnoreCase);
        var addRolesResult = await _userManager.AddToRolesAsync(appUser, rolesToAdd);
        if (!addRolesResult.Succeeded)
        {
            var message = string.Join("\n", addRolesResult.Errors.Select(e => e.Description));
            _logger.LogWarning("{Command} - Failed to add roles - {Message}", request.GetType().Name, message);
            return Result.Fail(new BadRequestError(message));
        }

        var rolesToRemove = userRoles.Except(selectedRoles, StringComparer.OrdinalIgnoreCase);
        var removeRolesResult = await _userManager.RemoveFromRolesAsync(appUser, rolesToRemove);
        if (!removeRolesResult.Succeeded)
        {
            var message = string.Join("\n", removeRolesResult.Errors.Select(e => e.Description));
            _logger.LogWarning("{Command} - Failed to remove roles - {Message}", request.GetType().Name, message);
            return Result.Fail(new BadRequestError(message));
        }

        var updatedRoles = await _userManager.GetRolesAsync(appUser);
        await _cacheService.RemoveByPrefixAsync(CacheConstants.AppUsers, cancellationToken);
        await _cacheService.RemoveByPrefixAsync(CacheConstants.AppUsersWithRoles, cancellationToken);

        return updatedRoles.ToArray();
    }
}
