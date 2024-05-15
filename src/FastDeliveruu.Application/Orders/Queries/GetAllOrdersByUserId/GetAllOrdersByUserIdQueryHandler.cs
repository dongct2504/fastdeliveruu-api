using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using MapsterMapper;
using MediatR;

namespace FastDeliveruu.Application.Orders.Queries.GetAllOrdersByUserId;

public class GetAllOrdersByUserIdQueryHandler : IRequestHandler<GetAllOrdersByUserIdQuery,
    PaginationResponse<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetAllOrdersByUserIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<OrderDto>> Handle(
        GetAllOrdersByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        QueryOptions<Order> options = new QueryOptions<Order>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize,
            Where = o => o.LocalUserId == request.UserId
        };

        PaginationResponse<OrderDto> paginationResponse = new PaginationResponse<OrderDto>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize,
            Items = _mapper.Map<IEnumerable<OrderDto>>(
                await _orderRepository.ListAllAsync(options, asNoTracking: true)),
            TotalRecords = await _orderRepository.GetCountAsync()
        };

        return paginationResponse;
    }
}
