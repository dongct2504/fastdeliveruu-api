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
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IFileStorageServices _fileStorageServices;

    public DeleteMenuItemCommandHandler(IMenuItemRepository menuItemRepository, IFileStorageServices fileStorageServices)
    {
        _menuItemRepository = menuItemRepository;
        _fileStorageServices = fileStorageServices;
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

        await _menuItemRepository.DeleteAsync(menuItem);

        await _fileStorageServices.DeleteImageAsync(menuItem.ImageUrl);

        return Result.Ok();
    }
}
