using FastDeliveruu.Common.Enums;
using FastDeliveruu.Application.Dtos.OrderDtos;
using MediatR;

namespace FastDeliveruu.Application.Orders.Queries.GetOrderSummary;

public class GetOrderSummaryQuery : IRequest<OrderSummaryDto>
{
    public GetOrderSummaryQuery(TimeRangeEnum timeRangeEnum)
    {
        TimeRange = timeRangeEnum;
    }

    public TimeRangeEnum TimeRange { get; }
}
