using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.AddressDtos;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Districts.Queries.GetDistrictById;

public class GetDistrictByIdQueryHandler : IRequestHandler<GetDistrictByIdQuery, Result<DistrictDetailDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ILogger<GetDistrictByIdQueryHandler> _logger;

    public GetDistrictByIdQueryHandler(
        FastDeliveruuDbContext dbContext,
        ILogger<GetDistrictByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<DistrictDetailDto>> Handle(GetDistrictByIdQuery request, CancellationToken cancellationToken)
    {
        DistrictDetailDto? districtDetailDto = await _dbContext.Districts
            .AsNoTracking()
            .ProjectToType<DistrictDetailDto>()
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
        if (districtDetailDto == null)
        {
            string message = "District does not exist";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        return districtDetailDto;
    }
}
