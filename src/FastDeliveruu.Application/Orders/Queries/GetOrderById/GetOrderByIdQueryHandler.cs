using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderHeaderDetailDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<Result<OrderHeaderDetailDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        QueryOptions<Order> options = new QueryOptions<Order>
        {
            SetIncludes = "LocalUser, Shipper, OrderDetails.MenuItem",
            Where = o => o.LocalUserId == request.UserId && o.OrderId == request.OrderId
        };
        Order? order = await _orderRepository.GetAsync(options, asNoTracking: true);
        if (order == null)
        {
            string message = "Order not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<OrderHeaderDetailDto>(new NotFoundError(message));
        }

        return _mapper.Map<OrderHeaderDetailDto>(order);
    }
}
