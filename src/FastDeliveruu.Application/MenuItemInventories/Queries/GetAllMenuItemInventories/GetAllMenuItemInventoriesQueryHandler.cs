using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.MenuItemInventories.Queries.GetAllMenuItemInventories;

public class GetAllMenuItemInventoriesQueryHandler : IRequestHandler<GetAllMenuItemInventoriesQuery, PagedList<MenuItemInventoryDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public GetAllMenuItemInventoriesQueryHandler(FastDeliveruuDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<PagedList<MenuItemInventoryDto>> Handle(GetAllMenuItemInventoriesQuery request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.MenuItemInventories}-{request.DefaultParams}";

        PagedList<MenuItemInventoryDto>? pagedListCache = await _cacheService.GetAsync<PagedList<MenuItemInventoryDto>>(key, cancellationToken);
        if (pagedListCache != null)
        {
            return pagedListCache;
        }

        IQueryable<MenuItemInventory> menuItemInventoriesQuery = _dbContext.MenuItemInventories.AsQueryable();

        PagedList<MenuItemInventoryDto> pagedList = new PagedList<MenuItemInventoryDto>
        {
            PageNumber = request.DefaultParams.PageNumber,
            PageSize = request.DefaultParams.PageSize,
            TotalRecords = await menuItemInventoriesQuery.CountAsync(cancellationToken),
            Items = await menuItemInventoriesQuery
                .AsNoTracking()
                .ProjectToType<MenuItemInventoryDto>()
                .Skip((request.DefaultParams.PageNumber - 1) * request.DefaultParams.PageSize)
                .Take(request.DefaultParams.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, pagedList, CacheOptions.DefaultExpiration, cancellationToken);

        return pagedList;
    }
}
