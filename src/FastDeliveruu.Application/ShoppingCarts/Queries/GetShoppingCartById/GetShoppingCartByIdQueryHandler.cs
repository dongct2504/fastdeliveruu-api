using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.ShoppingCarts.Queries.GetShoppingCartById;

public class GetShoppingCartByIdQueryHandler : IRequestHandler<GetShoppingCartByIdQuery, Result<ShoppingCartDto>>
{
    private readonly IShoppingCartRepository _shoppingCartRepository;
    private readonly IMapper _mapper;

    public GetShoppingCartByIdQueryHandler(IShoppingCartRepository shoppingCartRepository, IMapper mapper)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _mapper = mapper;
    }

    public async Task<Result<ShoppingCartDto>> Handle(
        GetShoppingCartByIdQuery request,
        CancellationToken cancellationToken)
    {
        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            SetIncludes = "MenuItem",
            Where = sc => sc.LocalUserId == request.LocalUserId && sc.MenuItemId == request.MenuItemId
        };
        ShoppingCart? shoppingCart = await _shoppingCartRepository.GetAsync(options);
        if (shoppingCart == null)
        {
            string message = "Shopping Cart not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<ShoppingCartDto>(new NotFoundError(message));
        }

        return _mapper.Map<ShoppingCartDto>(shoppingCart);
    }
}
