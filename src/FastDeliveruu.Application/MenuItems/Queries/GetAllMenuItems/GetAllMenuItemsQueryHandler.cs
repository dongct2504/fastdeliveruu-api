using FastDeliveruu.Application.Common;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Common.Helpers;

namespace FastDeliveruu.Application.MenuItems.Queries.GetAllMenuItems;

public class GetAllMenuItemsQueryHandler : IRequestHandler<GetAllMenuItemsQuery,
    PagedList<MenuItemDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetAllMenuItemsQueryHandler(
        ICacheService cacheService,
        FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<PagedList<MenuItemDto>> Handle(
        GetAllMenuItemsQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.MenuItems}-{request.MenuItemParams}";

        PagedList<MenuItemDto>? paginationResponseCache = await _cacheService
            .GetAsync<PagedList<MenuItemDto>>(key, cancellationToken);
        if (paginationResponseCache != null)
        {
            return paginationResponseCache;
        }

        bool hasUser = request.MenuItemParams.UserId != Guid.Empty;

        AppUser? user = null;
        AddressesCustomer? primaryAddress = null;

        if (hasUser)
        {
            user = await _dbContext.Users
                .Include(u => u.AddressesCustomers)
                .FirstOrDefaultAsync(u => u.Id == request.MenuItemParams.UserId, cancellationToken: cancellationToken);

            primaryAddress = user?
                .AddressesCustomers
                .FirstOrDefault(a => a.IsPrimary);
        }

        IQueryable<MenuItem> menuItemsQuery = _dbContext.MenuItems.AsQueryable();

        if (request.MenuItemParams.GenreId != null)
        {
            menuItemsQuery = menuItemsQuery.Where(mi => mi.GenreId == request.MenuItemParams.GenreId);
        }

        if (request.MenuItemParams.RestaurantId != null)
        {
            menuItemsQuery = menuItemsQuery.Where(mi => mi.RestaurantId == request.MenuItemParams.RestaurantId);
        }

        if (!string.IsNullOrEmpty(request.MenuItemParams.Search))
        {
            menuItemsQuery = menuItemsQuery
                .Where(mi => mi.Name.ToLower().Contains(request.MenuItemParams.Search.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.MenuItemParams.Sort))
        {
            switch (request.MenuItemParams.Sort)
            {
                case SortConstants.OldestUpdateAsc:
                    menuItemsQuery = menuItemsQuery.OrderBy(mi => mi.UpdatedAt);
                    break;
                case SortConstants.LatestUpdateDesc:
                    menuItemsQuery = menuItemsQuery.OrderByDescending(mi => mi.UpdatedAt);
                    break;
                case SortConstants.PriceAsc:
                    menuItemsQuery = menuItemsQuery.OrderBy(mi => mi.Price * (1 -mi.DiscountPercent));
                    break;
                case SortConstants.PriceDesc:
                    menuItemsQuery = menuItemsQuery.OrderByDescending(mi => mi.Price * (1 -mi.DiscountPercent));
                    break;
                case SortConstants.NameAsc:
                    menuItemsQuery = menuItemsQuery.OrderBy(mi => mi.Name);
                    break;
                case SortConstants.NameDesc:
                    menuItemsQuery = menuItemsQuery.OrderByDescending(mi => mi.Name);
                    break;
            }
        }
        else
        {
            menuItemsQuery = menuItemsQuery.OrderByDescending(mi => mi.UpdatedAt);
        }

        List<MenuItem> list = await menuItemsQuery
            .AsNoTracking()
            .Include(m => m.Restaurant)
            .ToListAsync(cancellationToken);

        //Sort special case
        bool isNearest = request.MenuItemParams.Sort == SortConstants.Nearest;
        bool isFarthest = request.MenuItemParams.Sort == SortConstants.Farthest;

        if ((isNearest || isFarthest) && primaryAddress != null)
        {
            double lat = (double)primaryAddress.Latitude;
            double lng = (double)primaryAddress.Longitude;

            list = isNearest
                ? list.OrderBy(x =>
                    GeoHelper.CalculateDistance(
                        lat, lng,
                        (double)x.Restaurant.Latitude,
                        (double)x.Restaurant.Longitude))
                    .ToList()
                : list.OrderByDescending(x =>
                    GeoHelper.CalculateDistance(
                        lat, lng,
                        (double)x.Restaurant.Latitude,
                        (double)x.Restaurant.Longitude))
                    .ToList();
        }

        PagedList<MenuItemDto> paginationResponse = new PagedList<MenuItemDto>
        {
            PageNumber = request.MenuItemParams.PageNumber,
            PageSize = request.MenuItemParams.PageSize,
            TotalRecords = await menuItemsQuery.CountAsync(cancellationToken),
            Items = list
                .Skip((request.MenuItemParams.PageNumber - 1) * request.MenuItemParams.PageSize)
                .Take(request.MenuItemParams.PageSize)
                .Adapt<List<MenuItemDto>>()
        };

        await _cacheService.SetAsync(key, paginationResponse, CacheOptions.DefaultExpiration, cancellationToken);

        return paginationResponse;
    }
}
