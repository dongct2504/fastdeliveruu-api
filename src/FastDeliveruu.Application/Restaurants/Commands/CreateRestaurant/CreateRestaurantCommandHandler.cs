using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Restaurants.Commands.CreateRestaurant;

public class CreateRestaurantCommandHandler : IRequestHandler<CreateRestaurantCommand, Result<RestaurantDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IFileStorageServices _fileStorageServices;
    private readonly IMapper _mapper;

    public CreateRestaurantCommandHandler(
        IRestaurantRepository restaurantRepository,
        IMapper mapper,
        IFileStorageServices fileStorageServices,
        ICacheService cacheService)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _fileStorageServices = fileStorageServices;
        _cacheService = cacheService;
    }

    public async Task<Result<RestaurantDto>> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
    {
        QueryOptions<Restaurant> options = new QueryOptions<Restaurant>
        {
            Where = r => r.Name == request.Name && r.PhoneNumber == request.PhoneNumber
        };
        Restaurant? restaurant = await _restaurantRepository.GetAsync(options);
        if (restaurant != null)
        {
            string message = "Restaurant is already exist.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<RestaurantDto>(new DuplicateError(message));
        }

        restaurant = _mapper.Map<Restaurant>(request);
        restaurant.RestaurantId = Guid.NewGuid();

        if (request.ImageFile != null)
        {
            string? fileNameWithExtension = await _fileStorageServices.UploadImageAsync(request.ImageFile,
                UploadPath.RestaurantImageUploadPath);
            restaurant.ImageUrl = UploadPath.RestaurantImageUploadPath + fileNameWithExtension;
        }
        restaurant.CreatedAt = DateTime.Now;
        restaurant.UpdatedAt = DateTime.Now;

        try
        {
            await _restaurantRepository.AddAsync(restaurant);
        }
        catch
        {
            await _fileStorageServices.DeleteImageAsync(restaurant.ImageUrl);
            throw;
        }

        await _cacheService.RemoveByPrefixAsync(CacheConstants.Restaurants, cancellationToken);

        return _mapper.Map<RestaurantDto>(restaurant);
    }
}
