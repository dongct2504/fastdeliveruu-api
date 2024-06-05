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
    PagedList<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetAllOrdersByUserIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<PagedList<OrderDto>> Handle(
        GetAllOrdersByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        QueryOptions<Order> options = new QueryOptions<Order>
        {
            PageNumber = request.PageNumber,
            PageSize = PageConstants.Default24,
            Where = o => o.LocalUserId == request.UserId
        };

        PagedList<OrderDto> paginationResponse = new PagedList<OrderDto>
        {
            PageNumber = request.PageNumber,
            PageSize = PageConstants.Default24,
            Items = _mapper.Map<IEnumerable<OrderDto>>(
                await _orderRepository.ListAllAsync(options, asNoTracking: true)),
            TotalRecords = await _orderRepository.GetCountAsync()
        };

        return paginationResponse;
    }
}
