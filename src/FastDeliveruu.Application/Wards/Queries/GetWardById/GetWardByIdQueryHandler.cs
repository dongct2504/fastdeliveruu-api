using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.AddressDtos;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Wards.Queries.GetWardById;

public class GetWardByIdQueryHandler : IRequestHandler<GetWardByIdQuery, Result<WardDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ILogger<GetWardByIdQueryHandler> _logger;

    public GetWardByIdQueryHandler(FastDeliveruuDbContext dbContext, ILogger<GetWardByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<WardDto>> Handle(GetWardByIdQuery request, CancellationToken cancellationToken)
    {
        WardDto? wardDto = await _dbContext.Wards
            .AsNoTracking()
            .ProjectToType<WardDto>()
            .FirstOrDefaultAsync(w => w.Id == request.Id);

        if (wardDto == null)
        {
            _logger.LogWarning($"{request.GetType().Name} - {ErrorMessageConstants.WardNotFound} - {request}");
            return Result.Fail(new NotFoundError(ErrorMessageConstants.WardNotFound));
        }

        return wardDto;
    }
}
