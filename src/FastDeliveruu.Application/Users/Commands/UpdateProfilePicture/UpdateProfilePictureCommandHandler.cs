using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Users.Commands.UpdateProfilePicture;

public class UpdateProfilePictureCommandHandler : IRequestHandler<UpdateProfilePictureCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly ILogger<UpdateProfilePictureCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateProfilePictureCommandHandler(UserManager<AppUser> userManager, IFileStorageServices fileStorageServices, ILogger<UpdateProfilePictureCommandHandler> logger, IDateTimeProvider dateTimeProvider)
    {
        _userManager = userManager;
        _fileStorageServices = fileStorageServices;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> Handle(UpdateProfilePictureCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.Users
            .Where(u => u.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
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

        UploadResult uploadResult = await _fileStorageServices.UploadImageAsync(
            request.ImageFile, UploadPath.UserImageUploadPath);

        user.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
        user.PublicId = uploadResult.PublicId;
        user.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;

        await _userManager.UpdateAsync(user);

        return Result.Ok();
    }
}
