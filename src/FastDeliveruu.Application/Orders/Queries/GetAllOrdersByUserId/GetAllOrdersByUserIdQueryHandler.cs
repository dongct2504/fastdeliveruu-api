using FastDeliveruu.Application.Common;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Orders.Queries.GetAllOrdersByUserId;

public class GetAllOrdersByUserIdQueryHandler : IRequestHandler<GetAllOrdersByUserIdQuery,
    PagedList<OrderDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetAllOrdersByUserIdQueryHandler(ICacheService cacheService, FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<PagedList<OrderDto>> Handle(
        GetAllOrdersByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Orders}-{request.UserId}-{request.PageNumber}-{request.PageSize}";

        PagedList<OrderDto>? pagedListCache = await _cacheService
            .GetAsync<PagedList<OrderDto>>(key, cancellationToken);
        if (pagedListCache != null)
        {
            return pagedListCache;
        }

        IQueryable<Order> ordersQuery = _dbContext.Orders.AsQueryable();

        ordersQuery = ordersQuery.Where(o => o.AppUserId == request.UserId);

        ordersQuery = ordersQuery.OrderByDescending(o => o.OrderDate);

        PagedList<OrderDto> pagedList = new PagedList<OrderDto>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = await ordersQuery.CountAsync(cancellationToken),
            Items = await ordersQuery
                .AsNoTracking()
                .ProjectToType<OrderDto>()
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, pagedList, CacheOptions.DefaultExpiration, cancellationToken);

        return pagedList;
    }
}
