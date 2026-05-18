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

        AppUser? user = await _dbContext.Users
            .Include(u => u.AddressesCustomers)
            .FirstOrDefaultAsync(u => u.Id == request.RestaurantParams.UserId, cancellationToken: cancellationToken);

        AddressesCustomer? primaryAddress = user?
            .AddressesCustomers
            .FirstOrDefault(a => a.IsPrimary);

        if (primaryAddress == null)
        {
            return Result.Fail(new BadRequestError(ErrorMessageConstants.PrimaryAddressNotFound));
        }

        IQueryable<Restaurant> restaurantsQuery = _dbContext.Restaurants.AsQueryable();


        restaurantsQuery = restaurantsQuery
            .Where(c => string.IsNullOrEmpty(request.RestaurantParams.Search) || c.Name.ToLower().Contains(request.RestaurantParams.Search.ToLower()));

        List<Restaurant> restaurants = await restaurantsQuery
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        if (!string.IsNullOrEmpty(request.RestaurantParams.Sort))
        {
            switch (request.RestaurantParams.Sort)
            {
                case SortConstants.OldestUpdateAsc:
                    restaurants = restaurants.OrderBy(r => r.UpdatedAt).ToList();
                    break;
                case SortConstants.LatestUpdateDesc:
                    restaurants = restaurants.OrderByDescending(r => r.UpdatedAt).ToList();
                    break;
                case SortConstants.NameAsc:
                    restaurants = restaurants.OrderBy(r => r.Name).ToList();
                    break;
                case SortConstants.NameDesc:
                    restaurants = restaurants.OrderByDescending(r => r.Name).ToList();
                    break;
                case SortConstants.Nearest:
                    restaurants = restaurants.OrderBy(r =>
                        GeoHelper.CalculateDistance(
                            (double)primaryAddress.Latitude,
                            (double)primaryAddress.Longitude,
                            (double)r.Latitude,
                            (double)r.Longitude))
                        .ToList();
                    break;
                case SortConstants.Farthest:
                    restaurants = restaurants.OrderByDescending(r =>
                        GeoHelper.CalculateDistance(
                            (double)primaryAddress.Latitude,
                            (double)primaryAddress.Longitude,
                            (double)r.Latitude,
                            (double)r.Longitude))
                        .ToList();
                    break;
            }
        }
        else
        {
            restaurants = restaurants.OrderBy(r => r.Name).ToList();
        }

        PagedList<RestaurantDto> paginationResponse = new PagedList<RestaurantDto>
        {
            PageNumber = request.RestaurantParams.PageNumber,
            PageSize = request.RestaurantParams.PageSize,
            TotalRecords = await restaurantsQuery.CountAsync(cancellationToken),
            //Items = await restaurantsQuery
            //    .AsNoTracking()
            //    .ProjectToType<RestaurantDto>()
            //    .Skip((request.RestaurantParams.PageNumber - 1) * request.RestaurantParams.PageSize)
            //    .Take(request.RestaurantParams.PageSize)
            //    .ToListAsync(cancellationToken)
            Items = restaurants
                .Skip((request.RestaurantParams.PageNumber - 1) * request.RestaurantParams.PageSize)
                .Take(request.RestaurantParams.PageSize)
                .Adapt<List<RestaurantDto>>()
        };

        await _cacheService.SetAsync(key, paginationResponse, CacheOptions.DefaultExpiration, cancellationToken);

        return paginationResponse;
    }
}