using FastDeliveruu.Application.Common;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
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
        string key = $"{CacheConstants.Restaurants}-{request.RestaurantParams}";

        PagedList<RestaurantDto>? paginationResponseCache = await _cacheService
            .GetAsync<PagedList<RestaurantDto>>(key, cancellationToken);
        if (paginationResponseCache != null)
        {
            return paginationResponseCache;
        }

        IQueryable<Restaurant> restaurantsQuery = _dbContext.Restaurants.AsQueryable();

        restaurantsQuery = restaurantsQuery
            .Where(c => string.IsNullOrEmpty(request.RestaurantParams.Search) || c.Name.ToLower().Contains(request.RestaurantParams.Search.ToLower()));

        if (!string.IsNullOrEmpty(request.RestaurantParams.Sort))
        {
            switch (request.RestaurantParams.Sort)
            {
                case SortConstants.OldestUpdateAsc:
                    restaurantsQuery = restaurantsQuery.OrderBy(r => r.UpdatedAt);
                    break;
                case SortConstants.LatestUpdateDesc:
                    restaurantsQuery = restaurantsQuery.OrderByDescending(r => r.UpdatedAt);
                    break;
                case SortConstants.NameAsc:
                    restaurantsQuery = restaurantsQuery.OrderBy(r => r.Name);
                    break;
                case SortConstants.NameDesc:
                    restaurantsQuery = restaurantsQuery.OrderByDescending(r => r.Name);
                    break;
            }
        }
        else
        {
            restaurantsQuery = restaurantsQuery.OrderBy(r => r.Name);
        }

        PagedList<RestaurantDto> paginationResponse = new PagedList<RestaurantDto>
        {
            PageNumber = request.RestaurantParams.PageNumber,
            PageSize = request.RestaurantParams.PageSize,
            TotalRecords = await restaurantsQuery.CountAsync(cancellationToken),
            Items = await restaurantsQuery
                .AsNoTracking()
                .ProjectToType<RestaurantDto>()
                .Skip((request.RestaurantParams.PageNumber - 1) * request.RestaurantParams.PageSize)
                .Take(request.RestaurantParams.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, paginationResponse, CacheOptions.DefaultExpiration, cancellationToken);

        return paginationResponse;
    }
}