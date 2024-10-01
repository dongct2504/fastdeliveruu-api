using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.AddressDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Districts.Queries.GetDistrictsByCity;

public class GetDistrictsByCityQueryHandler : IRequestHandler<GetDistrictsByCityQuery, Result<PagedList<DistrictDto>>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetDistrictsByCityQueryHandler> _logger;

    public GetDistrictsByCityQueryHandler(
        FastDeliveruuDbContext dbContext,
        ICacheService cacheService,
        ILogger<GetDistrictsByCityQueryHandler> logger)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<PagedList<DistrictDto>>> Handle(GetDistrictsByCityQuery request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.DistrictsByCity}-{request.DistrictParams}";

        PagedList<DistrictDto>? pagedListCache = await _cacheService.GetAsync<PagedList<DistrictDto>>(key, cancellationToken);
        if (pagedListCache != null)
        {
            return pagedListCache;
        }

        City? city = await _dbContext.Cities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.DistrictParams.CityId, cancellationToken);
        if (city == null)
        {
            string message = "City does not exist";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        IQueryable<District> districtsQuery = _dbContext.Districts.AsQueryable();

        districtsQuery = districtsQuery.Where(d => d.CityId == request.DistrictParams.CityId);

        if (!string.IsNullOrEmpty(request.DistrictParams.Search))
        {
            districtsQuery = districtsQuery
                .Where(c => c.Name.ToLower().Contains(request.DistrictParams.Search.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.DistrictParams.Sort))
        {
            switch (request.DistrictParams.Sort)
            {
                case SortConstants.OldestUpdateAsc:
                    districtsQuery = districtsQuery.OrderBy(c => c.UpdatedAt);
                    break;
                case SortConstants.LatestUpdateDesc:
                    districtsQuery = districtsQuery.OrderByDescending(c => c.UpdatedAt);
                    break;
                case SortConstants.NameAsc:
                    districtsQuery = districtsQuery.OrderBy(c => c.Name);
                    break;
                case SortConstants.NameDesc:
                    districtsQuery = districtsQuery.OrderByDescending(c => c.Name);
                    break;
            }
        }
        else
        {
            districtsQuery = districtsQuery.OrderBy(c => c.Name);
        }

        PagedList<DistrictDto> pagedList = new PagedList<DistrictDto>
        {
            PageNumber = request.DistrictParams.PageNumber,
            PageSize = request.DistrictParams.PageSize,
            TotalRecords = await districtsQuery.CountAsync(cancellationToken),
            Items = await districtsQuery
                .AsNoTracking()
                .ProjectToType<DistrictDto>()
                .Skip((request.DistrictParams.PageNumber - 1) * request.DistrictParams.PageSize)
                .Take(request.DistrictParams.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, pagedList, CacheOptions.DefaultExpiration, cancellationToken);

        return pagedList;
    }
}
