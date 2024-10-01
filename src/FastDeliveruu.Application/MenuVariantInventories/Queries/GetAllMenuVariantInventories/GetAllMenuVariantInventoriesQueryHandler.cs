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

namespace FastDeliveruu.Application.MenuVariantInventories.Queries.GetAllMenuVariantInventories;

public class GetAllMenuVariantInventoriesQueryHandler : IRequestHandler<GetAllMenuVariantInventoriesQuery, PagedList<MenuVariantInventoryDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public GetAllMenuVariantInventoriesQueryHandler(FastDeliveruuDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<PagedList<MenuVariantInventoryDto>> Handle(GetAllMenuVariantInventoriesQuery request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.MenuVariantInventories}-{request.DefaultParams}";

        PagedList<MenuVariantInventoryDto>? pagedListCache = await _cacheService.GetAsync<PagedList<MenuVariantInventoryDto>>(key, cancellationToken);
        if (pagedListCache != null)
        {
            return pagedListCache;
        }

        IQueryable<MenuVariantInventory> menuVariantInventoriesQuery = _dbContext.MenuVariantInventories
            .Include(mi => mi.MenuVariant)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.DefaultParams.Search))
        {
            menuVariantInventoriesQuery = menuVariantInventoriesQuery
                .Where(c => c.MenuVariant.VarietyName.ToLower().Contains(request.DefaultParams.Search.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.DefaultParams.Sort))
        {
            switch (request.DefaultParams.Sort)
            {
                case SortConstants.OldestUpdateAsc:
                    menuVariantInventoriesQuery = menuVariantInventoriesQuery.OrderBy(c => c.UpdatedAt);
                    break;
                case SortConstants.LatestUpdateDesc:
                    menuVariantInventoriesQuery = menuVariantInventoriesQuery.OrderByDescending(c => c.UpdatedAt);
                    break;
                case SortConstants.NameAsc:
                    menuVariantInventoriesQuery = menuVariantInventoriesQuery.OrderBy(c => c.MenuVariant.VarietyName);
                    break;
                case SortConstants.NameDesc:
                    menuVariantInventoriesQuery = menuVariantInventoriesQuery.OrderByDescending(c => c.MenuVariant.VarietyName);
                    break;
            }
        }
        else
        {
            menuVariantInventoriesQuery = menuVariantInventoriesQuery.OrderBy(c => c.MenuVariant.VarietyName);
        }

        PagedList<MenuVariantInventoryDto> pagedList = new PagedList<MenuVariantInventoryDto>
        {
            PageNumber = request.DefaultParams.PageNumber,
            PageSize = request.DefaultParams.PageSize,
            TotalRecords = await menuVariantInventoriesQuery.CountAsync(cancellationToken),
            Items = await menuVariantInventoriesQuery
                .AsNoTracking()
                .ProjectToType<MenuVariantInventoryDto>()
                .Skip((request.DefaultParams.PageNumber - 1) * request.DefaultParams.PageSize)
                .Take(request.DefaultParams.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, pagedList, CacheOptions.DefaultExpiration, cancellationToken);

        return pagedList;
    }
}
