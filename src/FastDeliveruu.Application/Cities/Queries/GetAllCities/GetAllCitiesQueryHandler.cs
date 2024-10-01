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

namespace FastDeliveruu.Application.Cities.Queries.GetAllCities;

public class GetAllCitiesQueryHandler : IRequestHandler<GetAllCitiesQuery, PagedList<CityDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;
    private readonly ICacheService _cacheService;

    public GetAllCitiesQueryHandler(FastDeliveruuDbContext dbContext, ICacheService cacheService)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }

    public async Task<PagedList<CityDto>> Handle(GetAllCitiesQuery request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Cities}-{request.DefaultParams}";

        PagedList<CityDto>? pagedListCache = await _cacheService.GetAsync<PagedList<CityDto>>(key);
        if (pagedListCache != null)
        {
            return pagedListCache;
        }

        IQueryable<City> citiesQuery = _dbContext.Cities.AsQueryable();

        if (!string.IsNullOrEmpty(request.DefaultParams.Search))
        {
            citiesQuery = citiesQuery
                .Where(c => c.Name.ToLower().Contains(request.DefaultParams.Search.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.DefaultParams.Sort))
        {
            switch (request.DefaultParams.Sort)
            {
                case SortConstants.OldestUpdateAsc:
                    citiesQuery = citiesQuery.OrderBy(c => c.UpdatedAt);
                    break;
                case SortConstants.LatestUpdateDesc:
                    citiesQuery = citiesQuery.OrderByDescending(c => c.UpdatedAt);
                    break;
                case SortConstants.NameAsc:
                    citiesQuery = citiesQuery.OrderBy(c => c.Name);
                    break;
                case SortConstants.NameDesc:
                    citiesQuery = citiesQuery.OrderByDescending(c => c.Name);
                    break;
            }
        }
        else
        {
            citiesQuery = citiesQuery.OrderBy(c => c.Name);
        }

        PagedList<CityDto> pagedList = new PagedList<CityDto>
        {
            PageNumber = request.DefaultParams.PageNumber,
            PageSize = request.DefaultParams.PageSize,
            TotalRecords = await citiesQuery.CountAsync(),
            Items = await citiesQuery
                .AsNoTracking()
                .ProjectToType<CityDto>()
                .Skip((request.DefaultParams.PageNumber - 1) * request.DefaultParams.PageSize)
                .Take(request.DefaultParams.PageSize)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, pagedList, CacheOptions.DefaultExpiration, cancellationToken);

        return pagedList;
    }
}
