using CloudinaryDotNet.Actions;
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

namespace FastDeliveruu.Application.MenuItems.Commands.CreateMenuItem;

public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Result<MenuItemDto>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateMenuItemCommandHandler> _logger;

    public CreateMenuItemCommandHandler(
        IFileStorageServices fileStorageServices,
        IMapper mapper,
        ILogger<CreateMenuItemCommandHandler> logger,
        IDateTimeProvider dateTimeProvider,
        IFastDeliveruuUnitOfWork unitOfWork,
        ICacheService cacheService)
    {
        _fileStorageServices = fileStorageServices;
        _mapper = mapper;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result<MenuItemDto>> Handle(
        CreateMenuItemCommand request,
        CancellationToken cancellationToken)
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

        MenuItem? menuItem = await _unitOfWork.MenuItems
            .GetWithSpecAsync(new MenuItemExistInRestaurantSpecification(request.RestaurantId, request.Name), true);
        if (menuItem != null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.MenuItemDuplicate} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.MenuItemDuplicate));
        }

        menuItem = _mapper.Map<MenuItem>(request);
        menuItem.Id = Guid.NewGuid();

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

        menuItem.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        var menuItemInventory = new MenuItemInventory
        {
            Id = Guid.NewGuid(),
            MenuItemId = menuItem.Id,
            QuantityAvailable = request.QuantityAvailable,
            QuantityReserved = request.QuantityReserved,
            CreatedAt = _dateTimeProvider.VietnamDateTimeNow
        };
        menuItem.MenuItemInventories.Add(menuItemInventory);

        _unitOfWork.MenuItems.Add(menuItem);
        await _unitOfWork.SaveChangesAsync();
        await _cacheService.RemoveByPrefixAsync(CacheConstants.MenuItems, cancellationToken);

        return _mapper.Map<MenuItemDto>(menuItem);
    }
}
