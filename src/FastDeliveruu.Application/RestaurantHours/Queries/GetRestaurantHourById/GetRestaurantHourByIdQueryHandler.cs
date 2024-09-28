using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.RestaurantDtos;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.RestaurantHours.Queries.GetRestaurantHourById;

public class GetRestaurantHourByIdQueryHandler : IRequestHandler<GetRestaurantHourByIdQuery, Result<RestaurantHourDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ILogger<GetRestaurantHourByIdQueryHandler> _logger;

    public GetRestaurantHourByIdQueryHandler(
        FastDeliveruuDbContext dbContext,
        ILogger<GetRestaurantHourByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<RestaurantHourDto>> Handle(GetRestaurantHourByIdQuery request, CancellationToken cancellationToken)
    {
        RestaurantHourDto? restaurantHourDto = await _dbContext.RestaurantHours
            .AsNoTracking()
            .ProjectToType<RestaurantHourDto>()
            .FirstOrDefaultAsync(rh => rh.Id == request.Id, cancellationToken);

        if (restaurantHourDto == null)
        {
            string message = "RestaurantHour does not exist";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        return restaurantHourDto;
    }
}
