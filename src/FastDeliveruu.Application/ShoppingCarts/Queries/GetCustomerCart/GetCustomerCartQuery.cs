using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using MediatR;

namespace FastDeliveruu.Application.ShoppingCarts.Queries.GetCustomerCart;

public class GetCustomerCartQuery : IRequest<List<ShoppingCartDto>>
{
    public GetCustomerCartQuery(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; }
}
