using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.AddressDtos;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Cities.Queries.GetCityById;

public class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery, Result<CityDetailDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ILogger<GetCityByIdQueryHandler> _logger;

    public GetCityByIdQueryHandler(FastDeliveruuDbContext dbContext, ILogger<GetCityByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<CityDetailDto>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
    {
        CityDetailDto? cityDetailDto = await _dbContext.Cities
            .AsNoTracking()
            .ProjectToType<CityDetailDto>()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (cityDetailDto == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.CityNotFound} - {request}");
            return Result.Fail(new NotFoundError(ErrorMessageConstants.CityNotFound));
        }

        return cityDetailDto;
    }
}
