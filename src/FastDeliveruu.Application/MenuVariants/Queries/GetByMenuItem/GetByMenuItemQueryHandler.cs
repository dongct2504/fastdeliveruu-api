using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos.MenuItemDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.MenuVariants.Queries.GetByMenuItem;

public class GetByMenuItemQueryHandler : IRequestHandler<GetByMenuItemQuery, List<MenuVariantDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetByMenuItemQueryHandler(ICacheService cacheService, FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<List<MenuVariantDto>> Handle(GetByMenuItemQuery request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.MenuVariants}-{request.MenuItemId}";
        List<MenuVariantDto>? menuVariantsCache = await _cacheService.GetAsync<List<MenuVariantDto>>(key);
        if (menuVariantsCache != null)
        {
            return menuVariantsCache;
        }

        IQueryable<MenuVariant> menuVariantQuery = _dbContext.MenuVariants.AsQueryable();

        menuVariantQuery = menuVariantQuery
            .Where(mv => mv.MenuItemId == request.MenuItemId);

        List<MenuVariantDto> menuVariantDtos = await menuVariantQuery
            .AsNoTracking()
            .ProjectToType<MenuVariantDto>()
            .ToListAsync();

        await _cacheService.SetAsync(key, menuVariantDtos, CacheOptions.DefaultExpiration, cancellationToken);

        return menuVariantDtos;
    }
}
