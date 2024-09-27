using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly ILogger<UpdateUserCommand> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(
        IFileStorageServices fileStorageServices,
        IMapper mapper,
        UserManager<AppUser> userManager,
        IDateTimeProvider dateTimeProvider,
        ILogger<UpdateUserCommand> logger)
    {
        _mapper = mapper;
        _fileStorageServices = fileStorageServices;
        _userManager = userManager;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = await _userManager.FindByIdAsync(request.Id.ToString());
        if (user == null)
        {
            string message = "User not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _mapper.Map(request, user);

        if (request.ImageFile != null)
        {
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
        }

        if (!string.IsNullOrEmpty(request.Role))
        {
            await _userManager.AddToRoleAsync(user, request.Role);
        }

        user.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;

        await _userManager.UpdateAsync(user);

        return Result.Ok();
    }
}