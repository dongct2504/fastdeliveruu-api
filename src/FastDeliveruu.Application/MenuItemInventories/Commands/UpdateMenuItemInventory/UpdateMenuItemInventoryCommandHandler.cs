using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.MenuItems;
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
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuItemNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuItemNotFound));
        }

        MenuItemInventory? menuItemInventory;
        if (request.Id == null) // add
        {
            menuItemInventory = await _unitOfWork.MenuItemInventories.GetWithSpecAsync(new MenuItemInventoryByMenuItemIdSpecification(menuItem.Id), true);
            if (menuItemInventory != null)
            {
                _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuItemInventoryDuplicate} - {request}");
                return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuItemInventoryDuplicate));
            }

            menuItemInventory = _mapper.Map<MenuItemInventory>(request);
            menuItemInventory.Id = Guid.NewGuid();
            menuItemInventory.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;
            _unitOfWork.MenuItemInventories.Add(menuItemInventory);
        }
        else // update
        {
            menuItemInventory = await _unitOfWork.MenuItemInventories.GetAsync(request.Id.Value);
            if (menuItemInventory == null)
            {
                _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuItemInventoryNotFound} - {request}");
                return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuItemInventoryNotFound));
            }

            _mapper.Map(request, menuItemInventory);
            menuItemInventory.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;
        }

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MenuItemInventoryDto>(menuItemInventory);
    }
}
