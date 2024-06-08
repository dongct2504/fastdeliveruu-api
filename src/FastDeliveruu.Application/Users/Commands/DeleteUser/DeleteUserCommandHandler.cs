using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly ILocalUserRepository _localUserRepository;
    private readonly IFileStorageServices _fileStorageServices;

    public DeleteUserCommandHandler(
        ILocalUserRepository localUserRepository,
        IFileStorageServices fileStorageServices)
    {
        _localUserRepository = localUserRepository;
        _fileStorageServices = fileStorageServices;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        LocalUser? localUser = await _localUserRepository.GetAsync(request.Id);
        if (localUser == null)
        {
            string message = "User Not found";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

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

        await _localUserRepository.DeleteAsync(localUser);

        return Result.Ok();
    }
}