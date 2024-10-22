using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuItems.Commands.DeleteMenuItem;

public class DeleteMenuItemCommandHandler : IRequestHandler<DeleteMenuItemCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteMenuItemCommandHandler> _logger;
    private readonly IFileStorageServices _fileStorageServices;

    public DeleteMenuItemCommandHandler(
        IFileStorageServices fileStorageServices,
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<DeleteMenuItemCommandHandler> logger)
    {
        _fileStorageServices = fileStorageServices;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        MenuItem? menuItem = await _unitOfWork.MenuItems.GetAsync(request.Id);
        if (menuItem == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuItemNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuItemNotFound));
        }

        DeletionResult deletionResult = await _fileStorageServices.DeleteImageAsync(menuItem.PublicId);
        if (deletionResult.Error != null)
        {
            string message = deletionResult.Error.Message;
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        _unitOfWork.MenuItems.Delete(menuItem);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
