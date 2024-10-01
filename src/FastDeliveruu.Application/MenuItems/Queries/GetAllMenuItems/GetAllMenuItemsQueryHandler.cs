using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
                    menuItemsQuery = menuItemsQuery.OrderBy(mi => mi.Price);
                    break;
                case SortConstants.PriceDesc:
                    menuItemsQuery = menuItemsQuery.OrderByDescending(mi => mi.Price);
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

        PagedList<MenuItemDto> paginationResponse = new PagedList<MenuItemDto>
        {
            PageNumber = request.MenuItemParams.PageNumber,
            PageSize = request.MenuItemParams.PageSize,
            TotalRecords = await menuItemsQuery.CountAsync(cancellationToken),
            Items = await menuItemsQuery
                .AsNoTracking()
                .ProjectToType<MenuItemDto>()
                .Skip((request.MenuItemParams.PageNumber - 1) * request.MenuItemParams.PageSize)
                .Take(request.MenuItemParams.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, paginationResponse, CacheOptions.DefaultExpiration, cancellationToken);

        return paginationResponse;
    }
}
