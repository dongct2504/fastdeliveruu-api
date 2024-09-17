using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.MenuItems;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.MenuItems.Commands.CreateMenuItem;

public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Result<MenuItemDto>>
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateMenuItemCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateMenuItemCommandHandler(
        IMenuItemRepository menuItemRepository,
        IGenreRepository genreRepository,
        IRestaurantRepository restaurantRepository,
        IFileStorageServices fileStorageServices,
        IMapper mapper,
        ILogger<CreateMenuItemCommandHandler> logger,
        IDateTimeProvider dateTimeProvider)
    {
        _menuItemRepository = menuItemRepository;
        _genreRepository = genreRepository;
        _restaurantRepository = restaurantRepository;
        _fileStorageServices = fileStorageServices;
        _mapper = mapper;
        _logger = logger;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<MenuItemDto>> Handle(
        CreateMenuItemCommand request,
        CancellationToken cancellationToken)
    {
        Genre? genre = await _genreRepository.GetAsync(request.GenreId);
        if (genre == null)
        {
            string message = "Genre not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        Restaurant? restaurant = await _restaurantRepository.GetAsync(request.RestaurantId);
        if (restaurant == null)
        {
            string message = "Restaurant not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        var spec = new MenuItemExistInRestaurantSpecification(request.RestaurantId, request.Name);

        MenuItem? menuItem = await _menuItemRepository.GetWithSpecAsync(spec, asNoTracking: true);
        if (menuItem != null)
        {
            string message = "MenuItem is already exist.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new DuplicateError(message));
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

        await _menuItemRepository.AddAsync(menuItem);

        return _mapper.Map<MenuItemDto>(menuItem);
    }
}
