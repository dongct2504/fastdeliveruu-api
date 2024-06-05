using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
using FastDeliveruu.Domain.Data;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Genres.Queries.GetAllGenres;

public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, PagedList<GenreDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetAllGenresQueryHandler(ICacheService cacheService, FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<PagedList<GenreDto>> Handle(
        GetAllGenresQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Genres}-{request.PageNumber}";

        PagedList<GenreDto>? paginationResponseCache = await _cacheService
            .GetAsync<PagedList<GenreDto>>(key, cancellationToken);
        if (paginationResponseCache != null)
        {
            return paginationResponseCache;
        }

        PagedList<GenreDto> paginationResponse = new PagedList<GenreDto>
        {
            PageNumber = request.PageNumber,
            PageSize = PageConstants.Default24,
            TotalRecords = await _dbContext.Genres.CountAsync(cancellationToken),
            Items = await _dbContext.Genres
                .AsNoTracking()
                .ProjectToType<GenreDto>()
                .Skip((request.PageNumber - 1) * PageConstants.Default24)
                .Take(PageConstants.Default24)
                .ToListAsync(cancellationToken)
        };

        await _cacheService.SetAsync(key, paginationResponse, CacheOptions.DefaultExpiration, cancellationToken);

        return paginationResponse;
    }
}