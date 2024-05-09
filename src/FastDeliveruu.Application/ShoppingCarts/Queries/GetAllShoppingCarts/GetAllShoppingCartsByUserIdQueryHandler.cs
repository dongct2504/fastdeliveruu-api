using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using MapsterMapper;
using MediatR;

namespace FastDeliveruu.Application.ShoppingCarts.Queries.GetAllShoppingCarts;

public class GetAllShoppingCartsByUserIdQueryHandler : IRequestHandler<GetAllShoppingCartsByUserIdQuery, 
    PaginationResponse<ShoppingCartDto>>
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IMapper _mapper;

    public GetAllShoppingCartsByUserIdQueryHandler(IShoppingCartRepository shoppingCartRepository, IMapper mapper)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _mapper = mapper;
    }

    public async Task<PaginationResponse<ShoppingCartDto>> Handle(
        GetAllShoppingCartsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize,
            SetIncludes = "MenuItem",
            Where = sc => sc.LocalUserId == request.UserId
        };

        PaginationResponse<ShoppingCartDto> paginationResponse = new PaginationResponse<ShoppingCartDto>
        {
            PageNumber = request.PageNumber,
            PageSize = PagingConstants.DefaultPageSize,
            Items = _mapper.Map<IEnumerable<ShoppingCartDto>>(await _shoppingCartRepository.ListAllAsync(options)),
            TotalRecords = await _shoppingCartRepository.GetCountAsync()
        };

        return paginationResponse;
    }
}
