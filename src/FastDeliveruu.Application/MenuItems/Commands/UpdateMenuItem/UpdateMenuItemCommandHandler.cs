using CloudinaryDotNet.Actions;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.MenuItems.Commands.UpdateMenuItem;

public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, Result>
{
    private readonly ICacheService _cacheService;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;

    public UpdateMenuItemCommandHandler(
        IMenuItemRepository menuItemRepository,
        IGenreRepository genreRepository,
        IRestaurantRepository restaurantRepository,
        IMapper mapper,
        IFileStorageServices fileStorageServices,
        ICacheService cacheService)
    {
        _menuItemRepository = menuItemRepository;
        _genreRepository = genreRepository;
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _fileStorageServices = fileStorageServices;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        Genre? genre = await _genreRepository.GetAsync(request.GenreId);
        if (genre == null)
        {
            string message = "Genre not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        Restaurant? restaurant = await _restaurantRepository.GetAsync(request.RestaurantId);
        if (restaurant == null)
        {
            string message = "Restaurant not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        MenuItem? menuItem = await _menuItemRepository.GetAsync(request.MenuItemId);
        if (menuItem == null)
        {
            string message = "MenuItem not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        _mapper.Map(request, menuItem);

        if (request.ImageFile != null)
        {
            DeletionResult deletionResult = await _fileStorageServices.DeleteImageAsync(menuItem.PublicId);
            if (deletionResult.Error != null)
            {
                string message = deletionResult.Error.Message;
                Log.Warning($"{request.GetType().Name} - {message} - {request}");
                return Result.Fail(new BadRequestError(message));
            }

            UploadResult uploadResult = await _fileStorageServices
                .UploadImageAsync(request.ImageFile, UploadPath.MenuItemImageUploadPath);
            if (uploadResult.Error != null)
            {
                string message = uploadResult.Error.Message;
                Log.Warning($"{request.GetType().Name} - {message} - {request}");
                return Result.Fail(new BadRequestError(message));
            }

            menuItem.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
            menuItem.PublicId = uploadResult.PublicId;
        }
        menuItem.UpdatedAt = DateTime.Now;

        await _menuItemRepository.UpdateAsync(menuItem);

        await _cacheService.RemoveAsync($"{CacheConstants.MenuItem}-{request.MenuItemId}", cancellationToken);

        return Result.Ok();
    }
}
