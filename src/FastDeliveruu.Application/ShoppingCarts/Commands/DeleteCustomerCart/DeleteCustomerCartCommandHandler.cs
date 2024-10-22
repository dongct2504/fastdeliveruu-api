using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.ShoppingCarts.Commands.DeleteCustomerCart;

public class DeleteCustomerCartCommandHandler : IRequestHandler<DeleteCustomerCartCommand, Result>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<DeleteCustomerCartCommandHandler> _logger;

    public DeleteCustomerCartCommandHandler(
        ICacheService cacheService,
        ILogger<DeleteCustomerCartCommandHandler> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteCustomerCartCommand request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.CustomerCart}-{request.UserId}";

        List<ShoppingCartDto>? customerCartCache = await _cacheService
            .GetAsync<List<ShoppingCartDto>>(key, cancellationToken);

        if (customerCartCache == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CustomerCartEmpty} - {request}");
            return Result.Fail(new BadRequestError(ErrorMessageConstants.CustomerCartEmpty));
        }

        await _cacheService.RemoveAsync(key, cancellationToken);

        return Result.Ok();
    }
}
