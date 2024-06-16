using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FastDeliveruu.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderHeaderDetailDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetOrderByIdQueryHandler(ICacheService cacheService, FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<Result<OrderHeaderDetailDto>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Order}-{request.UserId}-{request.OrderId}";

        OrderHeaderDetailDto? orderHeaderDetailDtoCache = await _cacheService
            .GetAsync<OrderHeaderDetailDto>(key, cancellationToken);
        if (orderHeaderDetailDtoCache != null)
        {
            return orderHeaderDetailDtoCache;
        }

        OrderHeaderDetailDto? orderHeaderDetailDto = await _dbContext.Orders
            .Where(o => o.AppUserId == request.UserId && o.OrderId == request.OrderId)
            .AsNoTracking()
            .ProjectToType<OrderHeaderDetailDto>()
            .FirstOrDefaultAsync(cancellationToken);
        if (orderHeaderDetailDto == null)
        {
            string message = "Order not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<OrderHeaderDetailDto>(new NotFoundError(message));
        }

        await _cacheService.SetAsync(key, orderHeaderDetailDto, CacheOptions.DefaultExpiration, cancellationToken);

        return orderHeaderDetailDto;
    }
}
