using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShoppingCartDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FastDeliveruu.Application.ShoppingCarts.Queries.GetShoppingCartById;

public class GetShoppingCartByIdQueryHandler : IRequestHandler<GetShoppingCartByIdQuery, Result<ShoppingCartDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetShoppingCartByIdQueryHandler(ICacheService cacheService, FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<Result<ShoppingCartDto>> Handle(
        GetShoppingCartByIdQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.ShoppingCart}-{request.LocalUserId}-{request.MenuItemId}";

        ShoppingCartDto? shoppingCartDtoCache = await _cacheService
            .GetAsync<ShoppingCartDto>(key, cancellationToken);
        if (shoppingCartDtoCache != null)
        {
            return shoppingCartDtoCache;
        }

        ShoppingCartDto? shoppingCartDto = await _dbContext.ShoppingCarts
            .Where(sc => sc.LocalUserId == request.LocalUserId && sc.MenuItemId == request.MenuItemId)
            .AsNoTracking()
            .ProjectToType<ShoppingCartDto>()
            .FirstOrDefaultAsync(cancellationToken);
        if (shoppingCartDto == null)
        {
            string message = "Shopping Cart not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<ShoppingCartDto>(new NotFoundError(message));
        }

        await _cacheService.SetAsync(key, shoppingCartDto, CacheOptions.DefaultExpiration, cancellationToken);

        return shoppingCartDto;
    }
}
