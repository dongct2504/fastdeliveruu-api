using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FastDeliveruu.Application.Restaurants.Queries.GetRestaurantById;

public class GetRestaurantByIdQueryHandler : IRequestHandler<GetRestaurantByIdQuery, Result<RestaurantDetailDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetRestaurantByIdQueryHandler(
        ICacheService cacheService,
        FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
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

        RestaurantDetailDto? restaurantDetailDto = await _dbContext.Restaurants
            .Where(r => r.Id == request.Id)
            .AsNoTracking()
            .ProjectToType<RestaurantDetailDto>()
            .FirstOrDefaultAsync(cancellationToken);
        if (restaurantDetailDto == null)
        {
            string message = "Restaurant not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<RestaurantDetailDto>(new NotFoundError(message));
        }

        await _cacheService.SetAsync(
            key,
            restaurantDetailDto,
            CacheOptions.DefaultExpiration,
            cancellationToken);

        return restaurantDetailDto;
    }
}