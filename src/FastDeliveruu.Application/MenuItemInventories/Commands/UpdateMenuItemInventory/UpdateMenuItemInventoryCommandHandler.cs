using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuItemInventories.Commands.CreateMenuItemInventory;

public class UpdateMenuItemInventoryCommandHandler : IRequestHandler<UpdateMenuItemInventoryCommand, Result<MenuItemInventoryDto>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMenuItemInventoryCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateMenuItemInventoryCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<UpdateMenuItemInventoryCommandHandler> logger,
        IMapper mapper,
        IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<MenuItemInventoryDto>> Handle(UpdateMenuItemInventoryCommand request, CancellationToken cancellationToken)
    {
        MenuItem? menuItem = await _unitOfWork.MenuItems.GetAsync(request.MenuItemId);
        if (menuItem == null)
        {
            string message = "MenuItem not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        MenuItemInventory? menuItemInventory;
        if (request.Id == Guid.Empty) // add
        {
            menuItemInventory = _mapper.Map<MenuItemInventory>(request);
            menuItemInventory.Id = Guid.NewGuid();
            menuItemInventory.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;
            _unitOfWork.MenuItemInventories.Add(menuItemInventory);
        }
        else // update
        {
            menuItemInventory = await _unitOfWork.MenuItemInventories.GetAsync(request.Id);
            if (menuItemInventory == null)
            {
                string message = "MenuItem Inventory not found.";
                _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
                return Result.Fail(new NotFoundError(message));
            }

            _mapper.Map(request, menuItemInventory);
            menuItemInventory.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;
        }

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MenuItemInventoryDto>(menuItemInventory);
    }
}
