using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Data;
using FluentResults;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Genres.Queries.GenGenreById;

public class GetGenreByIdQueryHandler : IRequestHandler<GetGenreByIdQuery, Result<GenreDetailDto>>
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetGenreByIdQueryHandler> _logger;
    private readonly FastDeliveruuDbContext _dbContext;

    public GetGenreByIdQueryHandler(
        ICacheService cacheService,
        FastDeliveruuDbContext dbContext,
        ILogger<GetGenreByIdQueryHandler> logger)
    {
        _cacheService = cacheService;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<GenreDetailDto>> Handle(
        GetGenreByIdQuery request,
        CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Genre}-{request.Id}";

        GenreDetailDto? genreCache = await _cacheService.GetAsync<GenreDetailDto>(key, cancellationToken);
        if (genreCache != null)
        {
            return genreCache;
        }

        GenreDetailDto? genreDetailDto = await _dbContext.Genres
            .Where(g => g.Id == request.Id)
            .AsNoTracking()
            .ProjectToType<GenreDetailDto>()
            .FirstOrDefaultAsync(cancellationToken);
        if (genreDetailDto == null)
        {
            string message = "Genre not found.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new NotFoundError(message));
        }

        await _cacheService.SetAsync(key, genreDetailDto, CacheOptions.DefaultExpiration, cancellationToken);

        return genreDetailDto;
    }
}