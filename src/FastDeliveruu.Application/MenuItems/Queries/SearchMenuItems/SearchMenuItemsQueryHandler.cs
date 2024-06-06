using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.MenuItems.Queries.SearchMenuItems;

public class SearchMenuItemsQueryHandler : IRequestHandler<SearchMenuItemsQuery, IEnumerable<MenuItemDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public SearchMenuItemsQueryHandler(ICacheService cacheService, FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<MenuItemDto>> Handle(
        SearchMenuItemsQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.MenuItems}-{request.Amount}-{request.DiscountPercent}";

        IEnumerable<MenuItemDto>? menuItemDtosCache = await _cacheService
            .GetAsync<IEnumerable<MenuItemDto>>(key, cancellationToken);
        if (menuItemDtosCache != null)
        {
            return menuItemDtosCache;
        }

        IQueryable<MenuItem> menuItemsQuery = _dbContext.MenuItems.AsQueryable();

        menuItemsQuery = menuItemsQuery
            .Where(mi => mi.Price == request.Amount && mi.DiscountPercent == request.DiscountPercent);

        IEnumerable<MenuItemDto> menuItemDtos = await menuItemsQuery
            .AsNoTracking()
            .ProjectToType<MenuItemDto>()
            .Take(PageConstants.Other18)
            .ToListAsync(cancellationToken);

        await _cacheService.SetAsync(key, menuItemDtos, CacheOptions.DefaultExpiration, cancellationToken);

        return menuItemDtos;
    }
}
