using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly ILocalUserRepository _localUserRepository;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;

    public UpdateUserCommandHandler(
        ILocalUserRepository localUserRepository,
        IFileStorageServices fileStorageServices,
        IMapper mapper)
    {
        _localUserRepository = localUserRepository;
        _mapper = mapper;
        _fileStorageServices = fileStorageServices;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        LocalUser? localUser = await _localUserRepository.GetAsync(request.LocalUserId);
        if (localUser == null)
        {
            string message = "User not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _mapper.Map(request, localUser);

        if (request.ImageFile != null)
        {
            if (!string.IsNullOrEmpty(localUser.PublicId))
            {
                DeletionResult deletionResult = await _fileStorageServices.DeleteImageAsync(localUser.PublicId);
                if (deletionResult.Error != null)
                {
                    string message = deletionResult.Error.Message;
                    Log.Warning($"{request.GetType().Name} - {message} - {request}");
                    return Result.Fail(new BadRequestError(message));
                }
            }

            UploadResult uploadResult = await _fileStorageServices.UploadImageAsync(
                request.ImageFile, UploadPath.UserImageUploadPath);

            localUser.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
            localUser.PublicId = uploadResult.PublicId;
        }
        localUser.Role ??= RoleConstants.Customer;
        localUser.UpdatedAt = DateTime.Now;

        await _localUserRepository.UpdateAsync(localUser);

        return Result.Ok();
    }
}