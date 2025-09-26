using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.MenuItems;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuItems.Commands.UpdateMenuItem;

public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, Result>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateMenuItemCommandHandler> _logger;

    public UpdateMenuItemCommandHandler(
        IMapper mapper,
        IFileStorageServices fileStorageServices,
        ILogger<UpdateMenuItemCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IFastDeliveruuUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _fileStorageServices = fileStorageServices;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        Genre? genre = await _unitOfWork.Genres.GetAsync(request.GenreId);
        if (genre == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.GenreNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.GenreNotFound));
        }

        Restaurant? restaurant = await _unitOfWork.Restaurants.GetAsync(request.RestaurantId);
        if (restaurant == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.RestaurantNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.RestaurantNotFound));
        }

        MenuItem? menuItem = await _unitOfWork.MenuItems.GetAsync(request.Id);
        if (menuItem == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuItemNotFound} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuItemNotFound));
        }

        _mapper.Map(request, menuItem);

        if (request.ImageFile != null)
        {
            DeletionResult deletionResult = await _fileStorageServices.DeleteImageAsync(menuItem.PublicId);
            if (deletionResult.Error != null)
            {
                string message = deletionResult.Error.Message;
                _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
                return Result.Fail(new BadRequestError(message));
            }

            UploadResult uploadResult = await _fileStorageServices
                .UploadImageAsync(request.ImageFile, UploadPath.MenuItemImageUploadPath);
            if (uploadResult.Error != null)
            {
                string message = uploadResult.Error.Message;
                _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
                return Result.Fail(new BadRequestError(message));
            }

            menuItem.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
            menuItem.PublicId = uploadResult.PublicId;
        }
        menuItem.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;

        var inventory = await _unitOfWork.MenuItemInventories
            .GetWithSpecAsync(new MenuItemInventoryByMenuItemIdSpecification(menuItem.Id));
        if (inventory == null)
        {
            inventory = new MenuItemInventory
            {
                Id = Guid.NewGuid(),
                MenuItemId = menuItem.Id,
                QuantityAvailable = request.QuantityAvailable,
                QuantityReserved = request.QuantityReserved,
                CreatedAt = _dateTimeProvider.VietnamDateTimeNow
            };
            _unitOfWork.MenuItemInventories.Add(inventory);
        }
        else
        {
            inventory.QuantityAvailable = request.QuantityAvailable;
            inventory.QuantityReserved = request.QuantityReserved;
            inventory.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;
        }

        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
