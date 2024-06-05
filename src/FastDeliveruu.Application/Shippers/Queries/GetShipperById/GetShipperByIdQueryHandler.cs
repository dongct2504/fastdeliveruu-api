using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace FastDeliveruu.Application.Shippers.Queries.GetShipperById;

public class GetShipperByIdQueryHandler : IRequestHandler<GetShipperByIdQuery, Result<ShipperDetailDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetShipperByIdQueryHandler(ICacheService cacheService, FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<Result<ShipperDetailDto>> Handle(
        GetShipperByIdQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Shipper}-{request.Id}";

        ShipperDetailDto? shipperDetailDtoCache = await _cacheService
            .GetAsync<ShipperDetailDto>(key, cancellationToken);
        if (shipperDetailDtoCache != null)
        {
            return shipperDetailDtoCache;
        }

        ShipperDetailDto? shipperDetailDto = await _dbContext.Shippers
            .Where(s => s.ShipperId == request.Id)
            .AsNoTracking()
            .ProjectToType<ShipperDetailDto>()
            .FirstOrDefaultAsync(cancellationToken);
        if (shipperDetailDto == null)
        {
            string message = "Shipper not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        await _cacheService.SetAsync(key, shipperDetailDto, CacheOptions.DefaultExpiration, cancellationToken);

        return shipperDetailDto;
    }
}