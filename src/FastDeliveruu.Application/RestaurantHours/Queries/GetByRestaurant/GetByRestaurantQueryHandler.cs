using FastDeliveruu.Application.Common;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.RestaurantHours.Queries.GetByRestaurant;

public class GetByRestaurantQueryHandler : IRequestHandler<GetByRestaurantQuery, List<RestaurantHourDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public GetByRestaurantQueryHandler(
        ICacheService cacheService,
        FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<List<RestaurantHourDto>> Handle(GetByRestaurantQuery request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.RestaurantHours}-{request.RestaurantId}";
        List<RestaurantHourDto>? restaurantHourDtosCache = await _cacheService
            .GetAsync<List<RestaurantHourDto>>(key, cancellationToken);

        if (restaurantHourDtosCache != null)
        {
            return restaurantHourDtosCache;
        }

        IQueryable<RestaurantHour> restaurantHoursQuery = _dbContext.RestaurantHours.AsQueryable();

        restaurantHoursQuery = restaurantHoursQuery
            .Where(rh => rh.RestaurantId == request.RestaurantId);

        List<RestaurantHourDto> restaurantHourDtos = await restaurantHoursQuery
            .AsNoTracking()
            .ProjectToType<RestaurantHourDto>()
            .ToListAsync(cancellationToken);

        await _cacheService.SetAsync(key, restaurantHourDtos, CacheOptions.DefaultExpiration, cancellationToken);

        return restaurantHourDtos;
    }
}
