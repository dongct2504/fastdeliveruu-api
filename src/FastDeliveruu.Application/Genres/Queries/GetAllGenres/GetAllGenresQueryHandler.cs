using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.Genres.Queries.GetAllGenres;

public class GetAllGenresQueryHandler : IRequestHandler<GetAllGenresQuery, List<GenreDto>>
{
    private readonly ICacheService _cacheService;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetAllGenresQueryHandler(ICacheService cacheService, FastDeliveruuDbContext dbContext)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
    }

    public async Task<List<GenreDto>> Handle(
        GetAllGenresQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Genres}";

        List<GenreDto>? genreDtosCache = await _cacheService
            .GetAsync<List<GenreDto>>(key, cancellationToken);
        if (genreDtosCache != null)
        {
            return genreDtosCache;
        }

        List<GenreDto> genreDtos = await _dbContext.Genres
                .AsNoTracking()
                .ProjectToType<GenreDto>()
                .ToListAsync(cancellationToken);

        await _cacheService.SetAsync(key, genreDtos, CacheOptions.DefaultExpiration, cancellationToken);

        return genreDtos;
    }
}