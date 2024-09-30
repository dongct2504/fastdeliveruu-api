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

namespace FastDeliveruu.Application.MenuVariantInventories.Commands.UpdateMenuVariantInventory;

public class UpdateMenuVariantInventoryCommandHandler : IRequestHandler<UpdateMenuVariantInventoryCommand, Result<MenuVariantInventoryDto>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateMenuVariantInventoryCommandHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateMenuVariantInventoryCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ILogger<UpdateMenuVariantInventoryCommandHandler> logger,
        IMapper mapper,
        IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<MenuVariantInventoryDto>> Handle(UpdateMenuVariantInventoryCommand request, CancellationToken cancellationToken)
    {
        MenuVariant? menuVariant = await _unitOfWork.MenuVariants.GetAsync(request.MenuVariantId);
        if (menuVariant == null)
        {
            string message = "MenuVariant not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        MenuVariantInventory? menuVariantInventory;
        if (request.Id == Guid.Empty) // add
        {
            menuVariantInventory = _mapper.Map<MenuVariantInventory>(request);
            menuVariantInventory.Id = Guid.NewGuid();
            menuVariantInventory.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;
            _unitOfWork.MenuVariantInventories.Add(menuVariantInventory);
        }
        else // update
        {
            menuVariantInventory = await _unitOfWork.MenuVariantInventories.GetAsync(request.Id);
            if (menuVariantInventory == null)
            {
                string message = "MenuVariant Inventory not found.";
                _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
                return Result.Fail(new NotFoundError(message));
            }

            _mapper.Map(request, menuVariantInventory);
            menuVariantInventory.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;
        }

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<MenuVariantInventoryDto>(menuVariantInventory);
    }
}
