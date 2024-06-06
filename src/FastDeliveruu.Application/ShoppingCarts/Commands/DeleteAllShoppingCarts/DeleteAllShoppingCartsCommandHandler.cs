using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.ShoppingCarts;
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
        var spec = new CartByUserIdSpecification(request.LocalUserId);
        IEnumerable<ShoppingCart> shoppingCarts = await _shoppingCartRepository
            .ListAllWithSpecAsync(spec, asNoTracking: true);
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
