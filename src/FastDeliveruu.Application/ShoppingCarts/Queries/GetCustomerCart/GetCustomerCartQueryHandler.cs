using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using MapsterMapper;
using MediatR;

namespace FastDeliveruu.Application.ShoppingCarts.Queries.GetCustomerCart;

public class GetCustomerCartQueryHandler : IRequestHandler<GetCustomerCartQuery, List<ShoppingCartDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public GetCustomerCartQueryHandler(ICacheService cacheService, IMapper mapper)
    {
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<List<ShoppingCartDto>> Handle(
        GetCustomerCartQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.CustomerCart}-{request.UserId}";

        List<ShoppingCart>? customerCart = await _cacheService.GetAsync<List<ShoppingCart>>(key, cancellationToken);
        if (customerCart == null)
        {
            return new List<ShoppingCartDto>();
        }

        return _mapper.Map<List<ShoppingCartDto>>(customerCart);
    }
}
