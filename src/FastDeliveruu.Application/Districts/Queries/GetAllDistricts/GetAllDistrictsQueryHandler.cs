using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AddressDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Districts.Queries.GetAllDistricts;

public class GetAllDistrictsQueryHandler : IRequestHandler<GetAllDistrictsQuery, PagedList<DistrictDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public GetAllDistrictsQueryHandler(FastDeliveruuDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<PagedList<DistrictDto>> Handle(GetAllDistrictsQuery request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Districts}-{request.DefaultParams}";

        PagedList<DistrictDto>? pagedListCache = await _cacheService.GetAsync<PagedList<DistrictDto>>(key, cancellationToken);
        if (pagedListCache != null)
        {
            return pagedListCache;
        }

        IQueryable<District> districtsQuery = _dbContext.Districts.AsQueryable();

        PagedList<DistrictDto> pagedList = new PagedList<DistrictDto>
        {
            PageNumber = request.DefaultParams.PageNumber,
            PageSize = request.DefaultParams.PageSize,
            TotalRecords = await districtsQuery.CountAsync(cancellationToken),
            Items = await districtsQuery
                .AsNoTracking()
                .ProjectToType<DistrictDto>()
                .Skip((request.DefaultParams.PageNumber - 1) * request.DefaultParams.PageSize)
                .Take(request.DefaultParams.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, pagedList, CacheOptions.DefaultExpiration, cancellationToken);

        return pagedList;
    }
}
