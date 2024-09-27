using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.OrderDtos;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderHeaderDetailDto>>
{
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetOrderByIdQueryHandler(
        FastDeliveruuDbContext dbContext,
        ILogger<GetOrderByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<OrderHeaderDetailDto>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        OrderHeaderDetailDto? orderHeaderDetailDto = await _dbContext.Orders
            .Where(o => o.AppUserId == request.UserId && o.Id == request.OrderId)
            .AsNoTracking()
            .ProjectToType<OrderHeaderDetailDto>()
            .FirstOrDefaultAsync(cancellationToken);
        if (orderHeaderDetailDto == null)
        {
            string message = "Order not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<OrderHeaderDetailDto>(new NotFoundError(message));
        }

        return orderHeaderDetailDto;
    }
}
