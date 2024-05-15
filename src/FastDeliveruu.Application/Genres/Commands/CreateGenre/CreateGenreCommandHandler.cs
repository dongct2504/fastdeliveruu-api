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

namespace FastDeliveruu.Application.Genres.Commands.CreateGenre;

public class CreateGenreCommandHandler : IRequestHandler<CreateGenreCommand, Result<GenreDto>>
{
    private readonly ICacheService _cacheService;
    private readonly IGenreRepository _genreRepository;
    private readonly IMapper _mapper;

    public CreateGenreCommandHandler(IGenreRepository genreRepository, IMapper mapper, ICacheService cacheService)
    {
        _genreRepository = genreRepository;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<Result<GenreDto>> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
    {
        Genre genre = _mapper.Map<Genre>(request);
        genre.GenreId = Guid.NewGuid();
        genre.CreatedAt = DateTime.Now;
        genre.UpdatedAt = DateTime.Now;

        QueryOptions<Genre> options = new QueryOptions<Genre>
        {
            Where = g => g.Name == genre.Name
        };
        if (await _genreRepository.GetAsync(options) != null)
        {
            string message = "genre is already exist.";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<GenreDto>(new DuplicateError(message));
        }

        await _genreRepository.AddAsync(genre);

        await _cacheService.RemoveByPrefixAsync(CacheConstants.Genres, cancellationToken);

        return _mapper.Map<GenreDto>(genre);
    }
}