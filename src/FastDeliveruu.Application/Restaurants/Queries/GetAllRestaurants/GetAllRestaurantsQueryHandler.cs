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
using FastDeliveruu.Common.Helpers;
using FastDeliveruu.Domain.Entities.Identity;
using FluentResults;
using FastDeliveruu.Application.Common.Errors;

namespace FastDeliveruu.Application.Restaurants.Queries.GetAllRestaurants;

public class GetAllRestaurantsQueryHandler : IRequestHandler<GetAllRestaurantsQuery, Result<PagedList<RestaurantDto>>>
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

    public async Task<Result<PagedList<RestaurantDto>>> Handle(
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

        bool hasUser = request.RestaurantParams.UserId != Guid.Empty;

        AppUser? user = null;
        AddressesCustomer? primaryAddress = null;

        if (hasUser)
        {
            user = await _dbContext.Users
                .Include(u => u.AddressesCustomers)
                .FirstOrDefaultAsync(u => u.Id == request.RestaurantParams.UserId, cancellationToken: cancellationToken);

            primaryAddress = user?
                .AddressesCustomers
                .FirstOrDefault(a => a.IsPrimary);
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
            restaurantsQuery = restaurantsQuery.OrderByDescending(mi => mi.UpdatedAt);
        }

        List<Restaurant> list = await restaurantsQuery
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        //Sort special case
        bool isNearest = request.RestaurantParams.Sort == SortConstants.Nearest;
        bool isFarthest = request.RestaurantParams.Sort == SortConstants.Farthest;

        if ((isNearest || isFarthest) && primaryAddress != null)
        {
            double lat = (double)primaryAddress.Latitude;
            double lng = (double)primaryAddress.Longitude;

            list = isNearest
                ? list.OrderBy(x =>
                    GeoHelper.CalculateDistance(
                        lat, lng,
                        (double)x.Latitude,
                        (double)x.Longitude))
                    .ToList()
                : list.OrderByDescending(x =>
                    GeoHelper.CalculateDistance(
                        lat, lng,
                        (double)x.Latitude,
                        (double)x.Longitude))
                    .ToList();
        }

        PagedList<RestaurantDto> paginationResponse = new PagedList<RestaurantDto>
        {
            PageNumber = request.RestaurantParams.PageNumber,
            PageSize = request.RestaurantParams.PageSize,
            TotalRecords = await restaurantsQuery.CountAsync(cancellationToken),
            Items = list
                .Skip((request.RestaurantParams.PageNumber - 1) * request.RestaurantParams.PageSize)
                .Take(request.RestaurantParams.PageSize)
                .Adapt<List<RestaurantDto>>()
        };

        await _cacheService.SetAsync(key, paginationResponse, CacheOptions.DefaultExpiration, cancellationToken);

        return paginationResponse;
    }
}