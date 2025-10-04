using FastDeliveruu.Application.Common;
using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AddressDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Wards.Queries.GetWardsByDistrict;

public class GetWardsByDistrictQueryHandler : IRequestHandler<GetWardsByDistrictQuery, Result<PagedList<WardDto>>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public GetWardsByDistrictQueryHandler(FastDeliveruuDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<Result<PagedList<WardDto>>> Handle(GetWardsByDistrictQuery request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.WardsByDistrict}-{request.WardParams}";

        PagedList<WardDto>? pagedListCache = await _cacheService.GetAsync<PagedList<WardDto>>(key, cancellationToken);
        if (pagedListCache != null)
        {
            return pagedListCache;
        }

        IQueryable<Ward> wardsQuery = _dbContext.Wards.AsQueryable();

        wardsQuery = wardsQuery.Where(w => w.DistrictId == request.WardParams.DistrictId);

        if (!string.IsNullOrEmpty(request.WardParams.Search))
        {
            wardsQuery = wardsQuery
                .Where(c => c.Name.ToLower().Contains(request.WardParams.Search.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.WardParams.Sort))
        {
            switch (request.WardParams.Sort)
            {
                case SortConstants.OldestUpdateAsc:
                    wardsQuery = wardsQuery.OrderBy(c => c.UpdatedAt);
                    break;
                case SortConstants.LatestUpdateDesc:
                    wardsQuery = wardsQuery.OrderByDescending(c => c.UpdatedAt);
                    break;
                case SortConstants.NameAsc:
                    wardsQuery = wardsQuery.OrderBy(c => c.Name);
                    break;
                case SortConstants.NameDesc:
                    wardsQuery = wardsQuery.OrderByDescending(c => c.Name);
                    break;
            }
        }
        else
        {
            wardsQuery = wardsQuery.OrderBy(c => c.Name);
        }

        PagedList<WardDto> pagedList = new PagedList<WardDto>
        {
            PageNumber = request.WardParams.PageNumber,
            PageSize = request.WardParams.PageSize,
            TotalRecords = await wardsQuery.CountAsync(cancellationToken),
            Items = await wardsQuery
                .AsNoTracking()
                .ProjectToType<WardDto>()
                .Skip((request.WardParams.PageNumber - 1) * request.WardParams.PageSize)
                .Take(request.WardParams.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, pagedList, CacheOptions.DefaultExpiration, cancellationToken);

        return pagedList;
    }
}
