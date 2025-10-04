using FastDeliveruu.Application.Common;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Common.Enums;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, PagedList<OrderDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetAllOrdersQueryHandler(ICacheService cacheService, FastDeliveruuDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<PagedList<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Orders}-{request.OrderParams}";

        PagedList<OrderDto>? pagedListCache = await _cacheService
            .GetAsync<PagedList<OrderDto>>(key, cancellationToken);
        if (pagedListCache != null)
        {
            return pagedListCache;
        }

        IQueryable<Order> query = _dbContext.Orders.AsQueryable();

        DateTime now = _dateTimeProvider.VietnamDateTimeNow;
        switch (request.OrderParams.TimeRange)
        {
            case TimeRangeEnum.Today:
                query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Date == now.Date);
                break;

            case TimeRangeEnum.ThisWeek:
                DateTime startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);
                query = query.Where(o => o.CreatedAt >= startOfWeek);
                break;

            case TimeRangeEnum.ThisMonth:
                query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Month == now.Month && o.CreatedAt.Value.Year == now.Year);
                break;

            case TimeRangeEnum.ThisYear:
                query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Year == now.Year);
                break;

            case TimeRangeEnum.All:
            default:
                break;
        }

        query = request.OrderParams.Sort switch
        {
            "date" => query.OrderByDescending(o => o.OrderDate),
            "total" => query.OrderByDescending(o => o.TotalAmount),
            _ => query.OrderByDescending(o => o.OrderDate)
        };

        PagedList<OrderDto> pagedList = new PagedList<OrderDto>
        {
            PageNumber = request.OrderParams.PageNumber,
            PageSize = request.OrderParams.PageSize,
            TotalRecords = await query.CountAsync(cancellationToken),
            Items = await query
                .AsNoTracking()
                .ProjectToType<OrderDto>()
                .Skip((request.OrderParams.PageNumber - 1) * request.OrderParams.PageSize)
                .Take(request.OrderParams.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, pagedList, CacheOptions.DefaultExpiration, cancellationToken);

        return pagedList;
    }
}
