using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.DeleteCustomerCart;

public class DeleteCustomerCartCommandHandler : IRequestHandler<DeleteCustomerCartCommand, Result>
{
    private readonly ICacheService _cacheService;

    public DeleteCustomerCartCommandHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(DeleteCustomerCartCommand request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.CustomerCart}-{request.UserId}";

        List<ShoppingCart>? customerCartCache = await _cacheService
            .GetAsync<List<ShoppingCart>>(key, cancellationToken);

        if (customerCartCache == null)
        {
            string message = "The customer's cart is already empty";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new BadRequestError(message));
        }

        await _cacheService.RemoveAsync(key, cancellationToken);

        return Result.Ok();
    }
}
