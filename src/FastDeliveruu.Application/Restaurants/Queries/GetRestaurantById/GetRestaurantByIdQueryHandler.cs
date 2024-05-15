using FastDeliveruu.Application.Common;
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

namespace FastDeliveruu.Application.Restaurants.Queries.GetRestaurantById;

public class GetRestaurantByIdQueryHandler : IRequestHandler<GetRestaurantByIdQuery, Result<RestaurantDetailDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMapper _mapper;

    public GetRestaurantByIdQueryHandler(
        IRestaurantRepository restaurantRepository,
        IMapper mapper,
        ICacheService cacheService)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<Result<RestaurantDetailDto>> Handle(
        GetRestaurantByIdQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Restaurant}-{request.Id}";

        RestaurantDetailDto? restaurantCache = await _cacheService
            .GetAsync<RestaurantDetailDto>(key, cancellationToken);
        if (restaurantCache != null)
        {
            return restaurantCache;
        }

        QueryOptions<Restaurant> options = new QueryOptions<Restaurant>
        {
            SetIncludes = "MenuItems",
            Where = r => r.RestaurantId == request.Id
        };
        Restaurant? restaurant = await _restaurantRepository.GetAsync(options, asNoTracking: true);
        if (restaurant == null)
        {
            string message = "Restaurant not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<RestaurantDetailDto>(new NotFoundError(message));
        }

        restaurantCache = _mapper.Map<RestaurantDetailDto>(restaurant);

        await _cacheService.SetAsync(key, restaurantCache, CacheOptions.DefaultExpiration, cancellationToken);

        return restaurantCache;
    }
}