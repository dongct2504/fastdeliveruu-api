using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuVariantInventories.Commands.DeleteMenuVariantInventory;

public class DeleteMenuVariantInventoryCommandHandler : IRequestHandler<DeleteMenuVariantInventoryCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteMenuVariantInventoryCommandHandler> _logger;

    public DeleteMenuVariantInventoryCommandHandler(IFastDeliveruuUnitOfWork unitOfWork, ILogger<DeleteMenuVariantInventoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteMenuVariantInventoryCommand request, CancellationToken cancellationToken)
    {
        MenuVariantInventory? menuVariantInventory = await _unitOfWork.MenuVariantInventories.GetAsync(request.Id);
        if (menuVariantInventory == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuVariantInventoryNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuVariantInventoryNotFound));
        }

        _unitOfWork.MenuVariantInventories.Delete(menuVariantInventory);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
