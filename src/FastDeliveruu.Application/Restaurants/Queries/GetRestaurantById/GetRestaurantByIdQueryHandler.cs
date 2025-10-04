using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Restaurants.Queries.GetRestaurantById;

public class GetRestaurantByIdQueryHandler : IRequestHandler<GetRestaurantByIdQuery, Result<RestaurantDetailDto>>
{
    private readonly ILogger<GetRestaurantByIdQueryHandler> _logger;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetRestaurantByIdQueryHandler(
        FastDeliveruuDbContext dbContext,
        ILogger<GetRestaurantByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<RestaurantDetailDto>> Handle(
        GetRestaurantByIdQuery request,
        CancellationToken cancellationToken)
    {
        RestaurantDetailDto? restaurantDetailDto = await _dbContext.Restaurants
            .Where(r => r.Id == request.Id)
            .AsNoTracking()
            .ProjectToType<RestaurantDetailDto>()
            .FirstOrDefaultAsync(cancellationToken);

        if (restaurantDetailDto == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.RestaurantNotFound} - {request}");
            return Result.Fail(new NotFoundError(ErrorMessageConstants.RestaurantNotFound));
        }

        return restaurantDetailDto;
    }
}