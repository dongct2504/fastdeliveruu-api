using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.ShoppingCarts.Queries.GetAllShoppingCarts;

public class GetAllShoppingCartsByUserIdQueryHandler : IRequestHandler<GetAllShoppingCartsByUserIdQuery, 
    PagedList<ShoppingCartDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetAllShoppingCartsByUserIdQueryHandler(FastDeliveruuDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<PagedList<ShoppingCartDto>> Handle(
        GetAllShoppingCartsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.ShoppingCarts}-{request.UserId}-{request.PageNumber}-{request.PageSize}";

        PagedList<ShoppingCartDto>? pagedListCache = await _cacheService
            .GetAsync<PagedList<ShoppingCartDto>>(key, cancellationToken);
        if (pagedListCache != null)
        {
            return pagedListCache;
        }

        IQueryable<ShoppingCart> shoppingCartsQuery = _dbContext.ShoppingCarts.AsQueryable();

        shoppingCartsQuery = shoppingCartsQuery
            .Where(sc => sc.LocalUserId == request.UserId);

        PagedList<ShoppingCartDto> pagedList = new PagedList<ShoppingCartDto>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = await shoppingCartsQuery.CountAsync(cancellationToken),
            Items = await shoppingCartsQuery
                .AsNoTracking()
                .ProjectToType<ShoppingCartDto>()
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, pagedList, CacheOptions.DefaultExpiration, cancellationToken);

        return pagedList;
    }
}
