using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        string key = $"{CacheConstants.MenuItems}-{request.GenreId}-{request.RestaurantId}-{request.PageNumber}";

        PagedList<MenuItemDto>? paginationResponseCache = await _cacheService
            .GetAsync<PagedList<MenuItemDto>>(key, cancellationToken);
        if (paginationResponseCache != null)
        {
            return paginationResponseCache;
        }

        IQueryable<MenuItem> menuItemsQuery = _dbContext.MenuItems.AsQueryable();

        if (request.GenreId != null)
        {
            menuItemsQuery = menuItemsQuery.Where(mi => mi.GenreId == request.GenreId);
        }

        if (request.RestaurantId != null)
        {
            menuItemsQuery = menuItemsQuery.Where(mi => mi.RestaurantId == request.RestaurantId);
        }

        PagedList<MenuItemDto> paginationResponse = new PagedList<MenuItemDto>
        {
            PageNumber = request.PageNumber,
            PageSize = PageConstants.Default24,
            TotalRecords = await menuItemsQuery.CountAsync(cancellationToken),
            Items = await menuItemsQuery
                .AsNoTracking()
                .ProjectToType<MenuItemDto>()
                .Skip((request.PageNumber - 1) * PageConstants.Default24)
                .Take(PageConstants.Default24)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, paginationResponse, CacheOptions.DefaultExpiration, cancellationToken);

        return paginationResponse;
    }
}
