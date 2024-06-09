using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.ShipperDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Shippers.Queries.GetAllShippers;

public class GetAllShippersQueryHandler : IRequestHandler<GetAllShippersQuery, PagedList<ShipperDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetAllShippersQueryHandler(
        ICacheService cacheService,
        FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<PagedList<ShipperDto>> Handle(
        GetAllShippersQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Shippers}-{request.PageNumber}-{request.PageSize}";

        PagedList<ShipperDto>? paginationResponseCache = await _cacheService
            .GetAsync<PagedList<ShipperDto>>(key, cancellationToken);
        if (paginationResponseCache != null)
        {
            return paginationResponseCache;
        }

        PagedList<ShipperDto> paginationResponse = new PagedList<ShipperDto>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalRecords = await _dbContext.Shippers.CountAsync(cancellationToken),
            Items = await _dbContext.Shippers
                .AsNoTracking()
                .ProjectToType<ShipperDto>()
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, paginationResponse, CacheOptions.DefaultExpiration, cancellationToken);

        return paginationResponse;
    }
}