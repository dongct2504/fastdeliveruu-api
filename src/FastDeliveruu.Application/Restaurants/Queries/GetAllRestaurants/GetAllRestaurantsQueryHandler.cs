using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Restaurants.Queries.GetAllRestaurants;

public class GetAllRestaurantsQueryHandler : IRequestHandler<GetAllRestaurantsQuery, PagedList<RestaurantDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetAllRestaurantsQueryHandler(
        ICacheService cacheService,
        FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<PagedList<RestaurantDto>> Handle(
        GetAllRestaurantsQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Restaurants}-{request.PageNumber}-{request.PageSize}";

        PagedList<RestaurantDto>? paginationResponseCache = await _cacheService
            .GetAsync<PagedList<RestaurantDto>>(key, cancellationToken);
        if (paginationResponseCache != null)
        {
            return paginationResponseCache;
        }

        PagedList<RestaurantDto> paginationResponse = new PagedList<RestaurantDto>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = await _dbContext.Restaurants.CountAsync(cancellationToken),
            Items = await _dbContext.Restaurants
                .AsNoTracking()
                .ProjectToType<RestaurantDto>()
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, paginationResponse, CacheOptions.DefaultExpiration, cancellationToken);

        return paginationResponse;
    }
}