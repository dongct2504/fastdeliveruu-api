using FastDeliveruu.Application.Common.Enums;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Orders.Queries.GetOrderSummary;

public class GetOrderSummaryQueryHandler : IRequestHandler<GetOrderSummaryQuery, OrderSummaryDto>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GetOrderSummaryQueryHandler(FastDeliveruuDbContext dbContext, IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<OrderSummaryDto> Handle(GetOrderSummaryQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Order> query = _dbContext.Orders.AsQueryable();

        DateTime now = _dateTimeProvider.VietnamDateTimeNow;
        switch (request.TimeRange)
        {
            case TimeRangeEnum.Today:
                query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Date == now.Date);
                break;

            case TimeRangeEnum.ThisWeek:
                DateTime startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek);
                query = query.Where(o => o.CreatedAt >= startOfWeek);
                break;

            case TimeRangeEnum.ThisMonth:
                query = query.Where(o => o.CreatedAt.HasValue
                    && o.CreatedAt.Value.Month == now.Month
                    && o.CreatedAt.Value.Year == now.Year);
                break;

            case TimeRangeEnum.ThisYear:
                query = query.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Year == now.Year);
                break;

            case TimeRangeEnum.All:
            default:
                break;
        }

        int totalOrders = await query.CountAsync();
        decimal totalRevenue = await query.SumAsync(o => o.TotalAmount);
        decimal totalPaid = await query.Where(o => o.TransactionId != null).SumAsync(o => o.TotalAmount);
        decimal totalUnpaid = totalRevenue - totalPaid;

        var summary = new OrderSummaryDto
        {
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            TotalPaid = totalPaid,
            TotalUnpaid = totalUnpaid
        };

        return summary;
    }
}
