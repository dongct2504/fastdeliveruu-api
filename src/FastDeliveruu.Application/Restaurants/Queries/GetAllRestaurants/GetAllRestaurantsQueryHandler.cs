using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
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

        if (!string.IsNullOrEmpty(request.RestaurantParams.Search))
        {
            string lowerCaseSearch = request.RestaurantParams.Search.ToLower();
            restaurantsQuery = restaurantsQuery
                .Where(r => EF.Functions.Like(r.Name.ToLower(), $"%{lowerCaseSearch}%"));
        }

        if (!string.IsNullOrEmpty(request.RestaurantParams.Sort))
        {
            switch (request.RestaurantParams.Sort)
            {
                case RestaurantSortConstants.LatestUpdateDesc:
                    restaurantsQuery = restaurantsQuery.OrderByDescending(r => r.UpdatedAt);
                    break;
                case RestaurantSortConstants.Name:
                    restaurantsQuery = restaurantsQuery.OrderBy(r => r.Name);
                    break;
            }
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