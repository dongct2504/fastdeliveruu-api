using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.Interfaces;
using MediatR;

namespace FastDeliveruu.Application.ShoppingCarts.Queries.GetCustomerCart;

public class GetCustomerCartQueryHandler : IRequestHandler<GetCustomerCartQuery, List<ShoppingCartDto>>
{
    private readonly ICacheService _cacheService;

    public GetCustomerCartQueryHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<List<ShoppingCartDto>> Handle(
        GetCustomerCartQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.CustomerCart}-{request.UserId}";

        List<ShoppingCartDto>? customerCart = await _cacheService.GetAsync<List<ShoppingCartDto>>(key, cancellationToken);
        if (customerCart == null)
        {
            return new List<ShoppingCartDto>();
        }

        return customerCart;
    }
}
