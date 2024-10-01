﻿using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuItemInventories.Commands.DeleteMenuItemInventory;

public class DeleteMenuItemInventoryCommandHandler : IRequestHandler<DeleteMenuItemInventoryCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteMenuItemInventoryCommandHandler> _logger;

    public DeleteMenuItemInventoryCommandHandler(IFastDeliveruuUnitOfWork unitOfWork, ILogger<DeleteMenuItemInventoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteMenuItemInventoryCommand request, CancellationToken cancellationToken)
    {
        MenuItemInventory? menuItemInventory = await _unitOfWork.MenuItemInventories.GetAsync(request.Id);
        if (menuItemInventory == null)
        {

            string message = "MenuItemInventory not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _unitOfWork.MenuItemInventories.Delete(menuItemInventory);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}