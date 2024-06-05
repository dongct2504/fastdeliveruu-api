using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.DeleteAllShoppingCarts;

public class DeleteAllShoppingCartsCommandHandler : IRequestHandler<DeleteAllShoppingCartsCommand, Result>
{
    private readonly IShoppingCartRepository _shoppingCartRepository;

    public DeleteAllShoppingCartsCommandHandler(IShoppingCartRepository shoppingCartRepository)
    {
        _shoppingCartRepository = shoppingCartRepository;
    }

    public async Task<Result> Handle(DeleteAllShoppingCartsCommand request, CancellationToken cancellationToken)
    {
        QueryOptions<ShoppingCart> options = new QueryOptions<ShoppingCart>
        {
            Where = sc => sc.LocalUserId == request.LocalUserId
        };
        IEnumerable<ShoppingCart> shoppingCarts = await _shoppingCartRepository.ListAllAsync(options);
        if (!shoppingCarts.Any())
        {
            string message = "The user does not have any cart.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        await _shoppingCartRepository.DeleteRangeAsync(shoppingCarts);

        return Result.Ok();
    }
}
