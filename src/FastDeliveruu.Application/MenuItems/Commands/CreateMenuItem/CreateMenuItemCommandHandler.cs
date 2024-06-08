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
using Serilog;

namespace FastDeliveruu.Application.MenuItems.Commands.CreateMenuItem;

public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Result<MenuItemDto>>
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;

    public CreateMenuItemCommandHandler(
        IMenuItemRepository menuItemRepository,
        IGenreRepository genreRepository,
        IRestaurantRepository restaurantRepository,
        IFileStorageServices fileStorageServices,
        IMapper mapper)
    {
        _menuItemRepository = menuItemRepository;
        _genreRepository = genreRepository;
        _restaurantRepository = restaurantRepository;
        _fileStorageServices = fileStorageServices;
        _mapper = mapper;
    }

    public async Task<Result<MenuItemDto>> Handle(
        CreateMenuItemCommand request,
        CancellationToken cancellationToken)
    {
        Genre? genre = await _genreRepository.GetAsync(request.GenreId);
        if (genre == null)
        {
            string message = "Genre not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<MenuItemDto>(new NotFoundError(message));
        }

        Restaurant? restaurant = await _restaurantRepository.GetAsync(request.RestaurantId);
        if (restaurant == null)
        {
            string message = "Restaurant not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<MenuItemDto>(new NotFoundError(message));
        }

        var spec = new MenuItemExistInRestaurantSpecification(request.RestaurantId, request.Name);

        MenuItem? menuItem = await _menuItemRepository.GetWithSpecAsync(spec, asNoTracking: true);
        if (menuItem != null)
        {
            string message = "MenuItem is already exist.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<MenuItemDto>(new DuplicateError(message));
        }

        menuItem = _mapper.Map<MenuItem>(request);
        menuItem.MenuItemId = Guid.NewGuid();

        if (request.ImageFile != null)
        {
            string? fileNameWithExtension = await _fileStorageServices.UploadImageAsync(request.ImageFile,
                UploadPath.MenuItemImageUploadPath);
            menuItem.ImageUrl = UploadPath.MenuItemImageUploadPath + fileNameWithExtension;
        }

        menuItem.CreatedAt = DateTime.Now;
        menuItem.UpdatedAt = DateTime.Now;

        await _menuItemRepository.AddAsync(menuItem);

        return _mapper.Map<MenuItemDto>(menuItem);
    }
}
