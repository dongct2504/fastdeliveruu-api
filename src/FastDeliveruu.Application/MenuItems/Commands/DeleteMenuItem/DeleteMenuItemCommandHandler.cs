﻿using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.MenuItems.Commands.DeleteMenuItem;

public class DeleteMenuItemCommandHandler : IRequestHandler<DeleteMenuItemCommand, Result>
{
    private readonly ICacheService _cacheService;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IFileStorageServices _fileStorageServices;

    public DeleteMenuItemCommandHandler(
        IMenuItemRepository menuItemRepository,
        IFileStorageServices fileStorageServices,
        ICacheService cacheService)
    {
        _menuItemRepository = menuItemRepository;
        _fileStorageServices = fileStorageServices;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        MenuItem? menuItem = await _menuItemRepository.GetAsync(request.Id);
        if (menuItem == null)
        {
            string message = "MenuItem not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        DeletionResult deletionResult = await _fileStorageServices.DeleteImageAsync(menuItem.PublicId);
        if (deletionResult.Error != null)
        {
            string message = deletionResult.Error.Message;
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        await _menuItemRepository.DeleteAsync(menuItem);

        await _cacheService.RemoveAsync($"{CacheConstants.MenuItem}-{request.Id}", cancellationToken);

        return Result.Ok();
    }
}
