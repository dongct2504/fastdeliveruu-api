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

namespace FastDeliveruu.Application.Wards.Queries.GetAllWards;

public class GetAllWardsQueryHandler : IRequestHandler<GetAllWardsQuery, PagedList<WardDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public GetAllWardsQueryHandler(FastDeliveruuDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<PagedList<WardDto>> Handle(GetAllWardsQuery request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Wards}-{request.DefaultParams}";

        PagedList<WardDto>? pagedListCache = await _cacheService.GetAsync<PagedList<WardDto>>(key, cancellationToken);
        if (pagedListCache != null)
        {
            return pagedListCache;
        }

        IQueryable<Ward> wardsQuery = _dbContext.Wards.AsQueryable();

        if (!string.IsNullOrEmpty(request.DefaultParams.Search))
        {
            wardsQuery = wardsQuery
                .Where(c => c.Name.ToLower().Contains(request.DefaultParams.Search.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.DefaultParams.Sort))
        {
            switch (request.DefaultParams.Sort)
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
            PageNumber = request.DefaultParams.PageNumber,
            PageSize = request.DefaultParams.PageSize,
            TotalRecords = await wardsQuery.CountAsync(cancellationToken),
            Items = await wardsQuery
                .AsNoTracking()
                .ProjectToType<WardDto>()
                .Skip((request.DefaultParams.PageNumber - 1) * request.DefaultParams.PageSize)
                .Take(request.DefaultParams.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, pagedList, CacheOptions.DefaultExpiration, cancellationToken);

        return pagedList;
    }
}
