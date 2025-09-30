using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ICacheService _cacheService;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(
        IFileStorageServices fileStorageServices, UserManager<AppUser> userManager, ILogger<DeleteUserCommandHandler> logger, ICacheService cacheService)
    {
        _fileStorageServices = fileStorageServices;
        _userManager = userManager;
        _logger = logger;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.AppUserNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.AppUserNotFound));
        }

        if (!string.IsNullOrEmpty(user.PublicId))
        {
            DeletionResult deletionResult = await _fileStorageServices.DeleteImageAsync(user.PublicId);
            if (deletionResult.Error != null)
            {
                string message = deletionResult.Error.Message;
                _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
                return Result.Fail(new BadRequestError(message));
            }
        }

        await _userManager.DeleteAsync(user);
        await _cacheService.RemoveByPrefixAsync(CacheConstants.AppUsers, cancellationToken);
        await _cacheService.RemoveByPrefixAsync(CacheConstants.AppUsersWithRoles, cancellationToken);

        return Result.Ok();
    }
}