using FastDeliveruu.Application.Common;
using FastDeliveruu.Application.Common.Constants;
using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Genres.Queries.GenGenreById;

public class GetGenreByIdQueryHandler : IRequestHandler<GetGenreByIdQuery, Result<GenreDetailDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IGenreRepository _genreRepository;
    private readonly IMapper _mapper;

    public GetGenreByIdQueryHandler(IGenreRepository genreRepository, IMapper mapper, ICacheService cacheService)
    {
        _genreRepository = genreRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<Result<GenreDetailDto>> Handle(GetGenreByIdQuery request, CancellationToken cancellationToken)
    {
        string key = $"{CacheConstants.Genre}-{request.Id}";

        GenreDetailDto? genreCache = await _cacheService.GetAsync<GenreDetailDto>(key, cancellationToken);
        if (genreCache != null)
        {
            return genreCache;
        }

        QueryOptions<Genre> options = new QueryOptions<Genre>
        {
            SetIncludes = "MenuItems",
            Where = g => g.GenreId == request.Id
        };
        Genre? genre = await _genreRepository.GetAsync(options, asNoTracking: true);
        if (genre == null)
        {
            string message = "Genre not found.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<GenreDetailDto>(new NotFoundError(message));
        }

        genreCache = _mapper.Map<GenreDetailDto>(genre);

        await _cacheService.SetAsync(key, genreCache, CacheOptions.DefaultExpiration, cancellationToken);

        return genreCache;
    }
}